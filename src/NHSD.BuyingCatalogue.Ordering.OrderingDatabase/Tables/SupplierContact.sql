CREATE TABLE dbo.SupplierContact
(
    [SupplierContactId] INT IDENTITY(1, 1) NOT NULL,
    [Name] VARCHAR(100) NULL, 
    [Email] VARCHAR(100) NULL, 
    [Phone] VARCHAR(35) NULL,
    CONSTRAINT PK_SupplierContactId PRIMARY KEY (SupplierContactId)
)
