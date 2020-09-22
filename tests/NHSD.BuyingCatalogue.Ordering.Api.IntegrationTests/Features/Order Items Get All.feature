Feature: Get all the order items for an order, filtering by catalogue item type
    As a Buyer User
    I want to view the order items for an order
    So that I can ensure that the information is correct

Background:
    Given Orders exist
        | OrderId    | IsDeleted | Description      | LastUpdatedBy                        | OrganisationId                       |
        | C000014-01 | false     | Some Description | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
        | C000014-02 | true      | Some Description | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    Given Service Recipients exist
        | OrderId    | OdsCode | Name     |
        | C000014-01 | eu      | EU Test  |
        | C000014-01 | auz     | AUZ Test |
        | C000014-01 | ods     | NULL     |
        | C000014-02 | eu      | EU Test  |
    Given Order items exist
        | OrderId    | CatalogueItemName | CatalogueItemType | OdsCode | Created    |
        | C000014-01 | Sol 1             | Solution          | eu      | 06/05/2020 |
        | C000014-01 | Sol 2             | Solution          | auz     | 30/04/2020 |
        | C000014-01 | Sol 3             | Solution          | ods     | 21/07/2020 |
        | C000014-01 | Add Serv 1        | AdditionalService | eu      | 05/05/2020 |
        | C000014-01 | Add Serv 2        | AdditionalService | ods     | 10/04/2020 |
        | C000014-01 | Ass Serv 1        | AssociatedService | auz     | 14/12/2019 |
        | C000014-02 | Sol 4             | Solution          | eu      | 06/02/2020 |
        | C000014-02 | Add Serv 3        | AdditionalService | eu      | 05/05/2020 |
        | C000014-02 | Ass Serv 2        | AssociatedService | auz     | 14/12/2019 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5124
Scenario: Get all order items for an ID filtering by catalogue item type
    When the user makes a request to retrieve a list of order items with orderID C000014-01 and catalogueItemType <CatalogueItemType>
    When the user sends the retrieve a list of order items request
    Then a response with status code 200 is returned
    And the order item response displays the expected order items

    Examples:
        | CatalogueItemType |
        | NULL              |
        | Solution          |
        | AdditionalService |
        | AssociatedService |
        | SOLutiON          |
        | AssocIATEDService |
        | ADDITIONALservice |

@5124
Scenario: Get all order items for a deleted order ID filtering by catalogue item type
    When the user makes a request to retrieve a list of order items with orderID C000014-02 and catalogueItemType <CatalogueItemType>
    When the user sends the retrieve a list of order items request
    Then a response with status code 404 is returned

    Examples:
        | CatalogueItemType |
        | NULL              |
        | Solution          |
        | AdditionalService |

@5124
Scenario: Passing through an order ID that does not exist, returns Not Found
    When the user makes a request to retrieve a list of order items with orderID INVALID and catalogueItemType Solution
    When the user sends the retrieve a list of order items request
    Then a response with status code 404 is returned

@5124
Scenario: Passing through an invalid catalogue item type, returns an empty list
    When the user makes a request to retrieve a list of order items with orderID C000014-01 and catalogueItemType INVALID
    When the user sends the retrieve a list of order items request
    Then a response with status code 200 is returned
    And the list order items response contains no entries

@5124
Scenario: If a user is not authorised then they cannot access the order items
    Given no user is logged in
    When the user makes a request to retrieve a list of order items with orderID C000014-01 and catalogueItemType Solution
    When the user sends the retrieve a list of order items request
    Then a response with status code 401 is returned

@5124
Scenario: A non buyer user cannot access the order items
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve a list of order items with orderID C000014-01 and catalogueItemType Solution
    When the user sends the retrieve a list of order items request
    Then a response with status code 403 is returned

@5124
Scenario: A buyer user cannot access the order items for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve a list of order items with orderID C000014-01 and catalogueItemType Solution
    When the user sends the retrieve a list of order items request
    Then a response with status code 403 is returned

@5124
Scenario: Service Failure
    Given the call to the database will fail
    When the user makes a request to retrieve a list of order items with orderID C000014-01 and catalogueItemType Solution
    When the user sends the retrieve a list of order items request
    Then a response with status code 500 is returned
