using DarkKitchen.Domain;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Models;
using DarkKitchen.Domain.States;
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
        ValidateClientExists(clientId);
        ValidateItems(request.Items);

        var shippingType = ResolveShippingType(request.ShippingType);
        var pricing = _pricingService.CalculateOrderPricing(request.Items, shippingType);
        var order = BuildOrder(request, clientId, shippingType, pricing);
        var saved = _orderRepository.Add(order);

        return BuildOrderResponse(clientId, saved.Id, pricing.Subtotal, pricing.Vat, pricing.ShippingCost, pricing.Total);
    }

    private void ValidateClientExists(int clientId)
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

    public PaginatedResponse<GetOrdersResponseDTO> GetOrders(GetOrdersRequestDTO request, UserRole role, int? clientId)
    {
        var filterClientId = role == UserRole.Client ? clientId : null;

        var (orders, totalCount) = _orderRepository.GetOrders(
            filterClientId,
            request.DateFrom,
            request.DateTo,
            request.Street,
            request.Status,
            request.Page,
            request.PageSize);

        return new PaginatedResponse<GetOrdersResponseDTO>
        {
            Items = orders.Select(MapToOrderResponse).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public OrderDetailResponseDTO GetOrderDetail(int orderId)
    {
        var order = _orderRepository.GetOrderById(orderId) ?? throw new NotFoundException($"Pedido con id {orderId} no encontrado.");

        return MapToDetailDTO(order);
    }

    public UpdateOrderStatusResponseDTO MarkAsPrepared(int orderId) =>
        ApplyTransition(orderId, order =>
        {
            var state = OrderStateFactory.Create(order.Status);
            state.Prepare(order);
        });

    public UpdateOrderStatusResponseDTO DeliverOrder(int orderId) =>
        ApplyTransition(orderId, order =>
        {
            var state = OrderStateFactory.Create(order.Status);
            state.Deliver(order);
        });

    public UpdateOrderStatusResponseDTO CancelOrder(int orderId) =>
        ApplyTransition(orderId, order =>
        {
            var state = OrderStateFactory.Create(order.Status);
            state.Cancel(order);
        });

    public UpdateOrderStatusResponseDTO MarkAsOnTheWay(int orderId) =>
        ApplyTransition(orderId, order =>
        {
            var state = OrderStateFactory.Create(order.Status);
            state.MarkOnTheWay(order);
        });

    public UpdateOrderStatusResponseDTO MarkAsNotDelivered(int orderId) =>
        ApplyTransition(orderId, order =>
        {
            var state = OrderStateFactory.Create(order.Status);
            state.MarkNotDelivered(order);
        });

    public UpdateOrderStatusResponseDTO MarkAsDelayed(int orderId) =>
        ApplyTransition(orderId, order =>
        {
            var state = OrderStateFactory.Create(order.Status);
            state.MarkDelayed(order);
        });

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
        var (report, totalCount) = _orderRepository.GetSalesReport(page, pageSize);

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
            GeneralTotal = months.Sum(m => m.MonthlyTotal),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public List<GetOrdersResponseDTO> GetDispatcherOrders()
    {
        var orders = _orderRepository.GetDispatcherOrders();
        return orders.Select(MapToOrderResponse).ToList();
    }

    private static OrderDetailResponseDTO MapToDetailDTO(Order order)
    {
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

    private static GetOrdersResponseDTO MapToOrderResponse(Order order)
    {
        return new GetOrdersResponseDTO
        {
            OrderId = order.Id,
            Client = order.Client != null ? new ClientInfoDTO
            {
                Id = order.Client.Id,
                Name = order.Client.Name,
                LastName = order.Client.LastName,
                Phone = order.Client.Phone
            }
            : new ClientInfoDTO(),
            Date = order.Date,
            Status = order.Status.ToString(),
            Total = order.Total,
            ItemCount = order.Items.Sum(i => i.Quantity),
            Items = order.Items.Select(item => new OrderItemResponseDTO
            {
                ProductName = item.Product.Name,
                Quantity = item.Quantity
            }).ToList()
        };
    }
}
