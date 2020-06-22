CREATE TABLE dbo.CataloguePriceType
(
    CataloguePriceTypeId int NOT NULL,
    [Name] nvarchar(10) NOT NULL,
    CONSTRAINT PK_CataloguePriceType PRIMARY KEY (CataloguePriceTypeId),
    CONSTRAINT IX_CataloguePriceTypeName UNIQUE NONCLUSTERED ([Name])
);
