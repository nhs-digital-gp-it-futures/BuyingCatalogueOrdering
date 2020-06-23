IF NOT EXISTS (SELECT * FROM dbo.CataloguePriceType)
    INSERT INTO dbo.CataloguePriceType(CataloguePriceTypeId, [Name])
    VALUES
    (1, 'Flat'),
    (2, 'Tiered');
GO
