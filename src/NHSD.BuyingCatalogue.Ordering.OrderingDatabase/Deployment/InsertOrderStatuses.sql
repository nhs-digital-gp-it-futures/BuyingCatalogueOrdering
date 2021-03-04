IF NOT EXISTS (SELECT * FROM dbo.OrderStatus)
    INSERT INTO dbo.OrderStatus(Id, [Name])
    VALUES
    (1, 'Complete'),
    (2, 'Incomplete');
GO
