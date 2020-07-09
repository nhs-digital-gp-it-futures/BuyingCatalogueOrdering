﻿Feature: Get all the order items for an order, filtering by catalogue item type
	As a Buyer Useer
	I want to view the order items for an order
	So that I can ensure that the information is correct

Background:
    Given Orders exist
        | OrderId    | Description      | OrderStatusId | LastUpdatedBy                        | OrganisationId                       |
        | C000014-01 | Some Description | 1             | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    Given Service Recipients exist
        | OrderId    | OdsCode | Name     |
        | C000014-01 | eu      | EU Test  |
        | C000014-01 | auz     | AUZ Test |
        | C000014-01 | ods     | NULL     |
    Given Order items exist
        | OrderId    | CatalogueItemName | CatalogueItemType | OdsCode |
        | C000014-01 | Sol 1             | Solution          | eu      |
        | C000014-01 | Sol 2             | Solution          | auz     |
        | C000014-01 | Sol 3             | Solution          | ods     |
        | C000014-01 | Add Serv 1        | AdditionalService | eu      |
        | C000014-01 | Add Serv 2        | AdditionalService | ods     |
        | C000014-01 | Ass Serv 1        | AssociatedService | auz     |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5124
Scenario: 1. Get all order items for an ID
    When the user makes a request to retrieve an order item with orderID C000014-01 and catalogueItemType NULL
    When the user sends the order item request
    Then a response with status code 200 is returned
    And the order item response displays the expected order item

@5124
Scenario: 2. Get all order items for an ID filtering by catalogue item type
    When the user makes a request to retrieve an order item with orderID C000014-01 and catalogueItemType <CatalogueItemType>
    When the user sends the order item request
    Then a response with status code 200 is returned
    And the order item response displays the expected order item

    Examples:
        | CatalogueItemType |
        | Solution          |
        | AdditionalService |
        | AssociatedService |

@5124
Scenario: 3. Passing through an order ID that does not exist, returns Not Found
    When the user makes a request to retrieve an order item with orderID INVALID and catalogueItemType Solution
    When the user sends the order item request
    Then a response with status code 404 is returned

@5124
Scenario: 4. Passing through an invalid catalogue item type, returns an empty list
    When the user makes a request to retrieve an order item with orderID C000014-01 and catalogueItemType INVALID
    When the user sends the order item request
    Then a response with status code 200 is returned
    And an empty catalougeItem list is returned

@5124
Scenario: 5. If a user is not authorised then they cannot access the order items
    Given no user is logged in
    When the user makes a request to retrieve an order item with orderID C000014-01 and catalogueItemType Solution
    When the user sends the order item request
    Then a response with status code 401 is returned

@5124
Scenario: 6. A non buyer user cannot access the order items
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve an order item with orderID C000014-01 and catalogueItemType Solution
    When the user sends the order item request
    Then a response with status code 403 is returned

@5124
Scenario: 7. A buyer user cannot access the order items for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve an order item with orderID C000014-01 and catalogueItemType Solution
    When the user sends the order item request
    Then a response with status code 403 is returned

@5124
Scenario: 8. Service Failure
    Given the call to the database will fail
    When the user makes a request to retrieve an order item with orderID C000014-01 and catalogueItemType Solution
    When the user sends the order item request
    Then a response with status code 500 is returned
