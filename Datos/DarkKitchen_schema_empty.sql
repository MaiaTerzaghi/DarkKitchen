-- ============================================================
-- DarkKitchen - Esquema de base de datos vacía
-- Equipo: 306213 - 310790 - 303018
-- SQL Server (EF Core 8.0)
-- ============================================================
-- Este script crea todas las tablas necesarias para la
-- aplicación DarkKitchen sin datos iniciales.
-- ============================================================

IF OBJECT_ID('PromotionProducts', 'U') IS NOT NULL DROP TABLE [PromotionProducts];
IF OBJECT_ID('OrderItem', 'U') IS NOT NULL DROP TABLE [OrderItem];
IF OBJECT_ID('Sessions', 'U') IS NOT NULL DROP TABLE [Sessions];
IF OBJECT_ID('Orders', 'U') IS NOT NULL DROP TABLE [Orders];
IF OBJECT_ID('Products', 'U') IS NOT NULL DROP TABLE [Products];
IF OBJECT_ID('Promotions', 'U') IS NOT NULL DROP TABLE [Promotions];
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE [Users];
IF OBJECT_ID('__EFMigrationsHistory', 'U') IS NOT NULL DROP TABLE [__EFMigrationsHistory];

-- ============================================================
-- Tabla: Users
-- ============================================================
CREATE TABLE [Users] (
    [Id]       INT            IDENTITY(1,1) NOT NULL,
    [Name]     NVARCHAR(MAX)  NOT NULL,
    [LastName] NVARCHAR(MAX)  NOT NULL,
    [Email]    NVARCHAR(MAX)  NOT NULL,
    [Phone]    NVARCHAR(MAX)  NOT NULL,
    [Password] NVARCHAR(MAX)  NOT NULL,
    [Role]     INT            NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

-- ============================================================
-- Tabla: Products
-- ============================================================
CREATE TABLE [Products] (
    [Id]             INT            IDENTITY(1,1) NOT NULL,
    [Code]           NVARCHAR(MAX)  NOT NULL,
    [Name]           NVARCHAR(MAX)  NOT NULL,
    [Description]    NVARCHAR(MAX)  NOT NULL,
    [Price]          FLOAT          NOT NULL,
    [CommercialLine] NVARCHAR(MAX)  NOT NULL,
    [Category]       NVARCHAR(MAX)  NOT NULL,
    [Images]         NVARCHAR(MAX)  NOT NULL,
    [IsActive]       BIT            NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
);

-- ============================================================
-- Tabla: Promotions
-- ============================================================
CREATE TABLE [Promotions] (
    [Id]                 INT            IDENTITY(1,1) NOT NULL,
    [Name]               NVARCHAR(MAX)  NOT NULL,
    [DiscountPercentage] DECIMAL(18,2)  NOT NULL,
    [ValidFrom]          DATETIME2      NOT NULL,
    [ValidTo]            DATETIME2      NOT NULL,
    [ProductLine]        NVARCHAR(MAX)  NOT NULL,
    CONSTRAINT [PK_Promotions] PRIMARY KEY ([Id])
);

-- ============================================================
-- Tabla: Sessions
-- ============================================================
CREATE TABLE [Sessions] (
    [Id]     UNIQUEIDENTIFIER NOT NULL,
    [Token]  NVARCHAR(MAX)    NOT NULL,
    [UserId] INT              NOT NULL,
    CONSTRAINT [PK_Sessions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Sessions_Users_UserId] FOREIGN KEY ([UserId])
        REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_Sessions_UserId] ON [Sessions] ([UserId]);

-- ============================================================
-- Tabla: Orders
-- ============================================================
CREATE TABLE [Orders] (
    [Id]           INT            IDENTITY(1,1) NOT NULL,
    [ClientId]     INT            NOT NULL,
    [DeliveryType] NVARCHAR(MAX)  NOT NULL,
    [Status]       INT            NOT NULL,
    [Date]         DATETIME2      NOT NULL,
    [UpdatedAt]    DATETIME2      NOT NULL,
    [Street]       NVARCHAR(MAX)  NOT NULL,
    [DoorNumber]   NVARCHAR(MAX)  NOT NULL,
    [Apartment]    NVARCHAR(MAX)  NULL,
    [Subtotal]     FLOAT          NOT NULL,
    [Discount]     FLOAT          NOT NULL,
    [ShippingCost] FLOAT          NOT NULL,
    [Vat]          FLOAT          NOT NULL,
    [Total]        FLOAT          NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_Users_ClientId] FOREIGN KEY ([ClientId])
        REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_Orders_ClientId] ON [Orders] ([ClientId]);

-- ============================================================
-- Tabla: OrderItem
-- ============================================================
CREATE TABLE [OrderItem] (
    [Id]        INT   IDENTITY(1,1) NOT NULL,
    [ProductId] INT   NOT NULL,
    [Quantity]  INT   NOT NULL,
    [UnitPrice] FLOAT NOT NULL,
    [OrderId]   INT   NULL,
    CONSTRAINT [PK_OrderItem] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItem_Orders_OrderId] FOREIGN KEY ([OrderId])
        REFERENCES [Orders] ([Id]),
    CONSTRAINT [FK_OrderItem_Products_ProductId] FOREIGN KEY ([ProductId])
        REFERENCES [Products] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_OrderItem_OrderId] ON [OrderItem] ([OrderId]);
CREATE INDEX [IX_OrderItem_ProductId] ON [OrderItem] ([ProductId]);

-- ============================================================
-- Tabla: PromotionProducts (muchos a muchos)
-- ============================================================
CREATE TABLE [PromotionProducts] (
    [ProductsId]  INT NOT NULL,
    [PromotionId] INT NOT NULL,
    CONSTRAINT [PK_PromotionProducts] PRIMARY KEY ([ProductsId], [PromotionId]),
    CONSTRAINT [FK_PromotionProducts_Products_ProductsId] FOREIGN KEY ([ProductsId])
        REFERENCES [Products] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PromotionProducts_Promotions_PromotionId] FOREIGN KEY ([PromotionId])
        REFERENCES [Promotions] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_PromotionProducts_PromotionId] ON [PromotionProducts] ([PromotionId]);

-- ============================================================
-- Tabla: __EFMigrationsHistory (requerida por EF Core)
-- ============================================================
CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId]    NVARCHAR(150) NOT NULL,
    [ProductVersion] NVARCHAR(32)  NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);
