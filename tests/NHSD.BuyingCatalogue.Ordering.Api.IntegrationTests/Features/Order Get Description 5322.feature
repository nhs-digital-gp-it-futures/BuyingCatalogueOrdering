Feature: Display the Order Description in a Buyer Section
    As a Buyer
    I want to view an order's description
    So that I can ensure that the information is correct

Background:
    Given Orders exist
        | OrderId    | Description         | OrderStatusId | Created    | LastUpdated | LastUpdatedBy                        | OrganisationId                       |
        | C000014-01 | Some Description    | 1             | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
        | C000014-02 | Another Description | 2             | 05/05/2020 | 09/05/2020  | a11a46f9-ce6f-448a-95c2-fde6e61c804a | 4af62b99-638c-4247-875e-965239cd0c48 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5322
Scenario: 1. Get an orders description
    When the user makes a request to retrieve the order description section with the ID C000014-01
    Then a response with status code 200 is returned
    And the order description is returned
        | Description      |
        | Some Description |

@5322
Scenario: 2. A non existent orderId returns not found
    When the user makes a request to retrieve the order description section with the ID INVALID
    Then a response with status code 404 is returned

@5322
Scenario: 3. If a user is not authorised then they cannot access the order description
    Given no user is logged in
    When the user makes a request to retrieve the order description section with the ID C000014-01
    Then a response with status code 401 is returned

@5322
Scenario: 4. A non buyer user cannot access the order description
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve the order description section with the ID C000014-01
    Then a response with status code 403 is returned

@5322
Scenario: 5. Service Failure
    Given the call to the database will fail
    When the user makes a request to retrieve the order description section with the ID C000014-01
    Then a response with status code 500 is returned