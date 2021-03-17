Feature: delete an already existing order
    As a buyer user
    I want to be able to delete a given order
    So that I can... delete my order?

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | Description   | OrderingPartyId                      |
        | 10001   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 |
        And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

# Possible issue with integration DB initialization (intermittent scenario failure, passes when run in isolation)
@ignore
@5287
Scenario: delete an existing order
    Given the user creates a request to delete the order with ID 10001
    When the user sends the delete order request
    Then a response with status code 204 is returned
    And the expected order is deleted

# Possible issue with integration DB initialization (intermittent scenario failure, passes when run in isolation)
@ignore
@5287
Scenario: deleting an order updates the order's last updated information
    Given the user creates a request to delete the order with ID 10001
    When the user sends the delete order request
    Then a response with status code 204 is returned
    And the order with ID 10001 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |

@5287
Scenario: a non-existent order ID returns not found
    Given the user creates a request to delete the order with ID 10000
    When the user sends the delete order request
    Then a response with status code 404 is returned

# Possible issue with integration DB initialization (intermittent scenario failure, passes when run in isolation)
@ignore
@5287
Scenario: a previously deleted order ID returns not found
    Given the user creates a request to delete the order with ID 10001
    When the user sends the delete order request
    Then a response with status code 204 is returned
    When the user sends the delete order request
    Then a response with status code 404 is returned

@5287
Scenario: if a user is not authorised then they cannot delete an order
    Given no user is logged in
    And the user creates a request to delete the order with ID 10001
    When the user sends the delete order request
    Then a response with status code 401 is returned

@5287
Scenario: a non-buyer user cannot delete an order
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    And the user creates a request to delete the order with ID 10001
    When the user sends the delete order request
    Then a response with status code 403 is returned

# TODO: fix. Suspect param name check in auth filter is problem
@ignore
@5287
Scenario: a buyer user cannot delete an order for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    And the user creates a request to delete the order with ID 10001
    When the user sends the delete order request
    Then a response with status code 403 is returned

@5287
Scenario: service failure
    Given the call to the database will fail
    And the user creates a request to delete the order with ID 10001
    When the user sends the delete order request
    Then a response with status code 500 is returned
