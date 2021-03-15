CREATE FUNCTION dbo.HasParentSolution
(
    @orderId int,
    @catalogueItemId nvarchar(14)
)
RETURNS TABLE
AS
RETURN
(
    SELECT o1.OrderId, o1.CatalogueItemId,
           CASE WHEN EXISTS (
                SELECT *
                  FROM dbo.OrderItemRecipients AS o2
                       INNER JOIN dbo.CatalogueItem AS c2 ON c2.Id = o2.CatalogueItemId
                 WHERE o2.OrderId = o1.OrderId
                   AND o2.OdsCode = o1.OdsCode
                   AND c2.Id = c1.ParentCatalogueItemId
                   AND c2.CatalogueItemTypeId IN (1, 2))
                THEN 1
                ELSE 0 END AS HasParentSolution
      FROM dbo.OrderItemRecipients AS o1
           INNER JOIN dbo.CatalogueItem AS c1 ON c1.Id = o1.CatalogueItemId
     WHERE o1.OrderId = @orderId
       AND o1.CatalogueItemId = @catalogueItemId
);
