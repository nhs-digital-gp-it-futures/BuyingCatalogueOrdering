CREATE TABLE dbo.[Address]
(
    Id int IDENTITY(1, 1) NOT NULL, 
    Line1 nvarchar(256) NOT NULL, 
    Line2 nvarchar(256) NULL, 
    Line3 nvarchar(256) NULL, 
    Line4 nvarchar(256) NULL, 
    Line5 nvarchar(256) NULL, 
    Town nvarchar(256) NULL, 
    County nvarchar(256) NULL, 
    Postcode nvarchar(10) NOT NULL, 
    Country nvarchar(256) NULL,
    CONSTRAINT PK_Address PRIMARY KEY (Id),
);
