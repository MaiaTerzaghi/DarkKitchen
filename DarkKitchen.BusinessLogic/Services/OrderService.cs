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
    IRepository<User> userRepository,
    IRepository<ShippingType> shippingTypeRepository) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IPricingService _pricingService = pricingService;
    private readonly IRepository<User> _userRepository = userRepository;
    private readonly IRepository<ShippingType> _shippingTypeRepository = shippingTypeRepository;

    public CreateOrderResponseDTO CreateOrder(CreateOrderRequestDTO request, int clientId)
    {
        ValidateClient(clientId);
        ValidateItems(request.Items);
        var shippingType = ResolveShippingType(request.ShippingType);
        var pricing = _pricingService.CalculateOrderPricing(request.Items, shippingType);
        var order = BuildOrder(request, clientId, shippingType, pricing);
        var saved = _orderRepository.Add(order);
        return BuildOrderResponse(clientId, saved.Id, pricing.Subtotal, pricing.Vat, pricing.ShippingCost, pricing.Total);
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

    public OrderPreviewResponseDTO PreviewOrder(List<OrderItemRequestDTO> items, string shippingTypeName)
    {
        var shippingType = ResolveShippingType(shippingTypeName);
        return _pricingService.PreviewOrderPricing(items, shippingType);
    }

    private ShippingType ResolveShippingType(string shippingTypeName)
    {
        return _shippingTypeRepository.Get(st => st.Name == shippingTypeName)
            ?? throw new ArgumentException($"Tipo de envío '{shippingTypeName}' no válido.");
    }

    private static Order BuildOrder(CreateOrderRequestDTO request, int clientId, ShippingType shippingType, PricingResult pricing)
    {
        return new Order
        {
            ClientId = clientId,
            ShippingTypeId = shippingType.Id,
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

    private static CreateOrderResponseDTO BuildOrderResponse(int clientId, int orderId, double subtotal, double vat, double shippingCost, double total)
    {
        return new CreateOrderResponseDTO
        {
            ClientId = clientId,
            OrderId = orderId,
            Subtotal = subtotal,
            Vat = vat,
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
            ShippingType = order.ShippingType?.Name ?? string.Empty,
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
        ApplyTransition(orderId, order => order.Prepare());

    public UpdateOrderStatusResponseDTO DeliverOrder(int orderId) =>
        ApplyTransition(orderId, order => order.Deliver());

    public UpdateOrderStatusResponseDTO CancelOrder(int orderId) =>
        ApplyTransition(orderId, order => order.Cancel());

    public UpdateOrderStatusResponseDTO MarkAsOnTheWay(int orderId) =>
        ApplyTransition(orderId, order => order.MarkOnTheWay());

    public UpdateOrderStatusResponseDTO MarkAsNotDelivered(int orderId) =>
        ApplyTransition(orderId, order => order.MarkNotDelivered());

    public UpdateOrderStatusResponseDTO MarkAsDelayed(int orderId) =>
        ApplyTransition(orderId, order => order.MarkDelayed());

    private UpdateOrderStatusResponseDTO ApplyTransition(int orderId, Action<Order> transition)
    {
        var order = _orderRepository.GetOrderById(orderId)
            ?? throw new NotFoundException($"Pedido con id {orderId} no encontrado.");

        transition(order);

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
            o => o.Date.Date >= dateFrom.Date && o.Date.Date <= dateTo.Date,
            AppConstants.TopProductsCount);

        return topProducts.Select(p => new TopProductResponseDTO
        {
            Code = p.Product.Code,
            Name = p.Product.Name,
            Quantity = p.Quantity,
            Images = p.Product.Images
        }).ToList();
    }

    public SalesReportWithTotalDTO GetSalesReport(int page, int pageSize)
    {
        var report = _orderRepository.GetSalesReport(page, pageSize);

        var months = report
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
                }).ToList(),
                MonthlyTotal = g.Sum(r => r.Total)
            }).ToList();

        return new SalesReportWithTotalDTO
        {
            Months = months,
            GeneralTotal = months.Sum(m => m.MonthlyTotal)
        };
    }

    public List<GetOrdersResponseDTO> GetDispatcherOrders()
    {
        var orders = _orderRepository.GetDispatcherOrders();

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

    public void ChangeStatus(int orderId, OrderStatus newStatus, string responsibleUser)
    {
        if (!StatusDispatch.TryGetValue(newStatus, out var transition))
        {
            throw new ArgumentException($"Estado {newStatus} no soporta transición.");
        }

        ApplyTransition(orderId, transition);
    }

    private static readonly Dictionary<OrderStatus, Action<Order>> StatusDispatch = new()
    {
        { OrderStatus.Prepared, order => order.Prepare() },
        { OrderStatus.Cancelled, order => order.Cancel() },
        { OrderStatus.OnTheWay, order => order.MarkOnTheWay() },
        { OrderStatus.Delivered, order => order.Deliver() },
        { OrderStatus.NotDelivered, order => order.MarkNotDelivered() },
        { OrderStatus.Delayed, order => order.MarkDelayed() },
    };
}
