Feature: order item PUT validation
    As a buyer
    I want to only create order items if the validation requirements are correct
    So that I can make sure correct information is stored

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | Description      | OrderingPartyId                      | LastUpdatedByName | LastUpdatedBy                        | CommencementDate |
        | 10001   | Some Description | 4af62b99-638c-4247-875e-965239cd0c48 | Tom Smith         | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 01/01/2021       |
    And order progress exists
        | OrderId | AdditionalServicesViewd | AssociatedServicesViewed | CatalogueSolutionsViewed |
        | 10001   | false                   | false                    | false                    |
    And pricing units exist
        | Name    | Description |
        | patient | per patient |
    And catalogue items exist
        | Id       | Name       | CatalogueItemType | ParentCatalogueItemId |
        | 1000-001 | Sol 1      | Solution          | NULL                  |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@6306
Scenario: create catalogue solution order item with a missing or invalid field
    Given the user creates a request to add the catalogue item with ID 1000-002 and the following details to the order with ID 10001
        | CatalogueItemName   | CatalogueItemType   | CatalogueSolutionId   | CurrencyCode   | EstimationPeriod   | ItemUnitName   | ItemUnitDescription   | Price   | ProvisioningType   | TimeUnitName   | TimeUnitDescription   | Type   |
        | <CatalogueItemName> | <CatalogueItemType> | <CatalogueSolutionId> | <CurrencyCode> | <EstimationPeriod> | <ItemUnitName> | <ItemUnitDescription> | <Price> | <ProvisioningType> | <TimeUnitName> | <TimeUnitDescription> | <Type> |
    And for the following recipients
        | OdsCode | Name        | Quantity | DeliveryDate |
        | ODS1    | Recipient 1 | 1        | 12/03/2021   |
    When the user sends the create order items request
    Then a response with status code 400 is returned
    And the response contains the following error details
        | Id        | Field        |
        | <ErrorId> | <ErrorField> |

    Examples: request payloads
        | CatalogueItemName        | CatalogueItemType | CatalogueSolutionId | CurrencyCode | EstimationPeriod | ItemUnitName            | ItemUnitDescription      | Price            | ProvisioningType | TimeUnitName            | TimeUnitDescription     | Type    | ErrorId                                    | ErrorField           |
        | NULL                     | Solution          | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | flat    | CatalogueItemNameRequired                  | CatalogueItemName    |
        | #A string of length 257# | Solution          | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | flat    | CatalogueItemNameTooLong                   | CatalogueItemName    |
        | Item A                   | NULL              | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | flat    | CatalogueItemTypeRequired                  | CatalogueItemType    |
        | Item A                   | Invalid           | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | flat    | CatalogueItemTypeValidValue                | CatalogueItemType    |
        | Item A                   | Solution          | NULL                | NULL         | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | flat    | CurrencyCodeRequired                       | CurrencyCode         |
        | Item A                   | Solution          | NULL                | ABC          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | flat    | CurrencyCodeValidValue                     | CurrencyCode         |
        | Item A                   | Solution          | NULL                | GBP          | NULL             | consultation            | per consultation         | 10.50            | OnDemand         | month                   | per month               | flat    | EstimationPeriodRequiredIfVariableOnDemand | EstimationPeriod     |
        | Item A                   | Solution          | NULL                | GBP          | Invalid          | consultation            | per consultation         | 10.50            | OnDemand         | month                   | per month               | flat    | EstimationPeriodValidValue                 | EstimationPeriod     |
        | Item A                   | Solution          | NULL                | GBP          | month            | NULL                    | NULL                     | 10.50            | Patient          | month                   | per month               | flat    | ItemUnitRequired                           | ItemUnit             |
        | Item A                   | Solution          | NULL                | GBP          | month            | NULL                    | per patient              | 10.50            | Patient          | month                   | per month               | flat    | ItemUnitNameRequired                       | ItemUnit.Name        |
        | Item A                   | Solution          | NULL                | GBP          | month            | #A string of length 21# | per patient              | 10.50            | Patient          | month                   | per month               | flat    | ItemUnitNameTooLong                        | ItemUnit.Name        |
        | Item A                   | Solution          | NULL                | GBP          | month            | patient                 | NULL                     | 10.50            | Patient          | month                   | per month               | flat    | ItemUnitDescriptionRequired                | ItemUnit.Description |
        | Item A                   | Solution          | NULL                | GBP          | month            | patient                 | #A string of length 101# | 10.50            | Patient          | month                   | per month               | flat    | ItemUnitDescriptionTooLong                 | ItemUnit.Description |
        | Item A                   | Solution          | NULL                | GBP          | month            | patient                 | per patient              | -0.0001          | Patient          | month                   | per month               | flat    | PriceGreaterThanOrEqualToZero              | Price                |
        | Item A                   | Solution          | NULL                | GBP          | month            | patient                 | per patient              | 1000000000000000 | Patient          | month                   | per month               | flat    | PriceLessThanMax                           | Price                |
        | Item A                   | Solution          | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | NULL             | month                   | per month               | flat    | ProvisioningTypeRequired                   | ProvisioningType     |
        | Item A                   | Solution          | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | Invalid          | month                   | per month               | flat    | ProvisioningTypeValidValue                 | ProvisioningType     |
        | Item A                   | Solution          | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | NULL                    | NULL                    | flat    | TimeUnitRequired                           | TimeUnit             |
        | Item A                   | Solution          | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | NULL                    | per month               | flat    | TimeUnitNameRequired                       | TimeUnit.Name        |
        | Item A                   | Solution          | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | #A string of length 21# | per month               | flat    | TimeUnitNameTooLong                        | TimeUnit.Name        |
        | Item A                   | Solution          | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | NULL                    | flat    | TimeUnitDescriptionRequired                | TimeUnit.Description |
        | Item A                   | Solution          | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | #A string of length 33# | flat    | TimeUnitDescriptionTooLong                 | TimeUnit.Description |
        | Item A                   | Solution          | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | NULL    | TypeRequired                               | Type                 |
        | Item A                   | Solution          | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | Invalid | TypeValidValue                             | Type                 |

