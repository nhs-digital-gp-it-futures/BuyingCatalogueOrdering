CREATE TABLE dbo.CatalogueItemType
(
    CatalogueItemTypeId int NOT NULL,
    [Name] nvarchar(20) NOT NULL,
    CONSTRAINT PK_CatalogueItemType PRIMARY KEY (CatalogueItemTypeId),
    CONSTRAINT IX_CatalogueItemTypeName UNIQUE NONCLUSTERED ([Name])
);
