CREATE TABLE dbo.OrderingParty
(
    Id uniqueidentifier NOT NULL,
    OdsCode nvarchar(8) NULL,
    [Name] nvarchar(256) NULL,
    AddressId int NULL,
    CONSTRAINT PK_OrderingParty PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT FK_OrderingParty_Address FOREIGN KEY (AddressId) REFERENCES dbo.[Address] (Id),
    CONSTRAINT AK_OrderingParty_OdsCode UNIQUE CLUSTERED (OdsCode),
);
