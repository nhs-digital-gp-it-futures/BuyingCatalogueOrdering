CREATE TABLE dbo.DefaultDeliveryDate
(
    OrderId int NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    DeliveryDate date NOT NULL,
    CONSTRAINT PK_DefaultDeliveryDate PRIMARY KEY (OrderId, CatalogueItemId),
    CONSTRAINT FK_DefaultDeliveryDate_Order FOREIGN KEY (OrderId) REFERENCES dbo.[Order] (Id) ON DELETE CASCADE,
);
