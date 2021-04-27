Feature: funding source PUT
    As a buyer
    I want to  update the funding source for an order
    So that I can ensure the information is correct

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | Description      | OrderingPartyId                      | Created    |
        | 10001   | Some Description | 4af62b99-638c-4247-875e-965239cd0c48 | 10/03/2021 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5437
Scenario: update funding source
    Given the user creates a request to update the funding source for the order with ID 10001
    And the user enters the '<FundingSourcePayload>' update funding source request payload
    When the user sends the update funding source request
    Then a response with status code 204 is returned
    And the funding source is set correctly
    And the order with ID 10001 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with ID 10001 has LastUpdated time present and it is the current time
Examples:
    | FundingSourcePayload |
    | funding-source-true  |
    | funding-source-false |

@5437
Scenario: validate funding source
    Given the user creates a request to update the funding source for the order with ID 10001
    And the user enters the 'funding-source-missing' update funding source request payload
    When the user sends the update funding source request
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id              | field   |
        | OnlyGMSRequired | OnlyGms |

@5437
Scenario: if a user is not authorised, then they cannot update the orders funding source
    Given no user is logged in
    And the user creates a request to update the funding source for the order with ID 10001
    And the user enters the 'funding-source-true' update funding source request payload
    When the user sends the update funding source request
    Then a response with status code 401 is returned

@5437
Scenario: a non-buyer user cannot update an orders funding source
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    And the user creates a request to update the funding source for the order with ID 10001
    And the user enters the 'funding-source-true' update funding source request payload
    When the user sends the update funding source request
    Then a response with status code 403 is returned

@5437
Scenario: a buyer user cannot update an orders funding source for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    And the user creates a request to update the funding source for the order with ID 10001
    And the user enters the 'funding-source-true' update funding source request payload
    When the user sends the update funding source request
    Then a response with status code 403 is returned

@5437
Scenario: a user with read only permissions, cannot update an orders funding source
    Given the user is logged in with the Read-only Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    And the user creates a request to update the funding source for the order with ID 10001
    And the user enters the 'funding-source-true' update funding source request payload
    When the user sends the update funding source request
    Then a response with status code 403 is returned

@5437
Scenario: service failure
    Given the call to the database will fail
    And the user creates a request to update the funding source for the order with ID 10001
    And the user enters the 'funding-source-true' update funding source request payload
    When the user sends the update funding source request
    Then a response with status code 500 is returned
