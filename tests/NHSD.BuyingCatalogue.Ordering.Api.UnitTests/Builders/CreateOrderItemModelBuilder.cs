using System;
using NHSD.BuyingCatalogue.Ordering.Api.Models;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders
{
    internal sealed class CreateOrderItemModelBuilder
    {
        private readonly ServiceRecipientModel _serviceRecipient;
        private readonly string _catalogueSolutionId;
        private readonly string _catalogueSolutionName;
        private readonly DateTime? _deliveryDate;
        private readonly int _quantity;
        private readonly string _estimationPeriod;
        private readonly string _provisioningType;
        private readonly string _type;
        private readonly string _currencyCode;
        private readonly ItemUnitModel _itemUnitModel;
        private readonly decimal? _price;

        private CreateOrderItemModelBuilder()
        {
            _serviceRecipient = new ServiceRecipientModel
            {
                OdsCode = "AB1234"
            };
            _catalogueSolutionId = "10000-001";
            _catalogueSolutionName = "Some solution name";
            _deliveryDate = DateTime.UtcNow;
            _quantity = 123;
            _estimationPeriod = "month";
            _provisioningType = "Patient";
            _type = "Flat";
            _currencyCode = "EUR";
            _itemUnitModel = new ItemUnitModel
            {
                Name = "patients",
                Description = "per patients"
            };
            _price = 25.1m;
        }

        public static CreateOrderItemModelBuilder Create() => new CreateOrderItemModelBuilder();

        public CreateOrderItemModel Build() =>
            new CreateOrderItemModel
            {
                ServiceRecipient = _serviceRecipient,
                CatalogueSolutionId = _catalogueSolutionId,
                CatalogueSolutionName = _catalogueSolutionName,
                DeliveryDate = _deliveryDate,
                Quantity = _quantity,
                EstimationPeriod = _estimationPeriod,
                ProvisioningType = _provisioningType,
                Type = _type,
                CurrencyCode = _currencyCode,
                ItemUnitModel = _itemUnitModel,
                Price = _price,
            };
    }
}
