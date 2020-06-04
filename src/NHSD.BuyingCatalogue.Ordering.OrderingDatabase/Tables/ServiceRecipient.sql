CREATE TABLE dbo.ServiceRecipient
(
    OdsCode NVARCHAR(8) NOT NULL, 
    OrderId NVARCHAR(10) NOT NULL,
    [Name] NVARCHAR(256) NULL, 
    CONSTRAINT PK_ServiceRecipient PRIMARY KEY (OdsCode),
    CONSTRAINT FK_ServiceRecipient_Order FOREIGN KEY (OrderId) REFERENCES [Order] (OrderId)
);
