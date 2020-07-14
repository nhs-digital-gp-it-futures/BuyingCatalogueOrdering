using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NHSD.BuyingCatalogue.Ordering.Api.Models;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Api.Attributes
{
    public sealed class CreateOrderItemModelConverter : JsonConverter<CreateOrderItemBaseModel>
    {
        public override CreateOrderItemBaseModel Read(
            ref Utf8JsonReader reader, 
            Type typeToConvert, 
            JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var model = JsonSerializer.Deserialize<CreateOrderItemModel>(doc.RootElement.GetRawText(), options);
            if (model.CatalogueItemType == CatalogueItemType.Solution.Name)
            {
                return (CreateOrderItemBaseModel)JsonSerializer.Deserialize(doc.RootElement.GetRawText(), typeof(CreateOrderItemSolutionModel), options);
            }
            
            if (model.CatalogueItemType == CatalogueItemType.AdditionalService.Name)
            {
                return (CreateOrderItemBaseModel)JsonSerializer.Deserialize(doc.RootElement.GetRawText(), typeof(CreateOrderItemAdditionalServiceModel), options);
            }

            return model;
        }

        public override void Write(Utf8JsonWriter writer, CreateOrderItemBaseModel value, JsonSerializerOptions options)
        {
            throw new InvalidOperationException("Use default serialization.");
        }
    }
}
