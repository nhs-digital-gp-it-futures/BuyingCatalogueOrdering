CREATE UNIQUE NONCLUSTERED INDEX AK_OrderingParty_OdsCode
ON dbo.OrderingParty (OdsCode)
WHERE OdsCode IS NOT NULL;
