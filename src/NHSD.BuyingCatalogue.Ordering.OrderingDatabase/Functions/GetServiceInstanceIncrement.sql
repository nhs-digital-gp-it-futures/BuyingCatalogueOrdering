CREATE FUNCTION dbo.GetServiceInstanceIncrement
(
    @orderId nvarchar(10),
    @orderItemId int
)
RETURNS TABLE
AS
RETURN
(
    WITH CalculateServiceInstance AS (
        SELECT o.OrderItemId,
               CASE WHEN o.CatalogueItemTypeId IN (1, 2)
                    THEN DENSE_RANK() OVER (
                        PARTITION BY o.OrderId, o.OdsCode,
                                     CASE WHEN o.CatalogueItemTypeId IN (1, 2)
                                          THEN 1
                                          ELSE 0 END
                            ORDER BY CASE WHEN o.CatalogueItemTypeId = 1
                                          THEN o.CatalogueItemId
                                          ELSE o.ParentCatalogueItemId END)
                    ELSE NULL END AS ServiceInstanceIncrement
          FROM dbo.OrderItem AS o
         WHERE o.OrderId = @orderId)
    SELECT c.OrderItemId, c.ServiceInstanceIncrement
      FROM CalculateServiceInstance AS c
     WHERE c.OrderItemId = @orderItemId
);
