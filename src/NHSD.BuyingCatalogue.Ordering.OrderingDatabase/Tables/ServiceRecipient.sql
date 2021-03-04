CREATE TABLE dbo.ServiceRecipient
(
    OdsCode nvarchar(8) NOT NULL,
    [Name] nvarchar(256) NULL,
    CONSTRAINT PK_ServiceRecipient PRIMARY KEY (OdsCode),
);
