using DarkKitchen.Domain;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
namespace DarkKitchen.BusinessLogic.Services;

public class OrderService(
    IOrderRepository orderRepository,
    IPricingService pricingService,
    IRepository<User> userRepository) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IPricingService _pricingService = pricingService;
    private readonly IRepository<User> _userRepository = userRepository;

    public CreateOrderResponseDTO CreateOrder(CreateOrderRequestDTO request, int clientId)
    {
        ValidateClient(clientId);
        ValidateItems(request.Items);
        var deliveryType = ParseDeliveryType(request.DeliveryType);
        var pricing = _pricingService.CalculateOrderPricing(request.Items, deliveryType);
        var order = BuildOrder(request, clientId, deliveryType, pricing);
        var saved = _orderRepository.Add(order);
        return BuildOrderResponse(clientId, saved.Id, pricing.Subtotal, pricing.ShippingCost, pricing.Total);
    }

    private void ValidateClient(int clientId)
    {
        _ = _userRepository.Get(u => u.Id == clientId)
            ?? throw new NotFoundException($"Cliente con id {clientId} no encontrado.");
    }

    private static void ValidateItems(List<OrderItemRequestDTO>? items)
    {
        if(items == null || items.Count == 0)
        {
            throw new ArgumentException("El pedido debe tener al menos un producto.");
        }
    }

    private static DeliveryType ParseDeliveryType(string deliveryType)
    {
        if(!Enum.TryParse<DeliveryType>(deliveryType, out var result))
        {
            throw new ArgumentException($"Tipo de entrega '{deliveryType}' no válido.");
        }

        return result;
    }

    private static Order BuildOrder(CreateOrderRequestDTO request, int clientId, DeliveryType deliveryType, PricingResult pricing)
    {
        return new Order
        {
            ClientId = clientId,
            DeliveryType = deliveryType,
            Status = OrderStatus.Pending,
            Street = request.Address.Street,
            DoorNumber = request.Address.DoorNumber,
            Apartment = request.Address.Apartment,
            Items = pricing.Items,
            Subtotal = pricing.Subtotal,
            Discount = pricing.Discount,
            ShippingCost = pricing.ShippingCost,
            Vat = pricing.Vat,
            Total = pricing.Total,
        };
    }

    private static CreateOrderResponseDTO BuildOrderResponse(int clientId, int orderId, double subtotal, double shippingCost, double total)
    {
        return new CreateOrderResponseDTO
        {
            ClientId = clientId,
            OrderId = orderId,
            Subtotal = subtotal,
            ShippingCost = shippingCost,
            Total = total
        };
    }

    public List<GetClientOrdersResponseDTO> GetClientOrders(GetClientOrdersRequestDTO request, int clientId)
    {
        var orders = _orderRepository.GetClientOrders(
            clientId,
            request.Status,
            request.DateFrom,
            request.DateTo);
        return orders.Select(o => new GetClientOrdersResponseDTO
        {
            OrderId = o.Id,
            ClientId = o.ClientId,
            Date = o.Date,
            Status = o.Status.ToString(),
            Total = o.Total,
            ItemCount = o.Items.Sum(i => i.Quantity)
        }).ToList();
    }

    public List<GetOrdersResponseDTO> GetOrders(GetOrdersRequestDTO request)
    {
        var orders = _orderRepository.GetOrders(
        request.DateFrom,
        request.DateTo,
        request.Street,
        request.Status);

        return orders.Select(order => new GetOrdersResponseDTO
        {
            OrderId = order.Id,
            Client = new ClientInfoDTO
            {
                Id = order.Client.Id,
                Name = order.Client.Name,
                LastName = order.Client.LastName,
                Phone = order.Client.Phone
            },
            Date = order.Date,
            Status = order.Status.ToString(),
            Items = order.Items.Select(item => new OrderItemResponseDTO
            {
                ProductName = item.Product.Name,
                Quantity = item.Quantity
            }).ToList()
        }).ToList();
    }

    public OrderDetailResponseDTO GetOrderDetail(int orderId)
    {
        var order = _orderRepository.GetOrderById(orderId) ?? throw new NotFoundException($"Pedido con id {orderId} no encontrado.");

        return new OrderDetailResponseDTO
        {
            OrderId = order.Id,
            ClientId = order.ClientId,
            Date = order.Date,
            Status = order.Status.ToString(),
            DeliveryType = order.DeliveryType.ToString(),
            Total = order.Total,
            Items = order.Items.Select(i => new OrderItemDetailDTO
            {
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Subtotal = i.UnitPrice * i.Quantity
            }).ToList()
        };
    }

    public UpdateOrderStatusResponseDTO MarkAsPrepared(int orderId) =>
        TransitionOrder(orderId, OrderStatus.Pending, OrderStatus.Prepared, "El pedido solo puede prepararse si está pendiente.");

    public UpdateOrderStatusResponseDTO DeliverOrder(int orderId) =>
        TransitionOrder(orderId, OrderStatus.OnTheWay, OrderStatus.Delivered, "El pedido solo puede entregarse si está en camino.");

    public UpdateOrderStatusResponseDTO CancelOrder(int orderId) =>
        TransitionOrder(orderId, OrderStatus.Pending, OrderStatus.Cancelled, "El pedido solo puede cancelarse si está pendiente.");

    public UpdateOrderStatusResponseDTO MarkAsOnTheWay(int orderId) =>
        TransitionOrder(orderId, OrderStatus.Prepared, OrderStatus.OnTheWay, "El pedido solo puede ponerse en camino si está preparado.");

    public UpdateOrderStatusResponseDTO MarkAsNotDelivered(int orderId) =>
        TransitionOrder(orderId, OrderStatus.OnTheWay, OrderStatus.NotDelivered, "El pedido solo puede marcarse como no entregado si está en camino.");

    private UpdateOrderStatusResponseDTO TransitionOrder(int orderId, OrderStatus requiredStatus, OrderStatus newStatus, string errorMessage)
    {
        var order = _orderRepository.GetOrderById(orderId)
            ?? throw new NotFoundException($"Pedido con id {orderId} no encontrado.");

        if(order.Status != requiredStatus)
        {
            throw new ConflictException(errorMessage);
        }

        order.Status = newStatus;
        order.UpdatedAt = DateTime.Now;

        _orderRepository.Update(order);

        return new UpdateOrderStatusResponseDTO
        {
            OrderId = order.Id,
            Status = order.Status.ToString(),
            UpdatedAt = order.UpdatedAt
        };
    }

    public List<TopProductResponseDTO> GetTopProducts(DateTime dateFrom, DateTime dateTo)
    {
        var topProducts = _orderRepository.GetTopProducts(
            o => o.Date >= dateFrom && o.Date <= dateTo,
            AppConstants.TopProductsCount);

        return topProducts.Select(p => new TopProductResponseDTO
        {
            Code = p.Product.Code,
            Name = p.Product.Name,
            Quantity = p.Quantity,
            Images = p.Product.Images
        }).ToList();
    }

    public List<SalesReportResponseDTO> GetSalesReport(int page, int pageSize)
    {
        var report = _orderRepository.GetSalesReport(page, pageSize);

        return report
            .GroupBy(r => new { r.Year, r.Month })
            .Select(g => new SalesReportResponseDTO
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Clients = g.Select(r => new ClientSalesDTO
                {
                    ClientId = r.ClientId,
                    ClientName = r.ClientName,
                    Total = r.Total
                }).ToList()
            }).ToList();
    }
}
