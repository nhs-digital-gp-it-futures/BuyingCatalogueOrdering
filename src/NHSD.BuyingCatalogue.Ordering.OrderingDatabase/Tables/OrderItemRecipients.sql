CREATE TABLE dbo.OrderItemRecipients
(
    OrderId int NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    OdsCode nvarchar(8) NOT NULL,
    Quantity int NOT NULL CONSTRAINT ServiceRecipient_PositiveQuantity CHECK (Quantity > 0),
    DeliveryDate date NOT NULL,
    CONSTRAINT PK_OrderItemRecipients PRIMARY KEY (OrderId, CatalogueItemId, OdsCode),
    CONSTRAINT FK_OrderItemRecipients_OrderItem FOREIGN KEY (OrderId, CatalogueItemId) REFERENCES dbo.OrderItem (OrderId, CatalogueItemId) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItemRecipients_OdsCode FOREIGN KEY (OdsCode) REFERENCES dbo.ServiceRecipient (OdsCode),
);
