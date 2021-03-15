CREATE FUNCTION dbo.GetServiceInstanceIncrement
(
    @orderId int,
    @catalogueItemId nvarchar(14)
)
RETURNS TABLE
AS
RETURN
(
    WITH CalculateServiceInstance AS (
        SELECT DISTINCT o.OrderId, o.CatalogueItemId, o.OdsCode,
               CASE WHEN i.CatalogueItemTypeId IN (1, 2)
                    THEN DENSE_RANK() OVER (
                        PARTITION BY o.OrderId, o.OdsCode,
                                     CASE WHEN i.CatalogueItemTypeId IN (1, 2)
                                          THEN 1
                                          ELSE 0 END
                            ORDER BY CASE WHEN i.CatalogueItemTypeId = 1
                                          THEN i.Id
                                          ELSE i.ParentCatalogueItemId END)
                    ELSE NULL END AS ServiceInstanceIncrement
          FROM dbo.OrderItemRecipients AS o
               INNER JOIN dbo.CatalogueItem AS i
                       ON i.Id = o.CatalogueItemId
         WHERE o.OrderId = @orderId)
    SELECT c.OrderId, c.CatalogueItemId, c.OdsCode, c.ServiceInstanceIncrement
      FROM CalculateServiceInstance AS c
     WHERE c.OrderId = @orderId
       AND c.CatalogueItemId = @catalogueItemId
);
