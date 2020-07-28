Feature: Funding Source Get
    As a Buyer User
    I want to view the funding source details for an order
    So that I can ensure that the information is correct

Background:
    Given Orders exist
        | OrderId    | Description      | OrganisationId                       | FundingSourceOnlyGMS |
        | C000014-01 | Some Description | 4af62b99-638c-4247-875e-965239cd0c48 | NULL                 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5437
Scenario: 1. Get the funding source details
    Given Orders exist
        | OrderId    | Description      | OrganisationId                       | FundingSourceOnlyGMS   |
        | C000015-01 | Some Description | 4af62b99-638c-4247-875e-965239cd0c48 | <FundingSourceOnlyGMS> |
    And the user creates a request to retrieve the funding source for order with ID 'C000015-01'
    When the user sends the retrieve funding source request
    Then a response with status code 200 is returned
    And the response contains the expected funding source details

    Examples:
        | FundingSourceOnlyGMS |
        | NULL                 |
        | True                 |
        | False                |

@5437
Scenario: 2. Get the funding source details for an order that does not exist
    Given the user creates a request to retrieve the funding source for order with ID 'DOESNOTEXIST'
    When the user sends the retrieve funding source request
    Then a response with status code 404 is returned

@5437
Scenario: 3. Get the funding source details by a user who is not authorised
    Given no user is logged in
    And the user creates a request to retrieve the funding source for order with ID 'C000014-01'
    When the user sends the retrieve funding source request
    Then a response with status code 401 is returned

@5437
Scenario: 4. Get the funding source details by a non buyer user
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    And the user creates a request to retrieve the funding source for order with ID 'C000014-01'
    When the user sends the retrieve funding source request
    Then a response with status code 403 is returned

@5437
Scenario: 5. Get the funding source details by a user who belongs to a different organisation
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    And the user creates a request to retrieve the funding source for order with ID 'C000014-01'
    When the user sends the retrieve funding source request
    Then a response with status code 403 is returned

@5437
Scenario: 6. Get the funding source details when the database is down
    Given the call to the database will fail
    And the user creates a request to retrieve the funding source for order with ID 'C000014-01'
    When the user sends the retrieve funding source request
    Then a response with status code 500 is returned
