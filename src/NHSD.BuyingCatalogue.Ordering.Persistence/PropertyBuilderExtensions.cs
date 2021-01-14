using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NHSD.BuyingCatalogue.Ordering.Persistence
{
    internal static class PropertyBuilderExtensions
    {
        internal static PropertyBuilder<TEntity> HasCamelCaseBackingField<TEntity>(
            this PropertyBuilder<TEntity> builder,
            string propertyName)
        {
            var fieldName = char.ToLowerInvariant(propertyName[0]) + propertyName[1..];

            return builder.HasField(fieldName);
        }
    }
}
