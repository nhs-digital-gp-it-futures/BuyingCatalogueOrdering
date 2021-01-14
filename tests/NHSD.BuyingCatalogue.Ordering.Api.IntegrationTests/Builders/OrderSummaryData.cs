using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders
{
    internal sealed class OrderSummaryData
    {
        public OrderSummaryData(
            OrderEntity orderEntity,
            ServiceRecipientEntity serviceRecipientEntity,
            OrderItemEntity catalogueSolutionEntity,
            OrderItemEntity additionalServiceEntity,
            OrderItemEntity associatedServiceEntity)
        {
            OrderEntity = orderEntity;

            ServiceRecipientEntity = serviceRecipientEntity;
            CatalogueSolutionEntity = catalogueSolutionEntity;
            AdditionalServiceEntity = additionalServiceEntity;
            AssociatedServiceEntity = associatedServiceEntity;
        }

        public OrderEntity OrderEntity { get; }

        public OrderItemEntity OrderItemEntities { get; }

        public ServiceRecipientEntity ServiceRecipientEntity { get; set; }

        public OrderItemEntity CatalogueSolutionEntity { get; set; }

        public OrderItemEntity AdditionalServiceEntity { get; set; }

        public OrderItemEntity AssociatedServiceEntity { get; set; }

        public async Task InsertAsync(string connectionString)
        {
            await OrderEntity.InsertAsync(connectionString);

            if (ServiceRecipientEntity != null)
            {
                await ServiceRecipientEntity.InsertAsync(connectionString);
            }

            if (CatalogueSolutionEntity != null)
            {
                await CatalogueSolutionEntity.InsertAsync(connectionString);
            }

            if (AdditionalServiceEntity != null)
            {
                await AdditionalServiceEntity.InsertAsync(connectionString);
            }

            if (AssociatedServiceEntity != null)
            {
                await AssociatedServiceEntity.InsertAsync(connectionString);
            }
        }
    }
}
