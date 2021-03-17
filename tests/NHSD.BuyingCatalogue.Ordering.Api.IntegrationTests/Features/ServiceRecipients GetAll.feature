Feature: display all of the selected service recipients for an order
    As a buyer
    I want to view all of the service recipients for an order
    So that I can ensure the information is correct

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | Description         | OrderingPartyId                      |
        | 10001   | Some Description    | 4af62b99-638c-4247-875e-965239cd0c48 |
        | 10002   | Another Description | 4af62b99-638c-4247-875e-965239cd0c48 |
    And pricing units exist
        | Name    | Description |
        | patient | per patient |
    And catalogue items exist
        | Id           | Name                 | CatalogueItemType | ParentCatalogueItemId |
        | 1000-001     | Solution 1           | Solution          | NULL                  |
        | 1000-002     | Solution 2           | Solution          | NULL                  |
        | 1000-003     | Solution 3           | Solution          | NULL                  |
    And service recipients exist
        | OdsCode | Name        | OrderId |
        | ODS1    | Recipient Z | 10001   |
        | ODS2    | Recipient X | 10002   |
        | ODS3    | Recipient Y | 10001   |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@7412
Scenario: get the selected service recipients from an exisiting ordering ID
    Given selected service recipients exist
        | OrderId | OdsCode |
        | 10001   | ODS1    |
        | 10002   | ODS2    |
        | 10001   | ODS3    |
    When the user makes a request to retrieve the service-recipients section with order ID 10001
    Then a response with status code 200 is returned
    And the service recipients are returned
        | OdsCode | Name        |
        | ODS3    | Recipient Y |
        | ODS1    | Recipient Z |

@7412
Scenario: get the service recipients from an exisiting ordering ID when there are no selected service recipients
    Given order items exist
        | OrderId | CatalogueItemId |
        | 10001   | 1000-001        |
        | 10001   | 1000-002        |
        | 10001   | 1000-003        |
        | 10002   | 1000-001        |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 1000-001        | ODS1    |
        | 10001   | 1000-002        | ODS3    |
        | 10001   | 1000-003        | ODS1    |
        | 10002   | 1000-001        | ODS2    |
    When the user makes a request to retrieve the service-recipients section with order ID 10001
    Then a response with status code 200 is returned
    And the service recipients are returned
        | OdsCode | Name        |
        | ODS3    | Recipient Y |
        | ODS1    | Recipient Z |

@7412
Scenario: if an order does not exist, return not found
    When the user makes a request to retrieve the service-recipients section with order ID 10000
    Then a response with status code 404 is returned

@7412
Scenario: if a user is not authorised then they cannot access the service recipients section
    Given no user is logged in
    When the user makes a request to retrieve the service-recipients section with order ID 10001
    Then a response with status code 401 is returned

@7412
Scenario: a non-buyer user cannot access the service recipients section
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve the service-recipients section with order ID 10001
    Then a response with status code 403 is returned

@7412
Scenario: a buyer user cannot access the service recipients section for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve the service-recipients section with order ID 10001
    Then a response with status code 403 is returned

@7412
Scenario: service failure
    Given the call to the database will fail
    When the user makes a request to retrieve the service-recipients section with order ID 10001
    Then a response with status code 500 is returned
