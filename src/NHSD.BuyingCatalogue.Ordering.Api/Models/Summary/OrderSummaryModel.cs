using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models.Summary
{
    public sealed class OrderSummaryModel
    {
        public string OrderId { get; set; }

        public Guid OrganisationId { get; set; }

        public string Description { get; set; }

        public string SectionStatus { get; set; }

        public IEnumerable<SectionModel> Sections { get; set; }

        public string Status { get; set; }

        public static OrderSummaryModel Create(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            int serviceRecipientsCount = order.OrderItems.Where(o => o.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution).SelectMany(o => o.OrderItemRecipients).Count();
            int catalogueSolutionsCount = order.OrderItems.Count(i => i.CatalogueItem.CatalogueItemType.Equals(CatalogueItemType.Solution));
            int associatedServicesCount = order.OrderItems.Count(i => i.CatalogueItem.CatalogueItemType.Equals(CatalogueItemType.AssociatedService));
            int additionalServicesCount = order.OrderItems.Count(i => i.CatalogueItem.CatalogueItemType.Equals(CatalogueItemType.AdditionalService));

            return new OrderSummaryModel
            {
                OrderId = order.CallOffId.ToString(),
                OrganisationId = order.OrderingParty?.Id ?? Guid.Empty,
                Description = order.Description,
                Sections = new List<SectionModel>
                {
                    SectionModel.Description,
                    SectionModel.OrderingParty.WithStatus(order.IsOrderingPartySectionComplete() ? "complete" : "incomplete"),
                    SectionModel.Supplier.WithStatus(order.IsSupplierSectionComplete() ? "complete" : "incomplete"),
                    SectionModel.CommencementDate.WithStatus(order.IsCommencementDateSectionComplete() ? "complete" : "incomplete"),
                    SectionModel.AssociatedServices
                        .WithStatus(order.IsAssociatedServicesSectionComplete() ? "complete" : "incomplete")
                        .WithCount(associatedServicesCount),
                    SectionModel
                        .ServiceRecipients
                        .WithStatus(order.IsServiceRecipientsSectionComplete() ? "complete" : "incomplete")
                        .WithCount(serviceRecipientsCount),
                    SectionModel.CatalogueSolutions
                        .WithStatus(order.IsCatalogueSolutionsSectionComplete() ? "complete" : "incomplete")
                        .WithCount(catalogueSolutionsCount),
                    SectionModel.AdditionalServices
                        .WithStatus(order.IsAdditionalServicesSectionComplete() ? "complete" : "incomplete")
                        .WithCount(additionalServicesCount),
                    SectionModel.FundingSource.WithStatus(order.IsFundingSourceComplete() ? "complete" : "incomplete"),
                },
                SectionStatus = order.IsSectionStatusComplete() ? "complete" : "incomplete",
                Status = order.OrderStatus.Name,
            };
        }
    }
}
