Feature: Update the catalogue solutions for an order
    As a Buyer User
    I want to update the catalogue solutions for a given order
    So that I can keep an order up to date

Background:
    Given Orders exist
        | OrderId    | Description         | OrderStatusId | Created    | LastUpdated | LastUpdatedBy                        | OrganisationId                       |
        | C000014-01 | Some Description    | 1             | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4631
Scenario: 1. Update an orders catalogue solutions
    When the user makes a request to update the order catalogue solutions section with the ID C000014-01
    Then a response with status code 204 is returned
    And the order with ID C000014-01 has catalogue solutions viewed set to true
    And the order with orderId C000014-01 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with orderId C000014-01 has LastUpdated time present and it is the current time

@4631
Scenario: 2. A non existent orderId returns not found
    When the user makes a request to update the order catalogue solutions section with the ID INVALID
    Then a response with status code 404 is returned

@4631
Scenario: 3. If a user is not authorised then they cannot update the order catalogue solutions
    Given no user is logged in
    When the user makes a request to update the order catalogue solutions section with the ID C000014-01
    Then a response with status code 401 is returned

@4631
Scenario: 4. A non buyer user cannot update the order catalogue solutions
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to update the order catalogue solutions section with the ID C000014-01
    Then a response with status code 403 is returned

@4631
Scenario: 5. A buyer user cannot update the order catalogue solutions for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to update the order catalogue solutions section with the ID C000014-01
    Then a response with status code 403 is returned

@4631
Scenario: 6. Service Failure
    Given the call to the database will fail
    When the user makes a request to update the order catalogue solutions section with the ID C000014-01
    Then a response with status code 500 is returned
