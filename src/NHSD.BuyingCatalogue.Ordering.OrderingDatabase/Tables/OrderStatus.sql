CREATE TABLE dbo.OrderStatus
(
    OrderStatusId INT NOT NULL,
    [Name] NVARCHAR(30) NOT NULL,

    CONSTRAINT PK_OrderStatusId PRIMARY KEY (OrderStatusId)
)
