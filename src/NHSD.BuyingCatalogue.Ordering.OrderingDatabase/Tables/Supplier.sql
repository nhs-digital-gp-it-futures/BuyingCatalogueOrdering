CREATE TABLE dbo.Supplier
(
    Id nvarchar(6) NOT NULL,
    [Name] nvarchar(256) NOT NULL,
    AddressId int NOT NULL,
    CONSTRAINT PK_Supplier PRIMARY KEY (Id),
    CONSTRAINT FK_Supplier_Address FOREIGN KEY (AddressId) REFERENCES dbo.[Address] (Id),
);
