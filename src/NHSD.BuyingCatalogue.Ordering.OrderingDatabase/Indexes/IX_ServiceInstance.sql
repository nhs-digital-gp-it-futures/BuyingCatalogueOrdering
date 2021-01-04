CREATE NONCLUSTERED INDEX IX_ServiceInstance ON dbo.OrderItem
(
    OrderId,
    OdsCode,
    CatalogueItemId,
    ParentCatalogueItemId
)
INCLUDE (CatalogueItemTypeId)
WHERE CatalogueItemTypeId IN (1, 2);
