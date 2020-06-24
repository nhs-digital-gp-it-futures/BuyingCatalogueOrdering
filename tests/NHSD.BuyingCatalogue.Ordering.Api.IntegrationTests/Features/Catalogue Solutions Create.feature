Feature: Create catalogue solution order item
    As a Buyer User
    I want to create a catalogue solution order item for a given order
    So that I can complete my order

Background:
    Given Orders exist
        | OrderId    | Description      | OrganisationId                       |
        | C000014-01 | Some Description | 4af62b99-638c-4247-875e-965239cd0c48 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@7840
Scenario: 1. Create catalogue solution order item
    Given the user creates a request to add a new catalogue solution order item to the order with ID 'C000014-01'
    And the user enters '<payload-type>' payload
    When the user sends the request
    Then a response with status code 201 is returned
    And the expected catalogue solution order item is created

    Examples: Request payloads
        | payload-type |
        | complete     |

@7840
Scenario: 2. Create catalogue solution order item with invalid order ID
    Given the user creates a request to add a new catalogue solution order item to the order with ID 'INVALID'
    And the user enters 'complete' payload
    When the user sends the request
    Then a response with status code 404 is returned

@7840
Scenario: 3. If a user is not authorised then they cannot create a catalogue solution order item
    Given no user is logged in
    And the user creates a request to add a new catalogue solution order item to the order with ID 'C000014-01'
    And the user enters 'complete' payload
    When the user sends the request
    Then a response with status code 401 is returned

@7840
Scenario: 4. A non buyer user cannot create a catalogue solution order item
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    And the user creates a request to add a new catalogue solution order item to the order with ID 'C000014-01'
    And the user enters 'complete' payload
    When the user sends the request
    Then a response with status code 403 is returned

@7840
Scenario: 5. A buyer user cannot create a catalogue solution order item for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    And the user creates a request to add a new catalogue solution order item to the order with ID 'C000014-01'
    And the user enters 'complete' payload
    When the user sends the request
    Then a response with status code 403 is returned

@7840
Scenario: 6. Service Failure
    Given the call to the database will fail
    And the user creates a request to add a new catalogue solution order item to the order with ID 'C000014-01'
    And the user enters 'complete' payload
    When the user sends the request
    Then a response with status code 500 is returned

@7840
Scenario: 7. Create catalogue solution order item without a body
    Given the user creates a request to add a new catalogue solution order item to the order with ID 'C000014-01'
    When the user sends the request
    Then a response with status code 400 is returned
