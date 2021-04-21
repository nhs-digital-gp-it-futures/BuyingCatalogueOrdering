CREATE VIEW dbo.ServiceInstanceItem AS
WITH ServiceInstanceIncrement AS
(
    SELECT r.OrderId, r.CatalogueItemId, r.OdsCode,
           DENSE_RANK() OVER (
               PARTITION BY r.OrderId, r.OdsCode
                   ORDER BY CASE WHEN c.CatalogueItemTypeId = 1 THEN r.CatalogueItemId ELSE c.ParentCatalogueItemId END) AS ServiceInstanceIncrement
      FROM dbo.OrderItemRecipients AS r
           INNER JOIN dbo.CatalogueItem AS c ON c.Id = r.CatalogueItemId
                   AND c.CatalogueItemTypeId IN (1, 2)
       WHERE (c.ParentCatalogueItemId IS NULL OR EXISTS (
            SELECT *
              FROM dbo.OrderItemRecipients AS r2
             WHERE r2.OrderId = r.OrderId
               AND r2.OdsCode = r.OdsCode
               AND r2.CatalogueItemId = c.ParentCatalogueItemId))
)
SELECT r.OrderId, r.CatalogueItemId, r.OdsCode,
       'SI' + CAST(s.ServiceInstanceIncrement AS nvarchar(3)) + '-' + r.OdsCode AS ServiceInstanceId
  FROM dbo.OrderItemRecipients AS r
       LEFT OUTER JOIN ServiceInstanceIncrement AS s
               ON s.OrderId = r.OrderId
              AND s.CatalogueItemId = r.CatalogueItemId
              AND s.OdsCode = r.OdsCode;
