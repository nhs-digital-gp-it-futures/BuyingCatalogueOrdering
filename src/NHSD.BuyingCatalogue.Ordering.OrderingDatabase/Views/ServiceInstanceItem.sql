CREATE VIEW dbo.ServiceInstanceItem AS
SELECT i.OrderId, i.OrderItemId,
       CASE WHEN (i.CatalogueItemTypeId = 2 AND h.HasParentSolution = 0) OR s.ServiceInstanceIncrement IS NULL
         THEN NULL
         ELSE CONCAT('SI', s.ServiceInstanceIncrement, '-', i.OdsCode)
       END AS ServiceInstanceId
  FROM dbo.OrderItem AS i
  CROSS APPLY dbo.GetServiceInstanceIncrement(i.OrderId, i.OrderItemId) AS s
  CROSS APPLY dbo.HasParentSolution(i.OrderId, i.OrderItemId) AS h;
