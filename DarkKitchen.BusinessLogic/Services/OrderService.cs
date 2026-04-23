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
    private IShippingStrategy? _shippingStrategy;
    private const double Vat = 0.22; // VAT es iva
    private const int TopProductsCount = 5;

    public CreateOrderResponseDTO CreateOrder(CreateOrderRequestDTO request)
    {
        var client = _userRepository.Get(u => u.Id == request.ClientId)
           ?? throw new NotFoundException($"Cliente con id {request.ClientId} no encontrado.");

        if(request.Items == null || request.Items.Count == 0)
        {
            throw new ArgumentException("El pedido debe tener al menos un producto.");
        }

        if(!Enum.TryParse<DeliveryType>(request.DeliveryType, out var deliveryType))
        {
            throw new ArgumentException($"Tipo de entrega '{request.DeliveryType}' no válido.");
        }

        var itemsWithProducts = request.Items.Select(i =>
        {
            var product = _productRepository.Get(p => p.Id == i.ProductId)
                ?? throw new NotFoundException($"Producto con id {i.ProductId} no encontrado.");

            if(!product.IsActive)
            {
                throw new ArgumentException($"El producto {product.Name} está inactivo.");
            }

            return (Item: new OrderItem { ProductId = i.ProductId, Quantity = i.Quantity }, Product: product);
        }).ToList();

        var subtotal = itemsWithProducts.Sum(i => i.Product.Price * i.Item.Quantity);

        var promotions = _promotionRepository.GetActivePromotions(DateTime.Today, null, null);

        var discountedSubtotal = ApplyPromotions(subtotal, itemsWithProducts.Select(i => i.Item).ToList(), promotions);

        var shippingCost = CalculateShipping(request.DeliveryType);

        var total = (discountedSubtotal * (1 + Vat)) + shippingCost;

        var order = new Order
        {
            ClientId = request.ClientId,
            DeliveryType = deliveryType,
            Status = OrderStatus.Pending,
            Street = request.Address.Street,
            DoorNumber = request.Address.DoorNumber,
            Apartment = request.Address.Apartment,
            Items = itemsWithProducts.Select(i => i.Item).ToList(),
            Date = DateTime.Now,
        };

        var saved = _orderRepository.Add(order);

        return new CreateOrderResponseDTO
        {
            ClientId = request.ClientId,
            OrderId = saved.Id,
            Subtotal = subtotal,
            ShippingCost = shippingCost,
            Total = Math.Round(total, 2)
        };
    }

    private static double ApplyPromotions(double subtotal, List<OrderItem> items, List<Promotion> promotions)
    {
        double discount = 0;

        foreach(var promotion in promotions)
        {
            var applies = items.Any(i => promotion.Products.Any(p => p.Id == i.ProductId));

            if(applies)
            {
                discount += subtotal * (double)(promotion.DiscountPercentage / 100);
            }
        }

        return subtotal - discount;
    }

    // Implemento Strategy
    private double CalculateShipping(string deliveryType)
    {
        _shippingStrategy = deliveryType switch
        {
            "Express" => new ExpressShipping(),
            "Standard" => new StandardShipping(),
            _ => throw new ArgumentException("Tipo de entrega no válido")
        };

        return _shippingStrategy.CalculateCost();
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

    public UpdateOrderStatusResponseDTO MarkAsPrepared(int orderId)
    {
        var order = _orderRepository.GetOrderById(orderId)
            ?? throw new NotFoundException($"Pedido con id {orderId} no encontrado.");

        if(order.Status != OrderStatus.Pending)
        {
            throw new ArgumentException("El pedido solo puede prepararse si está pendiente.");
        }

        order.Status = OrderStatus.Prepared;
        order.UpdatedAt = DateTime.Now;

        _orderRepository.Update(order);

        return new UpdateOrderStatusResponseDTO
        {
            OrderId = order.Id,
            Status = order.Status.ToString(),
            UpdatedAt = order.UpdatedAt
        };
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

    public UpdateOrderStatusResponseDTO DeliverOrder(int orderId)
    {
        var order = _orderRepository.GetOrderById(orderId)
        ?? throw new NotFoundException($"Pedido con id {orderId} no encontrado.");

        if(order.Status != OrderStatus.OnTheWay)
        {
            throw new ArgumentException("El pedido solo puede entregarse si está en camino.");
        }

        order.Status = OrderStatus.Delivered;
        order.UpdatedAt = DateTime.Now;
        _orderRepository.Update(order);

        return new UpdateOrderStatusResponseDTO
        {
            OrderId = order.Id,
            Status = order.Status.ToString(),
            UpdatedAt = order.UpdatedAt
        };
    }

    public UpdateOrderStatusResponseDTO CancelOrder(int orderId)
    {
        var order = _orderRepository.GetOrderById(orderId)
            ?? throw new NotFoundException($"Pedido con id {orderId} no encontrado.");

        if(order.Status != OrderStatus.Pending)
        {
            throw new ArgumentException("El pedido solo puede cancelarse si está pendiente.");
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.Now;

        _orderRepository.Update(order);

        return new UpdateOrderStatusResponseDTO
        {
            OrderId = order.Id,
            Status = order.Status.ToString(),
            UpdatedAt = order.UpdatedAt
        };
    }

    public UpdateOrderStatusResponseDTO MarkAsOnTheWay(int orderId)
    {
        var order = _orderRepository.GetOrderById(orderId)
         ?? throw new NotFoundException($"Pedido con id {orderId} no encontrado.");

        if(order.Status != OrderStatus.Prepared)
        {
            throw new ArgumentException("El pedido solo puede ponerse en camino si está preparado.");
        }

        order.Status = OrderStatus.OnTheWay;
        order.UpdatedAt = DateTime.Now;

        _orderRepository.Update(order);

        return new UpdateOrderStatusResponseDTO
        {
            OrderId = order.Id,
            Status = order.Status.ToString(),
            UpdatedAt = order.UpdatedAt
        };
    }

    public UpdateOrderStatusResponseDTO MarkAsNotDelivered(int orderId)
    {
        var order = _orderRepository.GetOrderById(orderId)
            ?? throw new NotFoundException($"Pedido con id {orderId} no encontrado.");

        if(order.Status != OrderStatus.OnTheWay)
        {
            throw new ArgumentException("El pedido solo puede marcarse como no entregado si está en camino.");
        }

        order.Status = OrderStatus.NotDelivered;
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
