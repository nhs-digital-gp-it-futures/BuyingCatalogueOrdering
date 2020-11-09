using System;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using EnumsNET;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    internal sealed class EnumValueByNameCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<CreateOrderItemModel>(c => new EnumValueByPropertyNameSpecimenBuilder<CatalogueItemType>());
            fixture.Customize<CreateOrderItemModel>(c => new EnumValueByPropertyNameSpecimenBuilder<CataloguePriceType>(nameof(CreateOrderItemModel.Type)));
            fixture.Customize<CreateOrderItemModel>(c => new EnumValueByPropertyNameSpecimenBuilder<ProvisioningType>());
            fixture.Customize<CreateOrderItemModel>(c => new EnumValueByPropertyNameSpecimenBuilder<TimeUnit>(
                nameof(CreateOrderItemModel.EstimationPeriod),
                e => e.AsString(EnumFormat.DisplayName)));

            fixture.Customize<CreateOrderItemRequest>(c => new EnumValueByParameterNameSpecimenBuilder<CatalogueItemType>());
            fixture.Customize<CreateOrderItemRequest>(c => new EnumValueByParameterNameSpecimenBuilder<CataloguePriceType>());
            fixture.Customize<CreateOrderItemRequest>(c => new EnumValueByParameterNameSpecimenBuilder<ProvisioningType>());

            var estimationPeriodNameBuilder = new EnumValueByParameterNameSpecimenBuilder<TimeUnit>("estimationPeriodName", e => e.AsString(EnumFormat.DisplayName));

            fixture.Customize<CreateOrderItemRequest>(c => estimationPeriodNameBuilder);
            fixture.Customize<UpdateOrderItemRequest>(c => estimationPeriodNameBuilder);
        }

        private abstract class EnumValueByNameSpecimenBuilder<TEnum, TInfo> : ISpecimenBuilder
            where TEnum : struct, Enum
        {
            private readonly Func<TEnum, string> getEnumName;
            private readonly Func<TInfo, (string Name, Type Type)> getItemDetails;
            private readonly string itemName;

            protected EnumValueByNameSpecimenBuilder(
                Func<TEnum, string> getEnumName,
                Func<TInfo, (string Name, Type Type)> getItemDetails,
                string itemName)
            {
                this.getEnumName = getEnumName;
                this.getItemDetails = getItemDetails;
                this.itemName = itemName;
            }

            public object Create(object request, ISpecimenContext context)
            {
                if (!(request is TInfo info))
                    return new NoSpecimen();

                if (IsItem(info))
                    return getEnumName(context.Create<TEnum>());

                return new NoSpecimen();
            }

            private bool IsItem(TInfo info)
            {
                (string name, Type type) = getItemDetails(info);

                return name == itemName && type == typeof(string);
            }
        }

        private class EnumValueByPropertyNameSpecimenBuilder<TEnum> : EnumValueByNameSpecimenBuilder<TEnum, PropertyInfo>
            where TEnum : struct, Enum
        {
            internal EnumValueByPropertyNameSpecimenBuilder()
                : this(typeof(TEnum).Name)
            {
            }

            internal EnumValueByPropertyNameSpecimenBuilder(string propertyName)
                : this(propertyName, e => e.ToString())
            {
            }

            internal EnumValueByPropertyNameSpecimenBuilder(string propertyName, Func<TEnum, string> getEnumName)
                : base(getEnumName, p => (p.Name, p.PropertyType), propertyName)
            {
            }
        }

        private class EnumValueByParameterNameSpecimenBuilder<TEnum> : EnumValueByNameSpecimenBuilder<TEnum, ParameterInfo>
            where TEnum : struct, Enum
        {
            internal EnumValueByParameterNameSpecimenBuilder()
                : this(GetDefaultParameterName(), e => e.ToString())
            {
            }

            internal EnumValueByParameterNameSpecimenBuilder(string paramName, Func<TEnum, string> getEnumName)
                : base(getEnumName, p => (p.Name, p.ParameterType), paramName)
            {
            }

            private static string GetDefaultParameterName()
            {
                var name = typeof(TEnum).Name;

                return char.ToLowerInvariant(name[0]) + name.Substring(1) + "Name";
            }
        }
    }
}
