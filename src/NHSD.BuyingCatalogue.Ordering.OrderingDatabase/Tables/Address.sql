CREATE TABLE dbo.[Address]
(
    AddressId INT IDENTITY(1, 1) NOT NULL, 
    Line1 NVARCHAR(256) NULL, 
    Line2 NVARCHAR(256) NULL, 
    Line3 NVARCHAR(256) NULL, 
    Line4 NVARCHAR(256) NULL, 
    Line5 NVARCHAR(256) NULL, 
    Town NVARCHAR(256) NULL, 
    County NVARCHAR(256) NULL, 
    Postcode NVARCHAR(10) NULL, 
    Country NVARCHAR(256) NULL,
    CONSTRAINT PK_Address PRIMARY KEY (AddressId)
);
