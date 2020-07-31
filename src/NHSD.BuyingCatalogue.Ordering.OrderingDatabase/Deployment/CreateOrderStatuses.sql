DECLARE @complete AS nvarchar(30) = N'Complete';
DECLARE @incomplete AS nvarchar(30) = N'Incomplete';

IF NOT EXISTS (
  SELECT *
  FROM dbo.OrderStatus
  WHERE [Name] IN (@complete, @incomplete))
BEGIN
	INSERT INTO dbo.OrderStatus (OrderStatusId, [Name]) VALUES (1, @complete);
    INSERT INTO dbo.OrderStatus (OrderStatusId, [Name]) VALUES (2, @incomplete);
END
