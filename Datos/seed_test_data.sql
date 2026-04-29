-- ============================================================
-- DarkKitchen - Script de datos de prueba para Postman
-- Equipo: 306213 - 310790 - 303018
-- ============================================================
-- Password para TODOS los usuarios: Passw0rd!Testing
-- Hash SHA256 Base64: 79akb8ulCHI4EQWXX/5cY0fJagfLseZYWC3Ur0MInoQ=
-- ============================================================
-- Roles: Client=0, Dispatcher=1, Administrative=2
-- OrderStatus: Pending=0, Prepared=1, Cancelled=2, OnTheWay=3, Delivered=4, NotDelivered=5
-- DeliveryType: almacenado como string ("Express", "Standard")
-- Shipping: Express=50, Standard=20
-- IVA: 22%
-- ============================================================

-- Limpiar datos existentes (en orden por FKs)
DELETE FROM [OrderItem];
DELETE FROM [PromotionProducts];
DELETE FROM [Sessions];
DELETE FROM [Orders];
DELETE FROM [Promotions];
DELETE FROM [Products];
DELETE FROM [Users];

-- Resetear identity seeds
DBCC CHECKIDENT ('Users', RESEED, 0);
DBCC CHECKIDENT ('Products', RESEED, 0);
DBCC CHECKIDENT ('Promotions', RESEED, 0);
DBCC CHECKIDENT ('Orders', RESEED, 0);
DBCC CHECKIDENT ('OrderItem', RESEED, 0);

-- ============================================================
-- USUARIOS (4)
-- ============================================================
SET IDENTITY_INSERT [Users] ON;

INSERT INTO [Users] (Id, Name, LastName, Email, Phone, Password, Role) VALUES
(1, 'Admin',   'Sistema',     'admin@darkkitchen.com',      '+59899100001', '79akb8ulCHI4EQWXX/5cY0fJagfLseZYWC3Ur0MInoQ=', 2),
(2, 'Carlos',  'Preparador',  'dispatcher@darkkitchen.com', '+59899100002', '79akb8ulCHI4EQWXX/5cY0fJagfLseZYWC3Ur0MInoQ=', 1),
(3, 'Juan',    'Perez',       'cliente@darkkitchen.com',    '+59899100003', '79akb8ulCHI4EQWXX/5cY0fJagfLseZYWC3Ur0MInoQ=', 0),
(4, 'Maria',   'Garcia Lopez','maria@darkkitchen.com',      '+59899100004', '79akb8ulCHI4EQWXX/5cY0fJagfLseZYWC3Ur0MInoQ=', 0);

SET IDENTITY_INSERT [Users] OFF;

-- ============================================================
-- PRODUCTOS (7)
-- ============================================================
-- Validaciones: Code 5-20 chars, Name 10-50 chars, Description 20-500 chars,
--               Price > 0, CommercialLine no vacia, Category no vacia,
--               Images: al menos 1, max 3, formato .jpg
SET IDENTITY_INSERT [Products] ON;

INSERT INTO [Products] (Id, Code, Name, Description, Price, CommercialLine, Category, Images, IsActive) VALUES
(1, 'PROD-00001', 'Pizza Margherita',      'Pizza clasica con salsa de tomate, mozzarella y albahaca fresca',                350.00, 'Minutas clasicas', 'Pastas',    'pizza_margherita.jpg',                      1),
(2, 'PROD-00002', 'Hamburguesa Clasica',   'Hamburguesa de carne vacuna con lechuga, tomate y queso cheddar',                280.00, 'Combo burgers',    'Fritos',    'hamburguesa_clasica.jpg',                   1),
(3, 'PROD-00003', 'Pasta Bolognesa',       'Pasta italiana con salsa bolognesa casera de carne y tomate',                    320.00, 'Minutas clasicas', 'Pastas',    'pasta_bolognesa.jpg',                       1),
(4, 'PROD-00004', 'Ensalada Caesar Premium','Ensalada con pollo grillado, crutones, parmesano y aderezo caesar',             250.00, 'Desayunos',        'Ensaladas', 'ensalada_caesar.jpg',                       1),
(5, 'PROD-00005', 'Milanesa Napolitana',   'Milanesa de ternera con salsa de tomate, jamon y queso mozzarella',              400.00, 'Minutas clasicas', 'Parrilla',  'milanesa_napo1.jpg, milanesa_napo2.jpg',    1),
(6, 'PROD-00006', 'Tostado Mixto Especial','Tostado de jamon y queso con oregano en pan de campo artesanal',                180.00, 'Desayunos',        'Fritos',    'tostado_mixto.jpg',                         1),
(7, 'PROD-00007', 'Producto Inactivo Test','Producto de prueba que esta desactivado para verificar la validacion',           100.00, 'Combo burgers',    'Fritos',    'producto_inactivo.jpg',                     0);

