﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Data
{
    public sealed class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasKey(orderItem => orderItem.OrderItemId);

            builder.Property(orderItem => orderItem.OrderItemId)
                .HasColumnName(nameof(OrderItem.OrderItemId))
                .IsRequired();

            builder.Property("OrderId")
                .HasColumnName("OrderId")
                .IsRequired();

            builder.Property(orderItem => orderItem.CatalogueItemId)
                .HasColumnName(nameof(OrderItem.CatalogueItemId))
                .IsRequired();

            builder
                .Property(orderItem => orderItem.CatalogueItemType)
                .HasConversion<int>()
                .HasColumnName("CatalogueItemTypeId");

            builder.Property(orderItem => orderItem.CatalogueItemName)
                .HasColumnName(nameof(OrderItem.CatalogueItemName))
                .IsRequired();

            builder.Property(orderItem => orderItem.ParentCatalogueItemId)
                .HasColumnName(nameof(OrderItem.ParentCatalogueItemId));

            builder.Property(orderItem => orderItem.OdsCode)
                .HasColumnName(nameof(OrderItem.OdsCode));

            builder
                .Property(orderItem => orderItem.ProvisioningType)
                .HasConversion<int>()
                .HasColumnName("ProvisioningTypeId");

            builder
                .Property(orderItem => orderItem.CataloguePriceType)
                .HasConversion<int>()
                .HasColumnName("CataloguePriceTypeId");

            builder.OwnsOne(orderItem => orderItem.CataloguePriceUnit, navigationBuilder =>
            {
                navigationBuilder.Property(cataloguePriceUnit => cataloguePriceUnit.Name)
                    .HasColumnName("PricingUnitName");

                navigationBuilder.Property(cataloguePriceUnit => cataloguePriceUnit.Description)
                    .HasColumnName("PricingUnitDescription");
            });

            builder
                .Property(orderItem => orderItem.PriceTimeUnit)
                .HasConversion<int>()
                .HasColumnName("TimeUnitId");

            builder.Property(orderItem => orderItem.CurrencyCode)
                .HasColumnName(nameof(OrderItem.CurrencyCode))
                .IsRequired();

            builder.Property(orderItem => orderItem.Quantity)
                .HasColumnName(nameof(OrderItem.Quantity))
                .IsRequired();

            builder
                .Property(orderItem => orderItem.EstimationPeriod)
                .HasConversion<int>()
                .HasColumnName("EstimationPeriodId");

            builder.Property(orderItem => orderItem.DeliveryDate)
                .HasColumnName(nameof(OrderItem.DeliveryDate));

            builder.Property(orderItem => orderItem.Price)
                .HasColumnName(nameof(OrderItem.Price))
                .HasColumnType("decimal(18, 3)");

            builder.Property(orderItem => orderItem.Created)
                .HasColumnName(nameof(OrderItem.Created))
                .IsRequired();

            builder.Property(orderItem => orderItem.LastUpdated)
                .HasColumnName(nameof(OrderItem.LastUpdated))
                .IsRequired();
        }
    }
}
