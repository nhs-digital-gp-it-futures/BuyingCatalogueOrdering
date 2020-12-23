CREATE FUNCTION dbo.HasParentSolution
(
    @orderId nvarchar(10),
    @orderItemId int
)
RETURNS TABLE
AS
RETURN
(
    SELECT o1.OrderItemId,
           CASE WHEN EXISTS (
                SELECT OrderItemId
                  FROM dbo.OrderItem AS o2
                 WHERE o2.OrderId = o1.OrderId
                   AND o2.OdsCode = o1.OdsCode
                   AND o2.CatalogueItemId = o1.ParentCatalogueItemId
                   AND o2.CatalogueItemTypeId IN (1, 2))
                THEN 1
                ELSE 0 END AS HasParentSolution
      FROM dbo.OrderItem AS o1
     WHERE o1.OrderId = @orderId
       AND o1.OrderItemId = @orderItemId
);