SET IDENTITY_INSERT [Products] OFF;

-- ============================================================
-- PROMOCIONES (3)
-- ============================================================
-- Validaciones: Name no vacio, DiscountPercentage > 0 y <= 100, ValidTo >= ValidFrom
SET IDENTITY_INSERT [Promotions] ON;

INSERT INTO [Promotions] (Id, Name, DiscountPercentage, ValidFrom, ValidTo, ProductLine) VALUES
(1, 'Black Friday 2026',   15.00, '2026-01-01', '2026-12-31', ''),
(2, 'Super Descuento VIP', 35.00, '2026-01-01', '2026-12-31', ''),
(3, 'Promo Vencida Test',  10.00, '2025-01-01', '2025-12-31', '');

SET IDENTITY_INSERT [Promotions] OFF;

-- ============================================================
-- RELACION PROMOCION-PRODUCTO (PromotionProducts)
-- ============================================================
-- Promo 1 (15%) tiene Productos 1 y 2
-- Promo 2 (35%) tiene Producto 1
-- Producto 1 (Pizza) esta en dos promos activas: 15% y 35% -> se aplica 35%
-- Promo 3 (vencida) no tiene productos asociados (para testear agregar a promo vencida)

INSERT INTO [PromotionProducts] (ProductsId, PromotionId) VALUES
(1, 1),
(2, 1),
(1, 2);

-- ============================================================
-- PEDIDOS (5)
-- ============================================================
-- Calculos verificados con la logica de PricingService:
--   Subtotal = sum(Price * Qty)
--   DiscountedSubtotal = Subtotal - Discount
--   VAT = Round(DiscountedSubtotal * 0.22, 2)
--   Total = Round(DiscountedSubtotal * 1.22 + ShippingCost, 2)

SET IDENTITY_INSERT [Orders] ON;

-- Pedido 1: PENDING (para testear prepared y cancel)
-- Cliente 3, Express ($50), Producto 2 x2 (Hamburguesa $280, tiene 15% de Promo 1)
-- Subtotal=560, Descuento=84, DiscSub=476, VAT=104.72, Shipping=50, Total=630.72
INSERT INTO [Orders] (Id, ClientId, DeliveryType, Status, Street, DoorNumber, Apartment, Subtotal, Discount, ShippingCost, Vat, Total, Date, UpdatedAt) VALUES
(1, 3, 'Express', 0, '18 de Julio', '1234', 'Apto 301', 560.00, 84.00, 50.00, 104.72, 630.72, '2026-04-15 10:00:00', '2026-04-15 10:00:00');

-- Pedido 2: PREPARED (para testear on-the-way)
-- Cliente 3, Standard ($20), Producto 3 x1 (Pasta $320, sin promo)
-- Subtotal=320, Descuento=0, VAT=70.40, Shipping=20, Total=410.40
INSERT INTO [Orders] (Id, ClientId, DeliveryType, Status, Street, DoorNumber, Apartment, Subtotal, Discount, ShippingCost, Vat, Total, Date, UpdatedAt) VALUES
(2, 3, 'Standard', 1, 'Bulevar Artigas', '567', NULL, 320.00, 0.00, 20.00, 70.40, 410.40, '2026-04-14 14:30:00', '2026-04-14 15:00:00');

-- Pedido 3: ON THE WAY (para testear deliver y not-delivered)
-- Cliente 3, Express ($50), Producto 4 x1 (Ensalada $250, sin promo)
-- Subtotal=250, Descuento=0, VAT=55.00, Shipping=50, Total=355.00
INSERT INTO [Orders] (Id, ClientId, DeliveryType, Status, Street, DoorNumber, Apartment, Subtotal, Discount, ShippingCost, Vat, Total, Date, UpdatedAt) VALUES
(3, 3, 'Express', 3, 'Av. Rivera', '890', 'Apto 102', 250.00, 0.00, 50.00, 55.00, 355.00, '2026-04-13 09:00:00', '2026-04-13 10:30:00');

