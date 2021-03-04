CREATE TABLE dbo.CatalogueItem
(
    Id nvarchar(14) NOT NULL,
    CatalogueItemTypeId int NOT NULL,
    [Name] nvarchar(256) NOT NULL,
    ParentCatalogueItemId nvarchar(14) NULL,
    CONSTRAINT PK_CatalogueItem PRIMARY KEY (Id),
    CONSTRAINT FK_CatalogueItem_CatalogueItemType FOREIGN KEY (CatalogueItemTypeId) REFERENCES dbo.CatalogueItemType (Id),
    CONSTRAINT FK_CatalogueItem_ParentCatalogueItem FOREIGN KEY (ParentCatalogueItemId) REFERENCES dbo.CatalogueItem (Id),
    INDEX IX_CatalogueItem_Type NONCLUSTERED (CatalogueItemTypeId, [Name]),
);