Scenario: create catalogue solution order item with missing service recipients
    Given the user creates a request to add the catalogue item with ID 1000-002 and the following details to the order with ID 10001
        | CatalogueItemName | CatalogueItemType   | CatalogueSolutionId |
        | Item A            | <CatalogueItemType> | 1000-001            |
    When the user sends the create order items request
    Then a response with status code 400 is returned
    And the response contains the following error details
        | Id                        | Field             |
        | ServiceRecipientsRequired | ServiceRecipients |

    Examples: request payloads
        | CatalogueItemType |
        | Solution          |
        | AdditionalService |

Scenario: create catalogue solution order item with a missing or invalid recipient field
    Given the user creates a request to add the catalogue item with ID 1000-002 and the following details to the order with ID 10001
        | CatalogueItemName | CatalogueItemType |
        | Item A            | Solution          |
    And for the following recipients
        | OdsCode   | Name   | Quantity   | DeliveryDate   |
        | <OdsCode> | <Name> | <Quantity> | <DeliveryDate> |
    When the user sends the create order items request
    Then a response with status code 400 is returned
    And the response contains the following error details
        | Id        | Field                             |
        | <ErrorId> | ServiceRecipients[0].<ErrorField> |

    Examples: request payloads
        | OdsCode                | Name                     | Quantity   | DeliveryDate | ErrorId                           | ErrorField   |
        | NULL                   | Recipient 1              | 1          | 15/03/2021   | OdsCodeRequired                   | OdsCode      |
        | #A string of length 9# | Recipient 1              | 1          | 15/03/2021   | OdsCodeTooLong                    | OdsCode      |
        | ODS1                   | #A string of length 257# | 1          | 15/03/2021   | NameTooLong                       | Name         |
        | ODS1                   | Recipient 1              | NULL       | 15/03/2021   | QuantityRequired                  | Quantity     |
        | ODS1                   | Recipient 1              | 0          | 15/03/2021   | QuantityGreaterThanZero           | Quantity     |
        | ODS1                   | Recipient 1              | 2147483647 | 15/03/2021   | QuantityLessThanMax               | Quantity     |
        | ODS1                   | Recipient 1              | 1          | NULL         | DeliveryDateRequired              | DeliveryDate |
        | ODS1                   | Recipient 1              | 1          | 31/12/2020   | DeliveryDateOutsideDeliveryWindow | DeliveryDate |
        | ODS1                   | Recipient 1              | 1          | 01/01/2026   | DeliveryDateOutsideDeliveryWindow | DeliveryDate |

