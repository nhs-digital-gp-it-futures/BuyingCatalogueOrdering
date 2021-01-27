Feature: Get an order item
    As a Buyer User
    I want to view an order item for an order
    So that I can ensure that the information is correct

Background:
    Given Orders exist
        | OrderId    | IsDeleted | Description               | LastUpdatedBy                        | OrganisationId                       |
        | C000014-01 | false     | Some Description          | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
        | C000014-02 | true      | Deleted Order Description | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    Given Service Recipients exist
        | OrderId    | OdsCode | Name    |
        | C000014-01 | eu      | EU Test |
        | C000014-02 | eu      | EU Test |
    Given Order items exist
        | OrderId    | CatalogueItemName | CatalogueItemType | OdsCode | PriceTimeUnit | EstimationPeriod |
        | C000014-01 | Item 1            | Solution          | eu      | Month         | Month            |
        | C000014-01 | Item 2            | AdditionalService | eu      | Year          | Year             |
        | C000014-01 | Item 3            | AssociatedService | eu      | Month         | Month            |
        | C000014-02 | Item 4            | Solution          | eu      | Month         | Month            |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@7580
Scenario: Get an order item
    Given the user creates a request to retrieve an order item with catalogue item name '<ItemName>' and order ID 'C000014-01'
    When the user sends the retrieve an order item request
    Then a response with status code 200 is returned
    And the response contains the expected order item
    Examples:
        | ItemName |
        | Item 1   |
        | Item 2   |
        | Item 3   |

@8122
Scenario: Get an order item from a deleted order
    Given the user creates a request to retrieve an order item with catalogue item name 'Item 4' and order ID 'C000014-02'
    When the user sends the retrieve an order item request
    Then a response with status code 404 is returned

@7580
Scenario: Get an order item for an order that does not exist
    Given the user creates a request to retrieve an order item that does not exist
    When the user sends the retrieve an order item request
    Then a response with status code 404 is returned

@7580
Scenario: Get an order item that does not exist
    Given the user creates a request to retrieve an order item for an order that does not exist
    When the user sends the retrieve an order item request
    Then a response with status code 404 is returned

@7580
Scenario: Get an order item by a user who is not authorised
    Given no user is logged in
    And the user creates a request to retrieve an order item with catalogue item name 'Item 1' and order ID 'C000014-01'
    When the user sends the retrieve an order item request
    Then a response with status code 401 is returned

Scenario: Get an order item by a non buyer user
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    And the user creates a request to retrieve an order item with catalogue item name 'Item 1' and order ID 'C000014-01'
    When the user sends the retrieve an order item request
    Then a response with status code 403 is returned

@7580
Scenario: Get an order item by a user who belongs to a different organisation
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    And the user creates a request to retrieve an order item with catalogue item name 'Item 1' and order ID 'C000014-01'
    When the user sends the retrieve an order item request
    Then a response with status code 403 is returned

@7580
Scenario: Get an order item when the database is down
    Given the call to the database will fail
    And the user creates a request to retrieve an order item with catalogue item name 'Item 1' and order ID 'C000014-01'
    When the user sends the retrieve an order item request
    Then a response with status code 500 is returned
