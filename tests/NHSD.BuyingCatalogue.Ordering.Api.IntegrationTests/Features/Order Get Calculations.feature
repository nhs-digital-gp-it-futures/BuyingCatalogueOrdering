Feature: Get the price calculation details of an order
    As an Buyer user
    I want to be able to view the pricing of a given order
    So that I can ensure that the information is correct

Background:
    Given Addresses exist
        | Line1 | Line2      | Line3      | Line4          | Line5          | Town      | County  | Postcode | Country        |
        | 4     | Upper Flat | Rocks Lane | Little Village | Bigger Village | Some Town | W Yorks | LS 1 3AP | United Kingdom |
    Given Contacts exist
        | FirstName | LastName | EmailAddress            | TelephoneNumber |
        | Fred      | Robinson | Fred.robinson@email.com | 12312543212     |
    Given Orders exist
        | OrderId    | Description   | OrganisationId                       | OrganisationName | OrganisationOdsCode | OrganisationContactEmail | SupplierAddressPostcode | SupplierContactEmail    |
        | C000014-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | Hampshire CC     | 432432              | Fred.robinson@email.com  | LS 1 3AP                | Fred.robinson@email.com |
    Given Service Recipients exist
        | OrderId    | OdsCode | Name    |
        | C000014-01 | eu      | EU Test |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

Scenario: 1. Verify the price calculations for Catalogue Solution order types
    Given Order items exist
        | OrderId    | CatalogueItemId | CatalogueItemName | CatalogueItemType | OdsCode | CataloguePriceUnitDescription | Price   | ProvisioningType   | Quantity   | EstimationPeriod   | PriceTimeUnit |
        | C000014-01 | Cat Item 1      | Sol1              | Solution          | eu      | Desc                          | <Price> | <ProvisioningType> | <Quantity> | <EstimationPeriod> | <TimeUnit>    |
    Given the user creates a request to retrieve the details of an order by ID 'C000014-01'
    When the user sends the get order request
    Then a response with status code 200 is returned
    And the get order response contains a yearly value of <Total> for order item with name 'Sol1'

    Examples:
        | ProvisioningType | TimeUnit | Price | Quantity | EstimationPeriod | Total |
        | OnDemand         | Null     | 1     | 10       | Month            | 120   |
        | OnDemand         | Null     | 1.111 | 6        | Year             | 6.666 |
        | OnDemand         | Null     | 100   | 0        | Year             | 0     |
        | OnDemand         | Null     | 100   | 0        | Month            | 0     |
        | Declarative      | Month    | 1     | 10       | Null             | 120   |
        | Patient          | Year     | 1     | 10       | Null             | 10    |

Scenario: 2. Verify the price calculations for an order with multiple order items
    Given Order items exist
        | OrderId    | CatalogueItemId | CatalogueItemName | CatalogueItemType   | OdsCode | CataloguePriceUnitDescription | Price   | ProvisioningType   | Quantity   | EstimationPeriod   | PriceTimeUnit |
        | C000014-01 | Cat Item 1      | Sol1              | <CatalogueItemType> | eu      | Desc                          | <Price> | <ProvisioningType> | <Quantity> | <EstimationPeriod> | <TimeUnit>    |
        | C000014-01 | Cat Item 2      | Sol2              | AssociatedService   | eu      | New Desc                      | 200     | OnDemand           | 120        | Year               | NULL          |
        | C000014-01 | Cat Item 3      | Sol3              | AdditionalService   | eu      | Another Desc                  | 600     | Declarative        | 1          | Month              | Month         |
        | C000014-01 | Cat Item 4      | Sol4              | AssociatedService   | eu      | Yet Another Desc              | 100     | Declarative        | 2          | NULL               | NULL          |
    Given the user creates a request to retrieve the details of an order by ID 'C000014-01'
    When the user sends the get order request
    Then a response with status code 200 is returned
    And the get order response contains a totalRecurringCostPerMonth of <RecurringPerMonthValue> for the order
    And the get order response contains a totalRecurringCostPerYear of <RecurringPerYearValue> for the order
    And the get order response contains a totalOneOffCost of <TotalOneOffCost> for the order
    And the get order response contains a totalOwnershipCost of <TotalOwnershipCost> for the order

    Examples:
        | CatalogueItemType | ProvisioningType | TimeUnit | Price | Quantity | EstimationPeriod | RecurringPerMonthValue | RecurringPerYearValue | TotalOneOffCost | TotalOwnershipCost |
        | Solution          | OnDemand         | Null     | 0     | 0        | Month            | 2600                   | 31200                 | 200             | 93800              |
        | AssociatedService | OnDemand         | Null     | 1     | 10       | Month            | 2610                   | 31320                 | 200             | 94160              |
        | AdditionalService | Declarative      | Month    | 9000  | 1        | Year             | 11600                  | 139200                | 200             | 417800             |
        | Solution          | Patient          | Year     | 450   | 3        | Month            | 2712.5                 | 32550                 | 200             | 97850              |
        | AssociatedService | Declarative      | NULL     | 125   | 100      | NULL             | 2600                   | 31200                 | 12700           | 106300             |