-- Pedido 4: DELIVERED (para testear "no se puede cancelar", y para reporte - MARZO)
-- Cliente 4, Standard ($20), Producto 1 x2 (Pizza $350, 35% desc) + Producto 5 x1 (Milanesa $400, sin promo)
-- Pizza discounted: 350*2*0.65=455, Milanesa: 400 -> DiscSub=855
-- Subtotal=1100, Descuento=245, VAT=188.10, Shipping=20, Total=1063.10
INSERT INTO [Orders] (Id, ClientId, DeliveryType, Status, Street, DoorNumber, Apartment, Subtotal, Discount, ShippingCost, Vat, Total, Date, UpdatedAt) VALUES
(4, 4, 'Standard', 4, 'Av. Brasil', '2020', NULL, 1100.00, 245.00, 20.00, 188.10, 1063.10, '2026-03-10 12:00:00', '2026-03-10 14:00:00');

-- Pedido 5: DELIVERED (reporte ventas - FEBRERO, otro mes y otro cliente)
-- Cliente 3, Standard ($20), Producto 6 x3 (Tostado $180, sin promo)
-- Subtotal=540, Descuento=0, VAT=118.80, Shipping=20, Total=678.80
INSERT INTO [Orders] (Id, ClientId, DeliveryType, Status, Street, DoorNumber, Apartment, Subtotal, Discount, ShippingCost, Vat, Total, Date, UpdatedAt) VALUES
(5, 3, 'Standard', 4, 'Constituyente', '1800', 'Apto 5', 540.00, 0.00, 20.00, 118.80, 678.80, '2026-02-20 18:00:00', '2026-02-20 19:30:00');

SET IDENTITY_INSERT [Orders] OFF;

-- ============================================================
-- ORDER ITEMS
-- ============================================================
SET IDENTITY_INSERT [OrderItem] ON;

-- Pedido 1: Hamburguesa x2
INSERT INTO [OrderItem] (Id, OrderId, ProductId, Quantity, UnitPrice) VALUES
(1, 1, 2, 2, 280.00);

-- Pedido 2: Pasta x1
INSERT INTO [OrderItem] (Id, OrderId, ProductId, Quantity, UnitPrice) VALUES
(2, 2, 3, 1, 320.00);

-- Pedido 3: Ensalada x1
INSERT INTO [OrderItem] (Id, OrderId, ProductId, Quantity, UnitPrice) VALUES
(3, 3, 4, 1, 250.00);

-- Pedido 4: Pizza x2, Milanesa x1
INSERT INTO [OrderItem] (Id, OrderId, ProductId, Quantity, UnitPrice) VALUES
(4, 4, 1, 2, 350.00),
(5, 4, 5, 1, 400.00);

-- Pedido 5: Tostado x3
INSERT INTO [OrderItem] (Id, OrderId, ProductId, Quantity, UnitPrice) VALUES
(6, 5, 6, 3, 180.00);

SET IDENTITY_INSERT [OrderItem] OFF;

-- ============================================================
-- RESUMEN DE DATOS PARA POSTMAN
-- ============================================================
-- Login con cualquier usuario: Password = Passw0rd!Testing
--
-- Admin:      admin@darkkitchen.com       (Role: Administrative)
-- Dispatcher: dispatcher@darkkitchen.com  (Role: Dispatcher)
-- Cliente 1:  cliente@darkkitchen.com     (Role: Client)
-- Cliente 2:  maria@darkkitchen.com       (Role: Client)
--
-- Productos activos: 1-6 (distintas lineas y categorias)
-- Producto inactivo: 7
--
-- Promo activa 15%: id=1 (productos 1,2)
-- Promo activa 35%: id=2 (producto 1) -> Pizza tiene 35% de descuento
-- Promo vencida:    id=3 (sin productos)
--
-- Pedido Pending:   id=1 (para prepared/cancel)
-- Pedido Prepared:  id=2 (para on-the-way)
-- Pedido OnTheWay:  id=3 (para deliver/not-delivered)
-- Pedido Delivered: id=4 (no se puede cancelar, datos para reporte marzo)
-- Pedido Delivered: id=5 (datos para reporte febrero)
-- ============================================================
