CREATE TABLE dbo.OrderStatus
(
    Id int NOT NULL,
    [Name] nvarchar(30) NOT NULL,
    CONSTRAINT PK_OrderStatus PRIMARY KEY (Id),
    CONSTRAINT AK_OrderStatus_Name UNIQUE NONCLUSTERED ([Name]),
);
