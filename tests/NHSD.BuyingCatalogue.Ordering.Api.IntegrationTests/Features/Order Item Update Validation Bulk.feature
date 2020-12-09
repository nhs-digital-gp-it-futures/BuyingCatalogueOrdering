Feature: Order Item Update Validation Bulk
    As a buyer
    I want to only update order items if the validation requirements are correct
    So that I can make sure correct information is stored

Background:
    Given Orders exist
        | OrderId    | Description      | LastUpdatedBy                        | LastUpdatedByName | OrganisationId                       | CommencementDate |
        | C000014-01 | Some Description | 335392e4-4bb1-413b-9de5-36a85c9c0422 | Tom Smith         | 4af62b99-638c-4247-875e-965239cd0c48 | 01/01/2021       |
        | C000014-02 | Some Description | 335392e4-4bb1-413b-9de5-36a85c9c0422 | Tom Smith         | 4af62b99-638c-4247-875e-965239cd0c48 | 01/01/2021       |
        | C000014-03 | Some Description | 335392e4-4bb1-413b-9de5-36a85c9c0422 | Tom Smith         | 4af62b99-638c-4247-875e-965239cd0c48 | 01/01/2021       |
    And Service Recipients exist
        | OdsCode | Name        | OrderId    |
        | ODS1    | Recipient 1 | C000014-01 |
        | ODS2    | Recipient 2 | C000014-01 |
        | ODS3    | Recipient 3 | C000014-01 |
    And Order items exist
        | OrderId    | OdsCode | CatalogueItemName | CatalogueItemType | ProvisioningType |
        | C000014-01 | ODS1    | Item A            | Solution          | OnDemand         |
        | C000014-01 | ODS1    | Item B            | Solution          | OnDemand         |
        | C000014-02 | ODS2    | Item C            | AdditionalService | Declarative      |
        | C000014-02 | ODS2    | Item D            | AdditionalService | Declarative      |
        | C000014-03 | ODS3    | Item E            | AssociatedService | Patient          |
        | C000014-03 | ODS3    | Item F            | AssociatedService | Patient          |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@10516
Scenario: Update catalogue solution order items with a missing required field
    Given the user creates a request to update the order with ID 'C000014-01' with the following items
        | OrderItemId | ItemType           | PayloadType           |
        | 1           | catalogue solution | <item-1-payload-type> |
        | 2           | catalogue solution | <item-2-payload-type> |
    When the user sends the edit order items request
    Then a response with status code 400 is returned
    And the response contains the following error details
        | Id                | Field                |
        | <item-1-error-id> | <item-1-error-field> |
        | <item-2-error-id> | <item-2-error-field> |

    Examples: Request payloads
        | item-1-payload-type               | item-1-error-id                            | item-1-error-field       | item-2-payload-type             | item-2-error-id                   | item-2-error-field       |
        | missing-catalogue-item-type       | CatalogueItemTypeRequired                  | CatalogueItemType        | missing-service-recipient       | ServiceRecipientRequired          | ServiceRecipient         |
        | missing-ods-code                  | OdsCodeRequired                            | ServiceRecipient.OdsCode | too-long-ods-code               | OdsCodeTooLong                    | ServiceRecipient.OdsCode |
        | missing-catalogue-item-id         | CatalogueItemIdRequired                    | CatalogueItemId          | missing-catalogue-item-name     | CatalogueItemNameRequired         | CatalogueItemName        |
        | missing-item-unit                 | ItemUnitRequired                           | ItemUnit                 | missing-time-unit               | TimeUnitRequired                  | TimeUnit                 |
        | missing-item-unit-name            | ItemUnitNameRequired                       | ItemUnit.Name            | missing-time-unit-name          | TimeUnitNameRequired              | TimeUnit.Name            |
        | missing-item-unit-description     | ItemUnitDescriptionRequired                | ItemUnit.Description     | missing-time-unit-description   | TimeUnitDescriptionRequired       | TimeUnit.Description     |
        | missing-provisioning-type         | ProvisioningTypeRequired                   | ProvisioningType         | missing-type                    | TypeRequired                      | Type                     |
        | missing-currency-code             | CurrencyCodeRequired                       | CurrencyCode             | missing-delivery-date           | DeliveryDateRequired              | DeliveryDate             |
        | missing-quantity                  | QuantityRequired                           | Quantity                 | missing-price                   | PriceRequired                     | Price                    |
        | missing-estimation-period         | EstimationPeriodRequiredIfVariableOnDemand | EstimationPeriod         | invalid-value-currency-code     | CurrencyCodeValidValue            | CurrencyCode             |
        | invalid-value-catalogue-item-type | CatalogueItemTypeValidValue                | CatalogueItemType        | invalid-value-type              | TypeValidValue                    | Type                     |
        | invalid-value-provisioning-type   | ProvisioningTypeValidValue                 | ProvisioningType         | invalid-value-estimation-period | EstimationPeriodValidValue        | EstimationPeriod         |
        | too-long-catalogue-item-id        | CatalogueItemIdTooLong                     | CatalogueItemId          | too-long-catalogue-item-name    | CatalogueItemNameTooLong          | CatalogueItemName        |
        | above-delivery-window             | DeliveryDateOutsideDeliveryWindow          | DeliveryDate             | below-delivery-window           | DeliveryDateOutsideDeliveryWindow | DeliveryDate             |
        | less-than-min-quantity            | QuantityGreaterThanZero                    | Quantity                 | greater-than-max-quantity       | QuantityLessThanMax               | Quantity                 |
        | less-than-min-price               | PriceGreaterThanOrEqualToZero              | Price                    | greater-than-max-price          | PriceLessThanMax                  | Price                    |

