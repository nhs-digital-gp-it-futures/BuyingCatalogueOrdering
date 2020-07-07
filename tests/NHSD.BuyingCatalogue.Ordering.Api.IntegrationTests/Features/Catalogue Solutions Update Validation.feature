Feature: Update Catalogue Solution Order Item Validation
    As a Buyer
    I want to only create a solution order item if the validation requirements are correct
    So that I can make sure correct information is stored

Background:
    Given Orders exist
        | OrderId    | Description      | LastUpdatedBy                        | LastUpdatedByName | OrganisationId                       | CommencementDate |
        | C000014-01 | Some Description | 335392e4-4bb1-413b-9de5-36a85c9c0422 | Tom Smith         | 4af62b99-638c-4247-875e-965239cd0c48 | 01/01/2021       |
    And Service Recipients exist
        | OdsCode | Name                | OrderId    |
        | ODS1    | Service Recipient   | C000014-01 |
    And Order items exist
        | OrderId    | OdsCode | CatalogueItemName | CatalogueItemType | ProvisioningType |
        | C000014-01 | ODS1    | Order Item 1      | Solution          | OnDemand         |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@7840
Scenario: 1. Update a catalogue solution order item with missing values
    Given the user creates a request to change the catalogue solution order item ('Order Item 1') for the order with ID 'C000014-01'
    And the user enters the '<payload-type>' update catalogue solution order item request payload
    When the user sends the update catalogue solution order item request
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id         | field         |
        | <error-id> | <error-field> |

    Examples: Request payloads
        | payload-type                    | error-id                                   | error-field      |
        | missing-delivery-date           | DeliveryDateRequired                       | DeliveryDate     |
        | missing-quantity                | QuantityRequired                           | Quantity         |
        | missing-estimation-period       | EstimationPeriodRequiredIfVariableOnDemand | EstimationPeriod |
        | missing-price                   | PriceRequired                              | Price            |
        | outside-delivery-window         | DeliveryDateOutsideDeliveryWindow          | DeliveryDate     |
        | less-than-min-quantity          | QuantityGreaterThanZero                    | Quantity         |
        | greater-than-max-quantity       | QuantityLessThanMax                        | Quantity         |
        | less-than-min-price             | PriceGreaterThanOrEqualToZero              | Price            |
        | greater-than-max-price          | PriceLessThanMax                           | Price            |
        | invalid-value-estimation-period | EstimationPeriodValidValue                 | EstimationPeriod |
