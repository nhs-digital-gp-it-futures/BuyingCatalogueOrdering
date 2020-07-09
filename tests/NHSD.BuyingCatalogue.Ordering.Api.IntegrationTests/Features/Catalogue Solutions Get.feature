Feature: Get a single order item
    As a Buyer User
    I want to view a single catalogue solution for a given order by the orderItem ID
    So that I can ensure that the information is correct

Background:
    Given Orders exist
        | OrderId    | Description      | OrderStatusId | LastUpdatedBy                        | OrganisationId                       |
        | C000014-01 | Some Description | 1             | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    Given Service Recipients exist
        | OrderId    | OdsCode | Name    |
        | C000014-01 | eu      | EU Test |
        | C000014-01 | ods     | NULL    |
    Given Order items exist
        | OrderId    | CatalogueItemId | CatalogueItemName | CatalogueItemType | OdsCode | CurrencyCode | DeliveryDate | EstimationPeriod | CataloguePriceUnitName | CataloguePriceUnitDescription | Price   | ProvisioningType | Quantity |
        | C000014-01 | Cat Item 1      | Sol1              | Solution          | eu      | GBP          | 01/01/2021   | Month            | Tier                   | Desc                          | 461.34  | Declarative      | 5        |
        | C000014-01 | Cat Item 2      | Sol2              | AdditionalService | ods     | USD          | 09/03/2022   | Year             | Unit                   | Another Desc                  | 3521.67 | Patient          | 3        |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@7840
Scenario: 1. Get a order item
    When the user makes a request to retrieve an order catalogue solution With orderID C000014-01 and CatalogueItemName Sol1
    Then a response with status code 200 is returned
    And the catalogue solutions response contains a single solution
        | ServiceRecipientOdsCode | ServiceRecipientName | CatalogueItemId | CatalogueItemName | CurrencyCode | DeliveryDate | EstimationPeriod | ItemUnitName | ItemUnitDescription | Price  | ProvisioningType | Quantity | Type |
        | eu                      | EU Test              | Cat Item 1      | Sol1              | GBP          | 01/01/2021   | month            | Tier         | Desc                | 461.34 | Declarative      | 5        | Flat |

@7840
Scenario: 2. A order item type that isn't solution, returns not found
    When the user makes a request to retrieve an order catalogue solution With orderID C000014-01 and CatalogueItemName Sol2
    Then a response with status code 404 is returned

@7840
Scenario: 3. A non existent order ID returns not found
    When the user makes a request to retrieve an order catalogue solution With orderID INVALID and CatalogueItemName Sol1
    Then a response with status code 404 is returned

@7840
Scenario: 4. A non existent order item ID returns not found
    When the user makes a request to retrieve an order catalogue solution With orderID C000014-01 and Invalid OrderItemID -1
    Then a response with status code 404 is returned

@7840
Scenario: 5. If a user is not authorised then they cannot access the order catalogue solutions
    Given no user is logged in
    When the user makes a request to retrieve an order catalogue solution With orderID C000014-01 and CatalogueItemName Sol1
    Then a response with status code 401 is returned

@7840
Scenario: 6. A non buyer user cannot access the order catalogue solutions
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve an order catalogue solution With orderID C000014-01 and CatalogueItemName Sol1
    Then a response with status code 403 is returned

@7840
Scenario: 7. A buyer user cannot access the order catalogue solutions for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve an order catalogue solution With orderID C000014-01 and CatalogueItemName Sol1
    Then a response with status code 403 is returned

@7840
Scenario: 8. Service Failure
    Given the call to the database will fail
    When the user makes a request to retrieve an order catalogue solution With orderID C000014-01 and CatalogueItemName Sol1
    Then a response with status code 500 is returned
