Feature: get all the order items for an order, filtering by catalogue item type
    As a buyer user
    I want to view the order items for an order
    So that I can ensure that the information is correct

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | IsDeleted | Description      | LastUpdatedBy                        | OrderingPartyId                      |
        | 10001   | false     | Some Description | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
        | 10002   | true      | Some Description | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    And pricing units exist
        | Name    | Description |
        | patient | per patient |
    And service recipients exist
        | OdsCode | Name     |
        | eu      | EU Test  |
        | auz     | AUZ Test |
        | ods     | NULL     |
    And selected service recipients exist
        | OrderId | OdsCode |
        | 10001   | eu      |
        | 10001   | auz     |
        | 10001   | ods     |
        | 10002   | eu      |
    And catalogue items exist
        | Id       | Name       | CatalogueItemType | ParentCatalogueItemId |
        | 1000-001 | Sol 1      | Solution          | NULL                  |
        | 1000-002 | Sol 2      | Solution          | NULL                  |
        | 1000-003 | Sol 3      | Solution          | NULL                  |
        | 1000-004 | Sol 4      | Solution          | NULL                  |
        | 1000-005 | Add Serv 1 | AdditionalService | 1000-001              |
        | 1000-006 | Add Serv 2 | AdditionalService | 1000-002              |
        | 1000-007 | Add Serv 3 | AdditionalService | 1000-003              |
        | 1000-008 | Ass Serv 1 | AssociatedService | NULL                  |
        | 1000-009 | Ass Serv 2 | AssociatedService | NULL                  |
    And order items exist
        | OrderId | CatalogueItemId | Created    |
        | 10001   | 1000-001        | 06/05/2020 |
        | 10001   | 1000-002        | 30/04/2020 |
        | 10001   | 1000-003        | 21/07/2020 |
        | 10001   | 1000-005        | 05/05/2020 |
        | 10001   | 1000-006        | 10/04/2020 |
        | 10001   | 1000-008        | 14/12/2019 |
        | 10002   | 1000-004        | 06/02/2020 |
        | 10002   | 1000-007        | 05/05/2020 |
        | 10002   | 1000-009        | 14/12/2019 |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 1000-001        | eu      |
        | 10001   | 1000-002        | auz     |
        | 10001   | 1000-003        | ods     |
        | 10001   | 1000-005        | eu      |
        | 10001   | 1000-006        | ods     |
        | 10001   | 1000-008        | auz     |
        | 10002   | 1000-004        | eu      |
        | 10002   | 1000-007        | eu      |
        | 10002   | 1000-009        | auz     |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5124
Scenario: get all order items for an ID filtering by catalogue item type
    When the user makes a request to retrieve a list of order items with orderID 10001 and catalogueItemType <CatalogueItemType>
    And the user sends the retrieve a list of order items request
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
Scenario: get all order items for a deleted order ID filtering by catalogue item type
    When the user makes a request to retrieve a list of order items with orderID 10002 and catalogueItemType <CatalogueItemType>
    And the user sends the retrieve a list of order items request
    Then a response with status code 404 is returned

    Examples:
        | CatalogueItemType |
        | NULL              |
        | Solution          |
        | AdditionalService |

@5124
Scenario: passing through an order ID that does not exist, returns Not Found
    When the user makes a request to retrieve a list of order items with orderID 10000 and catalogueItemType Solution
    And the user sends the retrieve a list of order items request
    Then a response with status code 404 is returned

@5124
Scenario: passing through an invalid catalogue item type returns a validation error
    When the user makes a request to retrieve a list of order items with orderID 10001 and catalogueItemType INVALID
    And the user sends the retrieve a list of order items request
    Then a response with status code 400 is returned
    And the response contains the following errors
        | Id                                | Field             |
        | The value 'INVALID' is not valid. | catalogueItemType |

@5124
Scenario: if a user is not authorised then they cannot access the order items
    Given no user is logged in
    When the user makes a request to retrieve a list of order items with orderID 10001 and catalogueItemType Solution
    And the user sends the retrieve a list of order items request
    Then a response with status code 401 is returned

@5124
Scenario: a non-buyer user cannot access the order items
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve a list of order items with orderID 10001 and catalogueItemType Solution
    And the user sends the retrieve a list of order items request
    Then a response with status code 403 is returned

@5124
Scenario: a buyer user cannot access the order items for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve a list of order items with orderID 10001 and catalogueItemType Solution
    And the user sends the retrieve a list of order items request
    Then a response with status code 403 is returned

@5124
Scenario: service failure
    Given the call to the database will fail
    When the user makes a request to retrieve a list of order items with orderID 10001 and catalogueItemType Solution
    And the user sends the retrieve a list of order items request
    Then a response with status code 500 is returned
