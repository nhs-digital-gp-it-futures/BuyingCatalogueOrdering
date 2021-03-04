CREATE TABLE dbo.CataloguePriceType
(
    Id int NOT NULL,
    [Name] nvarchar(10) NOT NULL,
    CONSTRAINT PK_CataloguePriceType PRIMARY KEY (Id),
    CONSTRAINT AK_CataloguePriceType_Name UNIQUE NONCLUSTERED ([Name]),
);