@10516
Scenario: Update additional service order items with a missing required field
    Given the user creates a request to update the order with ID 'C000014-02' with the following items
        | OrderItemId | ItemType           | PayloadType           |
        | 3           | additional service | <item-1-payload-type> |
        | 4           | additional service | <item-2-payload-type> |
    When the user sends the edit order items request
    Then a response with status code 400 is returned
    And the response contains the following error details
        | Id                | Field                |
        | <item-1-error-id> | <item-1-error-field> |
        | <item-2-error-id> | <item-2-error-field> |

    Examples: Request payloads
        | item-1-payload-type               | item-1-error-id                            | item-1-error-field       | item-2-payload-type             | item-2-error-id             | item-2-error-field   |
        | missing-catalogue-item-type       | CatalogueItemTypeRequired                  | CatalogueItemType        | missing-catalogue-item-name     | CatalogueItemNameRequired   | CatalogueItemName    |
        | missing-ods-code                  | OdsCodeRequired                            | ServiceRecipient.OdsCode | missing-currency-code           | CurrencyCodeRequired        | CurrencyCode         |
        | missing-catalogue-item-id         | CatalogueItemIdRequired                    | CatalogueItemId          | missing-catalogue-solution-id   | CatalogueSolutionIdRequired | CatalogueSolutionId  |
        | missing-item-unit                 | ItemUnitRequired                           | ItemUnit                 | missing-time-unit               | TimeUnitRequired            | TimeUnit             |
        | missing-item-unit-name            | ItemUnitNameRequired                       | ItemUnit.Name            | missing-time-unit-name          | TimeUnitNameRequired        | TimeUnit.Name        |
        | missing-item-unit-description     | ItemUnitDescriptionRequired                | ItemUnit.Description     | missing-time-unit-description   | TimeUnitDescriptionRequired | TimeUnit.Description |
        | missing-provisioning-type         | ProvisioningTypeRequired                   | ProvisioningType         | missing-type                    | TypeRequired                | Type                 |
        | missing-quantity                  | QuantityRequired                           | Quantity                 | missing-price                   | PriceRequired               | Price                |
        | missing-estimation-period         | EstimationPeriodRequiredIfVariableOnDemand | EstimationPeriod         | invalid-value-currency-code     | CurrencyCodeValidValue      | CurrencyCode         |
        | invalid-value-catalogue-item-type | CatalogueItemTypeValidValue                | CatalogueItemType        | invalid-value-provisioning-type | ProvisioningTypeValidValue  | ProvisioningType     |
        | invalid-value-type                | TypeValidValue                             | Type                     | invalid-value-estimation-period | EstimationPeriodValidValue  | EstimationPeriod     |
        | too-long-ods-code                 | OdsCodeTooLong                             | ServiceRecipient.OdsCode | too-long-catalogue-solution-id  | CatalogueSolutionIdTooLong  | CatalogueSolutionId  |
        | too-long-catalogue-item-id        | CatalogueItemIdTooLong                     | CatalogueItemId          | too-long-catalogue-item-name    | CatalogueItemNameTooLong    | CatalogueItemName    |
        | less-than-min-quantity            | QuantityGreaterThanZero                    | Quantity                 | greater-than-max-quantity       | QuantityLessThanMax         | Quantity             |
        | less-than-min-price               | PriceGreaterThanOrEqualToZero              | Price                    | greater-than-max-price          | PriceLessThanMax            | Price                |