@6306
Scenario: create additional service order item with a missing or invalid field
    Given the user creates a request to add the catalogue item with ID 1000-002 and the following details to the order with ID 10001
        | CatalogueItemName   | CatalogueItemType   | CatalogueSolutionId   | CurrencyCode   | EstimationPeriod   | ItemUnitName   | ItemUnitDescription   | Price   | ProvisioningType   | TimeUnitName   | TimeUnitDescription   | Type   |
        | <CatalogueItemName> | <CatalogueItemType> | <CatalogueSolutionId> | <CurrencyCode> | <EstimationPeriod> | <ItemUnitName> | <ItemUnitDescription> | <Price> | <ProvisioningType> | <TimeUnitName> | <TimeUnitDescription> | <Type> |
    And for the following recipients
        | OdsCode | Name        | Quantity | DeliveryDate |
        | ODS1    | Recipient 1 | 1        | 12/03/2021   |
    When the user sends the create order items request
    Then a response with status code 400 is returned
    And the response contains the following error details
        | Id        | Field        |
        | <ErrorId> | <ErrorField> |

    Examples: request payloads
        | CatalogueItemName        | CatalogueItemType | CatalogueSolutionId     | CurrencyCode | EstimationPeriod | ItemUnitName            | ItemUnitDescription      | Price            | ProvisioningType | TimeUnitName            | TimeUnitDescription     | Type    | ErrorId                                    | ErrorField           |
        | NULL                     | AdditionalService | 1000-001                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | flat    | CatalogueItemNameRequired                  | CatalogueItemName    |
        | #A string of length 257# | AdditionalService | 1000-001                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | flat    | CatalogueItemNameTooLong                   | CatalogueItemName    |
        | Item A                   | AdditionalService | NULL                    | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | flat    | CatalogueSolutionIdRequired                | CatalogueSolutionId  |
        | Item A                   | AdditionalService | #A string of length 15# | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | flat    | CatalogueSolutionIdTooLong                 | CatalogueSolutionId  |
        | Item A                   | AdditionalService | 1000-001                | NULL         | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | flat    | CurrencyCodeRequired                       | CurrencyCode         |
        | Item A                   | AdditionalService | 1000-001                | ABC          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | flat    | CurrencyCodeValidValue                     | CurrencyCode         |
        | Item A                   | AdditionalService | 1000-001                | GBP          | NULL             | consultation            | per consultation         | 10.50            | OnDemand         | month                   | per month               | flat    | EstimationPeriodRequiredIfVariableOnDemand | EstimationPeriod     |
        | Item A                   | AdditionalService | 1000-001                | GBP          | Invalid          | consultation            | per consultation         | 10.50            | OnDemand         | month                   | per month               | flat    | EstimationPeriodValidValue                 | EstimationPeriod     |
        | Item A                   | AdditionalService | 1000-001                | GBP          | month            | NULL                    | NULL                     | 10.50            | Patient          | month                   | per month               | flat    | ItemUnitRequired                           | ItemUnit             |
        | Item A                   | AdditionalService | 1000-001                | GBP          | month            | NULL                    | per patient              | 10.50            | Patient          | month                   | per month               | flat    | ItemUnitNameRequired                       | ItemUnit.Name        |
        | Item A                   | AdditionalService | 1000-001                | GBP          | month            | #A string of length 21# | per patient              | 10.50            | Patient          | month                   | per month               | flat    | ItemUnitNameTooLong                        | ItemUnit.Name        |
        | Item A                   | AdditionalService | 1000-001                | GBP          | month            | patient                 | NULL                     | 10.50            | Patient          | month                   | per month               | flat    | ItemUnitDescriptionRequired                | ItemUnit.Description |
        | Item A                   | AdditionalService | 1000-001                | GBP          | month            | patient                 | #A string of length 101# | 10.50            | Patient          | month                   | per month               | flat    | ItemUnitDescriptionTooLong                 | ItemUnit.Description |
        | Item A                   | AdditionalService | 1000-001                | GBP          | month            | patient                 | per patient              | -0.0001          | Patient          | month                   | per month               | flat    | PriceGreaterThanOrEqualToZero              | Price                |
        | Item A                   | AdditionalService | 1000-001                | GBP          | month            | patient                 | per patient              | 1000000000000000 | Patient          | month                   | per month               | flat    | PriceLessThanMax                           | Price                |
        | Item A                   | AdditionalService | 1000-001                | GBP          | month            | patient                 | per patient              | 10.50            | NULL             | month                   | per month               | flat    | ProvisioningTypeRequired                   | ProvisioningType     |
        | Item A                   | AdditionalService | 1000-001                | GBP          | month            | patient                 | per patient              | 10.50            | Invalid          | month                   | per month               | flat    | ProvisioningTypeValidValue                 | ProvisioningType     |
        | Item A                   | AdditionalService | 1000-001                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | NULL                    | NULL                    | flat    | TimeUnitRequired                           | TimeUnit             |
        | Item A                   | AdditionalService | 1000-001                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | NULL                    | per month               | flat    | TimeUnitNameRequired                       | TimeUnit.Name        |
        | Item A                   | AdditionalService | 1000-001                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | #A string of length 21# | per month               | flat    | TimeUnitNameTooLong                        | TimeUnit.Name        |
        | Item A                   | AdditionalService | 1000-001                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | NULL                    | flat    | TimeUnitDescriptionRequired                | TimeUnit.Description |
        | Item A                   | AdditionalService | 1000-001                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | #A string of length 33# | flat    | TimeUnitDescriptionTooLong                 | TimeUnit.Description |
        | Item A                   | AdditionalService | 1000-001                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | NULL    | TypeRequired                               | Type                 |
        | Item A                   | AdditionalService | 1000-001                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | Invalid | TypeValidValue                             | Type                 |

