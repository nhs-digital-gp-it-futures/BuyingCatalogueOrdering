using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Common.UnitTests
{
    public static class OrderExtensions
    {
        public static Order RemoveOrderItems(this Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var catalogueIds = order.OrderItems.Select(x => x.CatalogueItem.Id).ToList();

            catalogueIds.ForEach(ci => order.DeleteOrderItemAndUpdateProgress(ci));

            return order;
        }
    }
}
