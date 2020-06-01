CREATE TABLE dbo.OrderStatus
(
    OrderStatusId INT IDENTITY(1,1), 
    [Name] NVARCHAR(30) NULL,

    CONSTRAINT PK_OrderStatusId PRIMARY KEY (OrderStatusId)

)
