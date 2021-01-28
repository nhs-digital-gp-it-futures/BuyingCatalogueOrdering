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

        public ServiceRecipientEntity ServiceRecipientEntity { get; set; }

        public OrderItemEntity CatalogueSolutionEntity { get; set; }

        public OrderItemEntity AdditionalServiceEntity { get; set; }

        public OrderItemEntity AssociatedServiceEntity { get; set; }

        public async Task InsertAsync(string connectionString)
        {
            await OrderEntity.InsertAsync(connectionString);

            if (ServiceRecipientEntity is not null)
            {
                await ServiceRecipientEntity.InsertAsync(connectionString);
            }

            if (CatalogueSolutionEntity is not null)
            {
                await CatalogueSolutionEntity.InsertAsync(connectionString);
            }

            if (AdditionalServiceEntity is not null)
            {
                await AdditionalServiceEntity.InsertAsync(connectionString);
            }

            if (AssociatedServiceEntity is not null)
            {
                await AssociatedServiceEntity.InsertAsync(connectionString);
            }
        }
    }
}
