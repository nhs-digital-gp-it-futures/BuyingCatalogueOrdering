using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Builders
{
    public class OrderSummaryData
    {
        public List<OrderEntity> OrderEntities { get; }
        public List<OrderItemEntity> OrderItemEntities { get; }
        public List<ServiceRecipientEntity> ServiceRecipientEntities { get; set; }

        public OrderSummaryData(List<OrderEntity> orderEntities, List<OrderItemEntity> orderItemEntities, List<ServiceRecipientEntity> serviceRecipientEntities)
        {
            OrderEntities = orderEntities;
            OrderItemEntities = orderItemEntities;
            ServiceRecipientEntities = serviceRecipientEntities;
        }

        public async Task InsertAsync(string connectionString)
        {
            foreach (var order in OrderEntities)
            {
                await order.InsertAsync(connectionString);
            }

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