Scenario: create additional service order item with a missing or invalid recipient field
    Given the user creates a request to add the catalogue item with ID 1000-002 and the following details to the order with ID 10001
        | CatalogueItemName | CatalogueItemType | CatalogueSolutionId |
        | Item A            | AdditionalService | 1000-001            |
    And for the following recipients
        | OdsCode   | Name   | Quantity   | DeliveryDate   |
        | <OdsCode> | <Name> | <Quantity> | <DeliveryDate> |
    When the user sends the create order items request
    Then a response with status code 400 is returned
    And the response contains the following error details
        | Id        | Field                             |
        | <ErrorId> | ServiceRecipients[0].<ErrorField> |

    Examples: request payloads
        | OdsCode                | Name                     | Quantity   | DeliveryDate | ErrorId                           | ErrorField   |
        | NULL                   | Recipient 1              | 1          | 15/03/2021   | OdsCodeRequired                   | OdsCode      |
        | #A string of length 9# | Recipient 1              | 1          | 15/03/2021   | OdsCodeTooLong                    | OdsCode      |
        | ODS1                   | #A string of length 257# | 1          | 15/03/2021   | NameTooLong                       | Name         |
        | ODS1                   | Recipient 1              | NULL       | 15/03/2021   | QuantityRequired                  | Quantity     |
        | ODS1                   | Recipient 1              | 0          | 15/03/2021   | QuantityGreaterThanZero           | Quantity     |
        | ODS1                   | Recipient 1              | 2147483647 | 15/03/2021   | QuantityLessThanMax               | Quantity     |

