CREATE TABLE dbo.ServiceRecipient
(
    ServiceRecipientId INT IDENTITY(1, 1) NOT NULL,
    OrderId NVARCHAR(10) NOT NULL,
    [Name] NVARCHAR(256) NULL, 
    OdsCode NVARCHAR(8) NULL, 
    CONSTRAINT PK_ServiceRecipient PRIMARY KEY (ServiceRecipientId),    
    CONSTRAINT FK_ServiceRecipient_Order FOREIGN KEY (OrderId) REFERENCES [Order] (OrderId)
);
