using System;

namespace NHSD.BuyingCatalouge.Ordering.Api.Testing.Data.Entities
{
    public sealed class OrderEntity : EntityBase
    {
        public string OrderId { get; set; }

        public string Description { get; set; }

        public Guid OrganisationId { get; set; }

        public int OrderStatusId { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        public Guid LastUpdatedBy { get; set; }

        protected override string InsertSql => $@"
                                INSERT INTO [dbo].[Order]
                                (OrderId,
                                 Description,
                                 OrganisationId,
                                 OrderStatusId,
                                 Created,
                                 LastUpdated,
                                 LastUpdatedBy
                                 )
                                VALUES
                                (@OrderId,
                                 @Description,
                                 @OrganisationId,
                                 @OrderStatusId,
                                 @Created,
                                 @LastUpdated,
                                 @LastUpdatedBy)";
    }
}
