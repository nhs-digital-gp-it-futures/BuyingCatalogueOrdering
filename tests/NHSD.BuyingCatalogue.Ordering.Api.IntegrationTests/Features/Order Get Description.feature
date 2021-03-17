Feature: display the order description in a buyer section
    As a buyer
    I want to view an order's description
    So that I can ensure that the information is correct

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    Given orders exist
        | OrderId | Description         | OrderingPartyId                      |
        | 10001   | Some Description    | 4af62b99-638c-4247-875e-965239cd0c48 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5322
Scenario: get an order's description
    When the user makes a request to retrieve the order description section with the ID 10001
    Then a response with status code 200 is returned
    And the order description is returned
        | Description      |
        | Some Description |

@5322
Scenario: a non-existent order ID returns not found
    When the user makes a request to retrieve the order description section with the ID 10000
    Then a response with status code 404 is returned

@5322
Scenario: if a user is not authorised then they cannot access the order description
    Given no user is logged in
    When the user makes a request to retrieve the order description section with the ID 10001
    Then a response with status code 401 is returned

@5322
Scenario: A non-buyer user cannot access the order description
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve the order description section with the ID 10001
    Then a response with status code 403 is returned

@5322
Scenario: a buyer user cannot access the order description for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve the order description section with the ID 10001
    Then a response with status code 403 is returned

@5322
Scenario: service failure
    Given the call to the database will fail
    When the user makes a request to retrieve the order description section with the ID 10001
    Then a response with status code 500 is returned
