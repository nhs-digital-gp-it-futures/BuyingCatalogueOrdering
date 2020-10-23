using System;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using EnumsNET;
using NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Api.Services.UpdateOrderItem;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    internal sealed class EnumValueByNameCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<CreateOrderItemRequest>(c => new EnumValueByNameSpecimenBuilder<CataloguePriceType>());
            fixture.Customize<CreateOrderItemRequest>(c => new EnumValueByNameSpecimenBuilder<ProvisioningType>());

            var estimationPeriodNameBuilder = new EnumValueByNameSpecimenBuilder<TimeUnit>("estimationPeriodName", e => e.AsString(EnumFormat.DisplayName));

            fixture.Customize<CreateOrderItemRequest>(c => estimationPeriodNameBuilder);
            fixture.Customize<UpdateOrderItemRequest>(c => estimationPeriodNameBuilder);
        }

        private class EnumValueByNameSpecimenBuilder<T> : ISpecimenBuilder
            where T : struct, Enum
        {
            private readonly Func<T, string> nameFunc;
            private readonly string paramName;

            internal EnumValueByNameSpecimenBuilder()
                : this(GetDefaultPropertyName())
            {
            }

            internal EnumValueByNameSpecimenBuilder(string paramName, Func<T, string> nameFunc)
            {
                this.paramName = paramName;
                this.nameFunc = nameFunc;
            }

            private EnumValueByNameSpecimenBuilder(string paramName)
                : this(paramName, e => e.ToString())
            {
            }

            public object Create(object request, ISpecimenContext context)
            {
                if (!(request is ParameterInfo pi))
                    return new NoSpecimen();

                if (IsEnumParameter(pi))
                    return nameFunc(context.Create<T>());

                return new NoSpecimen();
            }

            private static string GetDefaultPropertyName()
            {
                var name = typeof(T).Name;

                return char.ToLowerInvariant(name[0]) + name.Substring(1) + "Name";
            }

            private bool IsEnumParameter(ParameterInfo info)
            {
                return info.ParameterType == typeof(string) && info.Name == paramName;
            }
        }
    }
}
