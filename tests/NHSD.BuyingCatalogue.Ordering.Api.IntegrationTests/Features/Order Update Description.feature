Feature: update an order's description in a buyer section
    As a buyer
    I want to update an order's description
    So that I can keep an order up to date

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    Given orders exist
        | OrderId | Description         | OrderingPartyId                      | Created    |
        | 10001   | Some Description    | 4af62b99-638c-4247-875e-965239cd0c48 | 05/03/2021 |
        | 10002   | Another Description | 4af62b99-638c-4247-875e-965239cd0c48 | 05/03/2021 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5322
Scenario: updating an order's description
    When the user makes a request to update the description for the order with ID 10001
        | Description         |
        | Another Description |
    Then a response with status code 204 is returned
    And the order description for the order with ID 10001 is set to
        | Description         |
        | Another Description |
    And the lastUpdatedName is updated in the database to Bob Smith for the order with ID 10001

@5322
Scenario: updating an orders description and with a changed user name
    When the user makes a request to update the description for the order with ID 10002
        | Description      |
        | Test Description |
    Then a response with status code 204 is returned
    And the order description for the order with ID 10002 is set to
        | Description      |
        | Test Description |
    And the lastUpdatedName is updated in the database to Bob Smith for the order with ID 10002

@5322
Scenario: updating an order, with a non existent model returns not found
    When the user makes a request to update the description for the order with ID 10001 with no model
    Then a response with status code 400 is returned

@5322
Scenario: updating an order, with no description, returns a relevant error message
    When the user makes a request to update the description for the order with ID 10001
        | Description |
        | NULL        |
    Then a response with status code 400 is returned
    And the response contains the following errors
        | Id                  | Field       |
        | DescriptionRequired | Description |

@5322
Scenario: updating an order, with a description, exceeding it's maximum limit, returns a relevant error message
    When the user makes a request to update the description for the order with ID 10001
        | Description              |
        | #A string of length 101# |
    Then a response with status code 400 is returned
    And the response contains the following errors
        | Id                 | Field       |
        | DescriptionTooLong | Description |

@5322
Scenario: if a user is not authorised, then they cannot update the orders description
    Given no user is logged in
    When the user makes a request to update the description for the order with ID 10001
        | Description         |
        | Another Description |
    Then a response with status code 401 is returned

@5322
Scenario: a non-buyer user cannot update an orders description
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to update the description for the order with ID 10001
        | Description         |
        | Another Description |
    Then a response with status code 403 is returned

@5322
Scenario: a buyer user cannot update an orders description for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to update the description for the order with ID 10001
        | Description         |
        | Another Description |
    Then a response with status code 403 is returned

@5322
Scenario: a user with read-only permissions, cannot update an orders description
    Given the user is logged in with the Read-only Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to update the description for the order with ID 10001
        | Description         |
        | Another Description |
    Then a response with status code 403 is returned

@5322
Scenario: service failure
    Given the call to the database will fail
    When the user makes a request to update the description for the order with ID 10001
        | Description         |
        | Another Description |
    Then a response with status code 500 is returned

@5322
Scenario: update order description to 100 characters should be successful
    When the user makes a request to update the description for the order with ID 10002
        | Description              |
        | #A string of length 100# |
    Then a response with status code 204 is returned
    And the order description for the order with ID 10002 is set to
        | Description              |
        | #A string of length 100# |
