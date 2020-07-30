using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders
{
    internal sealed class OrderSummaryData
    {
        public OrderEntity OrderEntity { get; }
        public List<OrderItemEntity> OrderItemEntities { get; }
        public List<ServiceRecipientEntity> ServiceRecipientEntities { get; set; }

        public OrderSummaryData(OrderEntity orderEntity, List<OrderItemEntity> orderItemEntities, List<ServiceRecipientEntity> serviceRecipientEntities)
        {
            OrderEntity = orderEntity;
            OrderItemEntities = orderItemEntities;
            ServiceRecipientEntities = serviceRecipientEntities;
        }

        public async Task InsertAsync(string connectionString)
        {
            await OrderEntity.InsertAsync(connectionString);

            foreach (var recipients in ServiceRecipientEntities)
            {
                await recipients.InsertAsync(connectionString);
            }

            foreach (var orderItem in OrderItemEntities)
            {
                await orderItem.InsertAsync(connectionString);
            }
        }
    }
}
