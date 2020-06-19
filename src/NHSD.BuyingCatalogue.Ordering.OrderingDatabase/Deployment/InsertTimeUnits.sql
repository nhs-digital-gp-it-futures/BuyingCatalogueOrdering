IF NOT EXISTS (SELECT * FROM dbo.TimeUnit)
    INSERT INTO dbo.TimeUnit(TimeUnitId, [Name], [Description])
    VALUES
    (1, 'month', 'per month'),
    (2, 'year', 'per year');
GO
