CREATE DATABASE BasketDb
GO

CREATE TABLE [BasketDb].[dbo].[ShoppingCarts]
(
    [ShoppingCartId] [int] IDENTITY(1,1) NOT NULL,
    [ClientId] [int] NOT NULL,
    [ShoppingCartStatus] [int] NOT NULL
) ON [PRIMARY]
GO

CREATE TABLE [BasketDb].[dbo].[ProductItems]
(
    [ProductItemId] [int] IDENTITY(1,1) NOT NULL,
    [ShoppingCartId] [int] NOT NULL,
    [ProductId] [int] NOT NULL,
    [Quantity] [int] NOT NULL,
)
GO