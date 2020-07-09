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
