using System;
namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class OrderModel
    {
        public string OrderId { get; set; }

        public string OrderDescription { get; set; }

        public Guid OrganisationId { get; set; }

        public string Status { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public Guid LastUpdatedBy { get; set; }
    }
}
