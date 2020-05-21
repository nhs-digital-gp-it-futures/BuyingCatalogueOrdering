CREATE TABLE dbo.Contact
(
    [ContactId] INT IDENTITY(1, 1) NOT NULL,
    FirstName VARCHAR(100) NULL, 
    LastName NVARCHAR(100) NULL,
    Email VARCHAR(100) NULL, 
    Phone VARCHAR(35) NULL,
    CONSTRAINT PK_SupplierContact PRIMARY KEY (ContactId)
);
