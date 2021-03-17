Feature: get the price calculation details of an order
    As a buyer user
    I want to be able to view the pricing of a given order
    So that I can ensure that the information is correct

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And pricing units exist
        | Name    | Description |
        | patient | per patient |
    And orders exist
        | OrderId | Description   | OrderingPartyId                      |
        | 10001   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 |
    And service recipients exist
        | OdsCode | Name    |
        | eu      | EU Test |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

Scenario: verify the price calculations for Catalogue Solution order types
    Given catalogue items exist
        | Id        | Name | CatalogueItemType |
        | 10001-001 | Sol1 | Solution          |
    And order items exist
        | OrderId | CatalogueItemId | Price   | ProvisioningType   | EstimationPeriod   | PriceTimeUnit |
        | 10001   | 10001-001       | <Price> | <ProvisioningType> | <EstimationPeriod> | <TimeUnit>    |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode | Quantity   |
        | 10001   | 10001-001       | eu      | <Quantity> |
    And the user creates a request to retrieve the details of an order by ID 10001
    When the user sends the get order request
    Then a response with status code 200 is returned
    And the get order response contains a yearly value of <Total>

    Examples:
        | ProvisioningType | TimeUnit | Price | Quantity | EstimationPeriod | Total |
        | OnDemand         | Null     | 1     | 10       | Month            | 120   |
        | OnDemand         | Null     | 1.111 | 6        | Year             | 6.666 |
        | OnDemand         | Null     | 0     | 100      | Year             | 0     |
        | OnDemand         | Null     | 0     | 100      | Month            | 0     |
        | Declarative      | Month    | 1     | 10       | Null             | 120   |
        | Patient          | Year     | 1     | 10       | Null             | 10    |

Scenario: verify the price calculations for an order with multiple order items
    Given catalogue items exist
        | Id        | Name | CatalogueItemType   |
        | 10001-001 | Sol1 | <CatalogueItemType> |
        | 10001-002 | Sol2 | AssociatedService   |
        | 10001-003 | Sol3 | AdditionalService   |
        | 10001-004 | Sol4 | AssociatedService   |
    And order items exist
        | OrderId | CatalogueItemId | Price   | ProvisioningType   | EstimationPeriod   | PriceTimeUnit |
        | 10001   | 10001-001       | <Price> | <ProvisioningType> | <EstimationPeriod> | <TimeUnit>    |
        | 10001   | 10001-002       | 200     | OnDemand           | Year               | NULL          |
        | 10001   | 10001-003       | 600     | Declarative        | Month              | Month         |
        | 10001   | 10001-004       | 100     | Declarative        | NULL               | NULL          |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode | Quantity   |
        | 10001   | 10001-001       | eu      | <Quantity> |
        | 10001   | 10001-002       | eu      | 120        |
        | 10001   | 10001-003       | eu      | 1          |
        | 10001   | 10001-004       | eu      | 2          |
    And the user creates a request to retrieve the details of an order by ID 10001
    When the user sends the get order request
    Then a response with status code 200 is returned
    And the get order response contains a totalRecurringCostPerMonth of <RecurringPerMonthValue> for the order
    And the get order response contains a totalRecurringCostPerYear of <RecurringPerYearValue> for the order
    And the get order response contains a totalOneOffCost of <TotalOneOffCost> for the order
    And the get order response contains a totalOwnershipCost of <TotalOwnershipCost> for the order

    Examples:
        | CatalogueItemType | ProvisioningType | TimeUnit | Price | Quantity | EstimationPeriod | RecurringPerMonthValue | RecurringPerYearValue | TotalOneOffCost | TotalOwnershipCost |
        | Solution          | OnDemand         | Null     | 0     | 1        | Month            | 2600                   | 31200                 | 200             | 93800              |
        | AssociatedService | OnDemand         | Null     | 1     | 10       | Month            | 2610                   | 31320                 | 200             | 94160              |
        | AdditionalService | Declarative      | Month    | 9000  | 1        | Year             | 11600                  | 139200                | 200             | 417800             |
        | Solution          | Patient          | Year     | 450   | 3        | Month            | 2712.5                 | 32550                 | 200             | 97850              |
        | AssociatedService | Declarative      | NULL     | 125   | 100      | NULL             | 2600                   | 31200                 | 12700           | 106300             |
