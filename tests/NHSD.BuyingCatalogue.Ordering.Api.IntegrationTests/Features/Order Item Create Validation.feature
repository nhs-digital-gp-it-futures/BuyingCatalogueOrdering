Feature: Create Order Item Validation
    As a Buyer
    I want to only create an order item if the validation requirements are correct
    So that I can make sure correct information is stored

Background:
    Given Orders exist
        | OrderId    | Description      | OrganisationId                       | LastUpdatedByName | LastUpdatedBy                        | CommencementDate |
        | C000014-01 | Some Description | 4af62b99-638c-4247-875e-965239cd0c48 | Tom Smith         | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 01/01/2021       |
    And Service Recipients exist
        | OdsCode | Name              | OrderId    |
        | ODS1    | Service Recipient | C000014-01 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@7840
Scenario: 1. Create catalogue solution order item with missing required field
    Given the user creates a request to add a new catalogue solution order item to the order with ID 'C000014-01'
    And the user enters the '<payload-type>' create order item request payload
    When the user sends the create order item request
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id         | field         |
        | <error-id> | <error-field> |

    Examples: Request payloads
        | payload-type                      | error-id                                   | error-field       |
        | missing-catalogue-item-type       | CatalogueItemTypeRequired                  | CatalogueItemType |
        | missing-service-recipient         | ServiceRecipientRequired                   | ServiceRecipient  |
        | missing-ods-code                  | OdsCodeRequired                            | OdsCode           |
        | missing-catalogue-item-id         | CatalogueItemIdRequired                    | CatalogueItemId   |
        | missing-catalogue-item-name       | CatalogueItemNameRequired                  | CatalogueItemName |
        | missing-item-unit                 | ItemUnitRequired                           | ItemUnit          |
        | missing-item-unit-name            | ItemUnitNameRequired                       | Name              |
        | missing-item-unit-description     | ItemUnitDescriptionRequired                | Description       |
        | missing-time-unit                 | TimeUnitRequired                           | TimeUnit          |
        | missing-time-unit-name            | TimeUnitNameRequired                       | Name              |
        | missing-time-unit-description     | TimeUnitDescriptionRequired                | Description       |
        | missing-provisioning-type         | ProvisioningTypeRequired                   | ProvisioningType  |
        | missing-type                      | TypeRequired                               | Type              |
        | missing-currency-code             | CurrencyCodeRequired                       | CurrencyCode      |
        | missing-delivery-date             | DeliveryDateRequired                       | DeliveryDate      |
        | missing-quantity                  | QuantityRequired                           | Quantity          |
        | missing-estimation-period         | EstimationPeriodRequiredIfVariableOnDemand | EstimationPeriod  |
        | missing-price                     | PriceRequired                              | Price             |
        | invalid-value-catalogue-item-type | CatalogueItemTypeValidValue                | CatalogueItemType |
        | invalid-value-currency-code       | CurrencyCodeValidValue                     | CurrencyCode      |
        | invalid-value-type                | TypeValidValue                             | Type              |
        | invalid-value-provisioning-type   | ProvisioningTypeValidValue                 | ProvisioningType  |
        | invalid-value-estimation-period   | EstimationPeriodValidValue                 | EstimationPeriod  |
        | too-long-ods-code                 | OdsCodeTooLong                             | OdsCode           |
        | too-long-catalogue-item-id        | CatalogueItemIdTooLong                     | CatalogueItemId   |
        | too-long-catalogue-item-name      | CatalogueItemNameTooLong                   | CatalogueItemName |
        | above-delivery-window             | DeliveryDateOutsideDeliveryWindow          | DeliveryDate      |
        | below-delivery-window             | DeliveryDateOutsideDeliveryWindow          | DeliveryDate      |
        | less-than-min-quantity            | QuantityGreaterThanZero                    | Quantity          |
        | greater-than-max-quantity         | QuantityLessThanMax                        | Quantity          |
        | less-than-min-price               | PriceGreaterThanOrEqualToZero              | Price             |
        | greater-than-max-price            | PriceLessThanMax                           | Price             |

@7840
Scenario: 2. Create additional service order item with missing required field
    Given the user creates a request to add a new additional service order item to the order with ID 'C000014-01'
    And the user enters the '<payload-type>' create order item request payload
    When the user sends the create order item request
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id         | field         |
        | <error-id> | <error-field> |

    Examples: Request payloads
        | payload-type                      | error-id                                   | error-field         |
        | missing-catalogue-item-type       | CatalogueItemTypeRequired                  | CatalogueItemType   |
        | missing-service-recipient         | ServiceRecipientRequired                   | ServiceRecipient    |
        | missing-ods-code                  | OdsCodeRequired                            | OdsCode             |
        | missing-catalogue-item-id         | CatalogueItemIdRequired                    | CatalogueItemId     |
        | missing-catalogue-item-name       | CatalogueItemNameRequired                  | CatalogueItemName   |
        | missing-catalogue-solution-id     | CatalogueSolutionIdRequired                | CatalogueSolutionId |
        | missing-item-unit                 | ItemUnitRequired                           | ItemUnit            |
        | missing-item-unit-name            | ItemUnitNameRequired                       | Name                |
        | missing-item-unit-description     | ItemUnitDescriptionRequired                | Description         |
        | missing-time-unit                 | TimeUnitRequired                           | TimeUnit            |
        | missing-time-unit-name            | TimeUnitNameRequired                       | Name                |
        | missing-time-unit-description     | TimeUnitDescriptionRequired                | Description         |
        | missing-provisioning-type         | ProvisioningTypeRequired                   | ProvisioningType    |
        | missing-type                      | TypeRequired                               | Type                |
        | missing-currency-code             | CurrencyCodeRequired                       | CurrencyCode        |
        | missing-quantity                  | QuantityRequired                           | Quantity            |
        | missing-estimation-period         | EstimationPeriodRequiredIfVariableOnDemand | EstimationPeriod    |
        | missing-price                     | PriceRequired                              | Price               |
        | invalid-value-catalogue-item-type | CatalogueItemTypeValidValue                | CatalogueItemType   |
        | invalid-value-currency-code       | CurrencyCodeValidValue                     | CurrencyCode        |
        | invalid-value-type                | TypeValidValue                             | Type                |
        | invalid-value-provisioning-type   | ProvisioningTypeValidValue                 | ProvisioningType    |
        | invalid-value-estimation-period   | EstimationPeriodValidValue                 | EstimationPeriod    |
        | too-long-ods-code                 | OdsCodeTooLong                             | OdsCode             |
        | too-long-catalogue-item-id        | CatalogueItemIdTooLong                     | CatalogueItemId     |
        | too-long-catalogue-solution-id    | CatalogueSolutionIdTooLong                 | CatalogueSolutionId |
        | too-long-catalogue-item-name      | CatalogueItemNameTooLong                   | CatalogueItemName   |
        | less-than-min-quantity            | QuantityGreaterThanZero                    | Quantity            |
        | greater-than-max-quantity         | QuantityLessThanMax                        | Quantity            |
        | less-than-min-price               | PriceGreaterThanOrEqualToZero              | Price               |
        | greater-than-max-price            | PriceLessThanMax                           | Price               |
