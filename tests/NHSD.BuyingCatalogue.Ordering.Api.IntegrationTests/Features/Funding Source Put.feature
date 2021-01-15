Feature: Update the funding source for an order
    As a Buyer
    I want to  update the funding source for an order
    So that I can ensure the information is correct

Background:
    Given Orders exist
        | OrderId    | Description         | OrganisationId                       |
        | C000014-01 | Some Description    | 4af62b99-638c-4247-875e-965239cd0c48 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5437
Scenario: 1. Update funding source
    Given the user creates a request to update the funding source for the order with ID 'C000014-01'
    And the user enters the '<FundingSourcePayload>' update funding source request payload
    When the user sends the update funding source request
    Then a response with status code 204 is returned
    And the funding source is set correctly
    And the order with orderId C000014-01 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with orderId C000014-01 has LastUpdated time present and it is the current time
Examples:
    | FundingSourcePayload |
    | funding-source-true  |
    | funding-source-false |

@5437
Scenario: 2. Validate funding source
    Given the user creates a request to update the funding source for the order with ID 'C000014-01'
    And the user enters the 'funding-source-missing' update funding source request payload
    When the user sends the update funding source request
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id              | field   |
        | OnlyGMSRequired | OnlyGms |

@5437
Scenario: 3. If a user is not authorised, then they cannot update the orders funding source
    Given no user is logged in
    Given the user creates a request to update the funding source for the order with ID 'C000014-01'
    And the user enters the 'funding-source-true' update funding source request payload
    When the user sends the update funding source request
    Then a response with status code 401 is returned

@5437
Scenario: 4. A non buyer user cannot update an orders funding source
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    Given the user creates a request to update the funding source for the order with ID 'C000014-01'
    And the user enters the 'funding-source-true' update funding source request payload
    When the user sends the update funding source request
    Then a response with status code 403 is returned

@5437
Scenario: 5. A buyer user cannot update an orders funding source for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    Given the user creates a request to update the funding source for the order with ID 'C000014-01'
    And the user enters the 'funding-source-true' update funding source request payload
    When the user sends the update funding source request
    Then a response with status code 403 is returned

@5437
Scenario: 6. A user with read only permissions, cannot update an orders funding source
    Given the user is logged in with the Readonly-Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    Given the user creates a request to update the funding source for the order with ID 'C000014-01'
    And the user enters the 'funding-source-true' update funding source request payload
    When the user sends the update funding source request
    Then a response with status code 403 is returned

@5437
Scenario: 7. Service Failure
    Given the call to the database will fail
    Given the user creates a request to update the funding source for the order with ID 'C000014-01'
    And the user enters the 'funding-source-true' update funding source request payload
    When the user sends the update funding source request
    Then a response with status code 500 is returned
