using DarkKitchen.BusinessLogic.Shipping;
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
    IRepository<Product> productRepository,
    IPromotionRepository promotionRepository,
    IRepository<User> userRepository) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IRepository<Product> _productRepository = productRepository;

    private readonly IPromotionRepository _promotionRepository = promotionRepository;
    private readonly IRepository<User> _userRepository = userRepository;
    private const double Vat = 0.22;
    private const int TopProductsCount = 5;

    public CreateOrderResponseDTO CreateOrder(CreateOrderRequestDTO request)
    {
        ValidateClient(request.ClientId);
        ValidateItems(request.Items);
        var deliveryType = ParseDeliveryType(request.DeliveryType);
        var itemsWithProducts = BuildOrderItems(request.Items);
        var items = itemsWithProducts.Select(i => i.Item).ToList();
        var subtotal = itemsWithProducts.Sum(i => i.Product.Price * i.Item.Quantity);
        var promotions = _promotionRepository.GetActivePromotions(DateTime.Today, null, null);
        var discountedSubtotal = ApplyPromotions(subtotal, items, promotions);
        var discount = subtotal - discountedSubtotal;
        var shippingCost = CalculateShipping(deliveryType);
        var total = CalculateTotal(discountedSubtotal, shippingCost);
        var order = BuildOrder(request, deliveryType, items, subtotal, discount, shippingCost);
        var saved = _orderRepository.Add(order);
        return BuildOrderResponse(request.ClientId, saved.Id, subtotal, shippingCost, total);
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

    private List<(OrderItem Item, Product Product)> BuildOrderItems(List<OrderItemRequestDTO> items)
    {
        return items.Select(i =>
        {
            var product = _productRepository.Get(p => p.Id == i.ProductId)
                ?? throw new NotFoundException($"Producto con id {i.ProductId} no encontrado.");

            if(!product.IsActive)
            {
                throw new ArgumentException($"El producto {product.Name} está inactivo.");
            }

            return (Item: new OrderItem { ProductId = i.ProductId, Quantity = i.Quantity, UnitPrice = product.Price, Product = product }, Product: product);
        }).ToList();
    }

    private double CalculateTotal(double discountedSubtotal, double shippingCost)
    {
        return Math.Round((discountedSubtotal * (1 + Vat)) + shippingCost, 2);
    }

    private static Order BuildOrder(CreateOrderRequestDTO request, DeliveryType deliveryType, List<OrderItem> items, double subtotal, double discount, double shippingCost)
    {
        return new Order
        {
            ClientId = request.ClientId,
            DeliveryType = deliveryType,
            Status = OrderStatus.Pending,
            Street = request.Address.Street,
            DoorNumber = request.Address.DoorNumber,
            Apartment = request.Address.Apartment,
            Items = items,
            Subtotal = subtotal,
            Discount = discount,
            ShippingCost = shippingCost,
            Date = DateTime.Now,
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

    private static double ApplyPromotions(double subtotal, List<OrderItem> items, List<Promotion> promotions)
    {
        double discount = 0;

        foreach(var item in items)
        {
            var bestPromotion = promotions
                .Where(p => p.Products.Any(prod => prod.Id == item.ProductId))
                .MaxBy(p => p.DiscountPercentage);

            if(bestPromotion != null)
            {
                var itemSubtotal = item.Product.Price * item.Quantity;
                discount += itemSubtotal * (double)(bestPromotion.DiscountPercentage / 100);
            }
        }

        return subtotal - discount;
    }

    private double CalculateShipping(DeliveryType deliveryType)
    {
        IShippingStrategy shippingStrategy = deliveryType switch
        {
            DeliveryType.Express => new ExpressShipping(),
            DeliveryType.Standard => new StandardShipping(),
            _ => throw new ArgumentException("Tipo de entrega no válido")
        };

        return shippingStrategy.CalculateCost();
    }

    public List<GetClientOrdersResponseDTO> GetClientOrders(GetClientOrdersRequestDTO request)
    {
        var orders = _orderRepository.GetClientOrders(
        request.ClientId,
        request.Status,
        request.DateFrom,
        request.DateTo);
        return orders.Select(o => new GetClientOrdersResponseDTO
        {
            OrderId = o.Id,
            ClientId = o.ClientId,
            Status = o.Status.ToString(),
            Total = o.Items.Sum(i => i.Product.Price * i.Quantity),
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
            ClientName = order.ClientId.ToString(),
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
            Total = order.Items.Sum(i => i.Product.Price * i.Quantity),
            Items = order.Items.Select(i => new OrderItemDetailDTO
            {
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                UnitPrice = i.Product.Price,
                Subtotal = i.Product.Price * i.Quantity
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
            throw new ArgumentException(errorMessage);
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
            TopProductsCount);

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
                    Total = r.Total
                }).ToList()
            }).ToList();
    }
}
