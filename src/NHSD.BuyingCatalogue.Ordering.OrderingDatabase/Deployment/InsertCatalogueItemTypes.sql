IF NOT EXISTS (SELECT * FROM dbo.CatalogueItemType)
    INSERT INTO dbo.CatalogueItemType(CatalogueItemTypeId, [Name])
    VALUES
    (1, 'Solution'),
    (2, 'Additional Service'),
    (3, 'Associated Service');
GO
