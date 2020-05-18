Feature: Display all of the Orders in an Authority Section
    As an Authority User
    I want to view all of the orders for a given organisation
    So that I can ensure that the information is correct

Background:
    Given Orders exist
        | OrderId    | Description          | OrderStatusId | Created    | LastUpdated | LastUpdatedBy                        | OrganisationId                       |
        | C000014-01 | Some Description     | 1             | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
        | C000014-02 | Another Description  | 2             | 05/05/2020 | 09/05/2020  | a11a46f9-ce6f-448a-95c2-fde6e61c804a | 4af62b99-638c-4247-875e-965239cd0c48 |
        | C000014-03 | One more Description | 2             | 15/05/2020 | 19/05/2020  | a11a46f9-ce6f-448a-95c2-fde6e61c804a | e6ea864e-ef1b-41aa-a4d5-04fc6fce0933 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4826
Scenario: 1. Get all of the orders from an existing organisationId
    When a GET request is made for a list of orders with organisationId 4af62b99-638c-4247-875e-965239cd0c48
    Then a response with status code 200 is returned
    And the orders list is returned with the following values
        | OrderId    | Description         | Status      | Created    | LastUpdated | LastUpdatedBy                        |
        | C000014-01 | Some Description    | Submitted   | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 |
        | C000014-02 | Another Description | Unsubmitted | 05/05/2020 | 09/05/2020  | a11a46f9-ce6f-448a-95c2-fde6e61c804a |

@4826
Scenario: 2. Get all of the orders from an invalid organisationId
    When a GET request is made for a list of orders with organisationId 3a72c6ab-0be8-4faa-8cb0-3b2c1f077eeb
    Then a response with status code 200 is returned
    And an empty list is returned

@4826
Scenario: 3. If a user is not authorised then they cannot access the orders
    Given no user is logged in
    When a GET request is made for a list of orders with organisationId 4af62b99-638c-4247-875e-965239cd0c48
    Then a response with status code 401 is returned

@4826
Scenario: 4. A non buyer user cannot access the orders
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When a GET request is made for a list of orders with organisationId 4af62b99-638c-4247-875e-965239cd0c48
    Then a response with status code 403 is returned

@4826
Scenario: 5. Service Failure
    Given the call to the database will fail
    When a GET request is made for a list of orders with organisationId 4af62b99-638c-4247-875e-965239cd0c48
    Then a response with status code 500 is returned