@10516
Scenario: Update associated service order items with a missing required field
    Given the user creates a request to update the order with ID 'C000014-03' with the following items
        | OrderItemId | ItemType           | PayloadType           |
        | 5           | associated service | <item-1-payload-type> |
        | 6           | associated service | <item-2-payload-type> |
    When the user sends the edit order items request
    Then a response with status code 400 is returned
    And the response contains the following error details
        | Id                | Field                |
        | <item-1-error-id> | <item-1-error-field> |
        | <item-2-error-id> | <item-2-error-field> |

    Examples: Request payloads
        | item-1-payload-type           | item-1-error-id               | item-1-error-field   | item-2-payload-type               | item-2-error-id                            | item-2-error-field |
        | missing-catalogue-item-type   | CatalogueItemTypeRequired     | CatalogueItemType    | missing-catalogue-item-type       | CatalogueItemTypeRequired                  | CatalogueItemType  |
        | missing-catalogue-item-id     | CatalogueItemIdRequired       | CatalogueItemId      | missing-catalogue-item-name       | CatalogueItemNameRequired                  | CatalogueItemName  |
        | missing-item-unit             | ItemUnitRequired              | ItemUnit             | missing-item-unit-name            | ItemUnitNameRequired                       | ItemUnit.Name      |
        | missing-item-unit-description | ItemUnitDescriptionRequired   | ItemUnit.Description | missing-provisioning-type         | ProvisioningTypeRequired                   | ProvisioningType   |
        | missing-type                  | TypeRequired                  | Type                 | invalid-value-catalogue-item-type | CatalogueItemTypeValidValue                | CatalogueItemType  |
        | missing-currency-code         | CurrencyCodeRequired          | CurrencyCode         | missing-estimation-period         | EstimationPeriodRequiredIfVariableOnDemand | EstimationPeriod   |
        | missing-quantity              | QuantityRequired              | Quantity             | missing-price                     | PriceRequired                              | Price              |
        | invalid-value-currency-code   | CurrencyCodeValidValue        | CurrencyCode         | invalid-value-estimation-period   | EstimationPeriodValidValue                 | EstimationPeriod   |
        | invalid-value-type            | TypeValidValue                | Type                 | invalid-value-provisioning-type   | ProvisioningTypeValidValue                 | ProvisioningType   |
        | too-long-catalogue-item-id    | CatalogueItemIdTooLong        | CatalogueItemId      | too-long-catalogue-item-name      | CatalogueItemNameTooLong                   | CatalogueItemName  |
        | less-than-min-quantity        | QuantityGreaterThanZero       | Quantity             | greater-than-max-quantity         | QuantityLessThanMax                        | Quantity           |
        | less-than-min-price           | PriceGreaterThanOrEqualToZero | Price                | greater-than-max-price            | PriceLessThanMax                           | Price              |

@10516
Scenario: Update catalogue solution order items with a duplicate order item ID
    Given the user creates a request to update the order with ID 'C000014-01' with the following items
        | OrderItemId | ItemType           | PayloadType |
        | 1           | catalogue solution | complete    |
        | 1           | catalogue solution | complete    |
    When the user sends the edit order items request
    Then a response with status code 400 is returned
    And the response contains the following error details
        | Id                   | Field       |
        |                      |             |
        | OrderItemIdDuplicate | OrderItemId |

@10516
Scenario: Update catalogue solution order items with a invalid order item IDs
    Given the user creates a request to update the order with ID 'C000014-01' with the following items
        | OrderItemId | ItemType           | PayloadType |
        | 3           | catalogue solution | complete    |
        | 4           | catalogue solution | complete    |
    When the user sends the edit order items request
    Then a response with status code 400 is returned
    And the response contains the following error details
        | Id                   | Field       |
        | OrderItemIdNotFound  | OrderItemId |
        | OrderItemIdNotFound  | OrderItemId |
