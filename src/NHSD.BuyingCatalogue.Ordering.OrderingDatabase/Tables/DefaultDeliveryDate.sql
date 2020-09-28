CREATE TABLE dbo.DefaultDeliveryDate
(
    OrderId nvarchar(10) NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    PriceId int NOT NULL,
    DeliveryDate datetime2 NOT NULL,
    CONSTRAINT PK_DefaultDeliveryDate PRIMARY KEY (OrderId, CatalogueItemId, PriceId),
    CONSTRAINT FK_DefaultDeliveryDate_OrderId FOREIGN KEY (OrderId) REFERENCES dbo.[Order](OrderId) ON DELETE CASCADE,
);
