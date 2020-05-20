CREATE TABLE dbo.[Order]
(
    OrderId NVARCHAR(10) NOT NULL PRIMARY KEY,
    [Description] NVARCHAR(100) NOT NULL,
    OrganisationId UNIQUEIDENTIFIER NOT NULL,
    OrderStatusId INT NOT NULL,
    Created DATETIME2 NOT NULL,
    LastUpdated DATETIME2 NOT NULL,
    LastUpdatedBy UNIQUEIDENTIFIER NOT NULL,
    LastUpdatedByName NVARCHAR(256) NULL

    CONSTRAINT FK_Order_OrderStatusId_OrderStatus_OrderStatusId FOREIGN KEY (OrderStatusId) REFERENCES OrderStatus (OrderStatusId)    
)
