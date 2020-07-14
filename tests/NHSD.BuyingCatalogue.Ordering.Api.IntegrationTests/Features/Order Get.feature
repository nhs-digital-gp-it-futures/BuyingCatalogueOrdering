Feature: Get the details of a single order
    As an Buyer user
    I want to be able to view a preview of a given order
    So that I can ensure that the information is complete

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
        | C000014-01 | au      | UA Test |
    Given Order items exist
        | OrderId    | CatalogueItemId | CatalogueItemName | CatalogueItemType | OdsCode | CataloguePriceUnitDescription | Price   | ProvisioningType | Quantity | PriceTimeUnit | EstimationPeriod | DeliveryDate |
        | C000014-01 | Cat Item 1      | Sol1              | Solution          | eu      | Desc                          | 461.34  | OnDemand         | 5        | Null          | Month            | 03/21/2021   |
        | C000014-01 | Cat Item 2      | Sol2              | AssociatedService | au      | Desc                          | 721.34  | Declarative      | 2        | Month         | Null             | 07/15/2020   |
        | C000014-01 | Cat Item 3      | Sol3              | AdditionalService | eu      | Desc                          | 3532.12 | Patient          | 1        | Year          | Year             | NULL         |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@8122
Scenario: 1. Get an order
    Given the user creates a request to retrieve the details of an order by ID 'C000014-01'
    When the user sends the get order request
    Then a response with status code 200 is returned
    And the get order response displays the expected order

@8122
Scenario: 2. A non existent order ID returns not found
    Given the user creates a request to retrieve the details of an order by ID 'INVALID'
    When the user sends the get order request
    Then a response with status code 404 is returned

@8122
Scenario: 3. If a user is not authorised then they cannot update an order
    Given no user is logged in
    Given the user creates a request to retrieve the details of an order by ID 'C000014-01'
    When the user sends the get order request
    Then a response with status code 401 is returned

@8122
Scenario: 4. A non buyer user cannot update an order
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    Given the user creates a request to retrieve the details of an order by ID 'C000014-01'
    When the user sends the get order request
    Then a response with status code 403 is returned

@8122
Scenario: 5. A buyer user cannot update an order for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    Given the user creates a request to retrieve the details of an order by ID 'C000014-01'
    When the user sends the get order request
    Then a response with status code 403 is returned

@8122
Scenario: 6. Service Failure
    Given the call to the database will fail
    Given the user creates a request to retrieve the details of an order by ID 'C000014-01'
    When the user sends the get order request
    Then a response with status code 500 is returned
