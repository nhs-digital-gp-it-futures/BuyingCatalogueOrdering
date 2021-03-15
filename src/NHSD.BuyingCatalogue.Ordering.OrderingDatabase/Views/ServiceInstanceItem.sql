CREATE VIEW dbo.ServiceInstanceItem AS
SELECT DISTINCT r.OrderId, r.CatalogueItemId, r.OdsCode,
       CASE WHEN (c.CatalogueItemTypeId = 2 AND h.HasParentSolution = 0) OR s.ServiceInstanceIncrement IS NULL
         THEN NULL
         ELSE CONCAT('SI', s.ServiceInstanceIncrement, '-', r.OdsCode)
       END AS ServiceInstanceId
  FROM dbo.OrderItemRecipients AS r
  INNER JOIN dbo.CatalogueItem AS c ON c.Id = r.CatalogueItemId
  CROSS APPLY dbo.GetServiceInstanceIncrement(r.OrderId, r.CatalogueItemId) AS s
  CROSS APPLY dbo.HasParentSolution(r.OrderId, r.CatalogueItemId) AS h;
