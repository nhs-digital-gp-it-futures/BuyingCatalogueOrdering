Feature: get an order item
    As a buyer user
    I want to view an order item for an order
    So that I can ensure that the information is correct

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | IsDeleted | Description               | LastUpdatedBy                        | OrderingPartyId                      |
        | 10001   | false     | Some Description          | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
        | 10002   | true      | Deleted Order Description | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    And pricing units exist
        | Name    | Description |
        | patient | per patient |
    And service recipients exist
        | OdsCode | Name    |
        | eu      | EU Test |
    And selected service recipients exist
        | OrderId | OdsCode |
        | 10001   | eu      |
        | 10002   | eu      |
    And catalogue items exist
        | Id       | Name   | CatalogueItemType | ParentCatalogueItemId |
        | 1000-001 | Item 1 | Solution          | NULL                  |
        | 1000-002 | Item 2 | AdditionalService | 1000-001              |
        | 1000-003 | Item 3 | AssociatedService | NULL                  |
        | 1000-004 | Item 4 | Solution          | NULL                  |
    And order items exist
        | OrderId | CatalogueItemId | PriceTimeUnit | EstimationPeriod |
        | 10001   | 1000-001        | Month         | Month            |
        | 10001   | 1000-002        | Year          | Year             |
        | 10001   | 1000-003        | Month         | Month            |
        | 10002   | 1000-004        | Month         | Month            |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 1000-001        | eu      |
        | 10001   | 1000-002        | eu      |
        | 10001   | 1000-003        | eu      |
        | 10002   | 1000-004        | eu      |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@7580
Scenario: get an order item
    Given the user creates a request to retrieve an order item with catalogue item ID '<ItemId>' and order ID 10001
    When the user sends the retrieve an order item request
    Then a response with status code 200 is returned
    And the response contains the expected order item
    Examples:
        | ItemId   |
        | 1000-001 |
        | 1000-002 |
        | 1000-003 |

@8122
Scenario: get an order item from a deleted order
    Given the user creates a request to retrieve an order item with catalogue item ID '1000-004' and order ID 10002
    When the user sends the retrieve an order item request
    Then a response with status code 404 is returned

@7580
Scenario: get an order item that does not exist
    Given the user creates a request to retrieve an order item that does not exist
    When the user sends the retrieve an order item request
    Then a response with status code 404 is returned

@7580
Scenario: get an order item for an order that does not exist
    Given the user creates a request to retrieve an order item for an order that does not exist
    When the user sends the retrieve an order item request
    Then a response with status code 404 is returned

@7580
Scenario: get an order item by a user who is not authorised
    Given no user is logged in
    And the user creates a request to retrieve an order item with catalogue item ID '1000-001' and order ID 10001
    When the user sends the retrieve an order item request
    Then a response with status code 401 is returned

Scenario: get an order item by a non buyer user
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    And the user creates a request to retrieve an order item with catalogue item ID '1000-001' and order ID 10001
    When the user sends the retrieve an order item request
    Then a response with status code 403 is returned

@7580
Scenario: get an order item by a user who belongs to a different organisation
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    And the user creates a request to retrieve an order item with catalogue item ID '1000-001' and order ID 10001
    When the user sends the retrieve an order item request
    Then a response with status code 403 is returned

@7580
Scenario: get an order item when the database is down
    Given the call to the database will fail
    And the user creates a request to retrieve an order item with catalogue item ID '1000-001' and order ID 10001
    When the user sends the retrieve an order item request
    Then a response with status code 500 is returned
