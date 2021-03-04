CREATE TABLE dbo.OrderProgress
(
    OrderId int NOT NULL,
    ServiceRecipientsViewed bit CONSTRAINT DF_OrderProgress_ServiceRecipientsViewed DEFAULT 0 NOT NULL,
    CatalogueSolutionsViewed bit CONSTRAINT DF_OrderProgress_CatalogueSolutionsViewed DEFAULT 0 NOT NULL,
    AdditionalServicesViewed bit CONSTRAINT DF_OrderProgress_AdditionalServicesViewed DEFAULT 0 NOT NULL,
    AssociatedServicesViewed bit CONSTRAINT DF_OrderProgress_AssociatedServicesViewed DEFAULT 0 NOT NULL,
    CONSTRAINT PK_OrderProgress PRIMARY KEY (OrderId),
    CONSTRAINT FK_OrderProgress_Order FOREIGN KEY (OrderId) REFERENCES dbo.[Order] (Id) ON DELETE CASCADE,
);
