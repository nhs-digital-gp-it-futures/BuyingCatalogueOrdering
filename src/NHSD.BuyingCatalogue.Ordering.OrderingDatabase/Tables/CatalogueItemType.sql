CREATE TABLE dbo.CatalogueItemType
(
    Id int NOT NULL,
    [Name] nvarchar(20) NOT NULL,
    CONSTRAINT PK_CatalogueItemType PRIMARY KEY (Id),
    CONSTRAINT AK_CatalogueItemType_Name UNIQUE NONCLUSTERED ([Name]),
);