@6306
Scenario: create associated service order item with a missing or invalid field
    Given the user creates a request to add the catalogue item with ID 1000-002 and the following details to the order with ID 10001
        | CatalogueItemName   | CatalogueItemType   | CatalogueSolutionId   | CurrencyCode   | EstimationPeriod   | ItemUnitName   | ItemUnitDescription   | Price   | ProvisioningType   | TimeUnitName   | TimeUnitDescription   | Type   |
        | <CatalogueItemName> | <CatalogueItemType> | <CatalogueSolutionId> | <CurrencyCode> | <EstimationPeriod> | <ItemUnitName> | <ItemUnitDescription> | <Price> | <ProvisioningType> | <TimeUnitName> | <TimeUnitDescription> | <Type> |
    And for the following recipients
        | OdsCode | Name        | Quantity | DeliveryDate |
        | ODS1    | Recipient 1 | 1        | 12/03/2021   |
    When the user sends the create order items request
    Then a response with status code 400 is returned
    And the response contains the following error details
        | Id        | Field        |
        | <ErrorId> | <ErrorField> |

    Examples: request payloads
        | CatalogueItemName        | CatalogueItemType | CatalogueSolutionId | CurrencyCode | EstimationPeriod | ItemUnitName            | ItemUnitDescription      | Price            | ProvisioningType | TimeUnitName            | TimeUnitDescription     | Type    | ErrorId                                    | ErrorField           |
        | NULL                     | AssociatedService | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | flat    | CatalogueItemNameRequired                  | CatalogueItemName    |
        | #A string of length 257# | AssociatedService | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | flat    | CatalogueItemNameTooLong                   | CatalogueItemName    |
        | Item A                   | AssociatedService | NULL                | NULL         | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | flat    | CurrencyCodeRequired                       | CurrencyCode         |
        | Item A                   | AssociatedService | NULL                | ABC          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | flat    | CurrencyCodeValidValue                     | CurrencyCode         |
        | Item A                   | AssociatedService | NULL                | GBP          | NULL             | consultation            | per consultation         | 10.50            | OnDemand         | month                   | per month               | flat    | EstimationPeriodRequiredIfVariableOnDemand | EstimationPeriod     |
        | Item A                   | AssociatedService | NULL                | GBP          | Invalid          | consultation            | per consultation         | 10.50            | OnDemand         | month                   | per month               | flat    | EstimationPeriodValidValue                 | EstimationPeriod     |
        | Item A                   | AssociatedService | NULL                | GBP          | month            | NULL                    | NULL                     | 10.50            | Patient          | month                   | per month               | flat    | ItemUnitRequired                           | ItemUnit             |
        | Item A                   | AssociatedService | NULL                | GBP          | month            | NULL                    | per patient              | 10.50            | Patient          | month                   | per month               | flat    | ItemUnitNameRequired                       | ItemUnit.Name        |
        | Item A                   | AssociatedService | NULL                | GBP          | month            | #A string of length 21# | per patient              | 10.50            | Patient          | month                   | per month               | flat    | ItemUnitNameTooLong                        | ItemUnit.Name        |
        | Item A                   | AssociatedService | NULL                | GBP          | month            | patient                 | NULL                     | 10.50            | Patient          | month                   | per month               | flat    | ItemUnitDescriptionRequired                | ItemUnit.Description |
        | Item A                   | AssociatedService | NULL                | GBP          | month            | patient                 | #A string of length 101# | 10.50            | Patient          | month                   | per month               | flat    | ItemUnitDescriptionTooLong                 | ItemUnit.Description |
        | Item A                   | AssociatedService | NULL                | GBP          | month            | patient                 | per patient              | -0.0001          | Patient          | month                   | per month               | flat    | PriceGreaterThanOrEqualToZero              | Price                |
        | Item A                   | AssociatedService | NULL                | GBP          | month            | patient                 | per patient              | 1000000000000000 | Patient          | month                   | per month               | flat    | PriceLessThanMax                           | Price                |
        | Item A                   | AssociatedService | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | NULL             | month                   | per month               | flat    | ProvisioningTypeRequired                   | ProvisioningType     |
        | Item A                   | AssociatedService | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | Invalid          | month                   | per month               | flat    | ProvisioningTypeValidValue                 | ProvisioningType     |
        | Item A                   | AssociatedService | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | NULL    | TypeRequired                               | Type                 |
        | Item A                   | AssociatedService | NULL                | GBP          | month            | patient                 | per patient              | 10.50            | Patient          | month                   | per month               | Invalid | TypeValidValue                             | Type                 |

Scenario: create associated service order item with a missing or invalid recipient field
    Given the user creates a request to add the catalogue item with ID 1000-002 and the following details to the order with ID 10001
        | CatalogueItemName | CatalogueItemType |
        | Item A            | AssociatedService |
    And for the following recipients
        | OdsCode   | Name   | Quantity   | DeliveryDate   |
        | <OdsCode> | <Name> | <Quantity> | <DeliveryDate> |
    When the user sends the create order items request
    Then a response with status code 400 is returned
    And the response contains the following error details
        | Id        | Field                             |
        | <ErrorId> | ServiceRecipients[0].<ErrorField> |

    Examples: request payloads
        | OdsCode                | Name                     | Quantity   | DeliveryDate | ErrorId                           | ErrorField   |
        | NULL                   | Recipient 1              | 1          | 15/03/2021   | OdsCodeRequired                   | OdsCode      |
        | #A string of length 9# | Recipient 1              | 1          | 15/03/2021   | OdsCodeTooLong                    | OdsCode      |
        | ODS1                   | #A string of length 257# | 1          | 15/03/2021   | NameTooLong                       | Name         |
        | ODS1                   | Recipient 1              | NULL       | 15/03/2021   | QuantityRequired                  | Quantity     |
        | ODS1                   | Recipient 1              | 0          | 15/03/2021   | QuantityGreaterThanZero           | Quantity     |
        | ODS1                   | Recipient 1              | 2147483647 | 15/03/2021   | QuantityLessThanMax               | Quantity     |
