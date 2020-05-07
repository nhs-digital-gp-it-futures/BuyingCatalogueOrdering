DECLARE @submitted AS nvarchar(30) = N'Submitted';
DECLARE @unsubmitted AS nvarchar(30) = N'Unsubmitted';

IF NOT EXISTS (
  SELECT *
  FROM dbo.OrderStatus
  WHERE [Name] IN (@submitted, @unsubmitted))
BEGIN
	INSERT INTO dbo.OrderStatus ([Name]) VALUES (@submitted), (@unsubmitted)
END
