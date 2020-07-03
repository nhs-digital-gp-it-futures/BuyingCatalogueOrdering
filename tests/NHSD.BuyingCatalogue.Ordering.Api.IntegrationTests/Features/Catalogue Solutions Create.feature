Feature: Create catalogue solution order item
    As a Buyer User
    I want to create a catalogue solution order item for a given order
    So that I can complete my order

Background:
    Given Orders exist
        | OrderId    | Description      | OrganisationId                       | LastUpdatedByName | LastUpdatedBy                        | CommencementDate |
        | C000014-01 | Some Description | 4af62b99-638c-4247-875e-965239cd0c48 | Tom Smith         | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 01/01/2021       |
    And Service Recipients exist
        | OdsCode | Name              | OrderId    |
        | ODS1    | Service Recipient | C000014-01 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@7840
Scenario: 1. Create catalogue solution order item
    Given the user creates a request to add a new catalogue solution order item to the order with ID 'C000014-01'
    And the user enters the '<payload-type>' create catalogue solution order item request payload
    When the user sends the create catalogue solution order item request
    Then a response with status code 201 is returned
    And the expected catalogue solution order item is created

    Examples: Request payloads
        | payload-type |
        | complete     |

@7840
Scenario: 2. Create catalogue solution order item and the new order item ID is returned
    Given the user creates a request to add a new catalogue solution order item to the order with ID 'C000014-01'
    And the user enters the 'complete' create catalogue solution order item request payload
    When the user sends the create catalogue solution order item request
    And the response contains the new order item ID

@7840
Scenario: 3. Create catalogue solution order item and the order audit information is updated
    Given the user creates a request to add a new catalogue solution order item to the order with ID 'C000014-01'
    And the user enters the 'complete' create catalogue solution order item request payload
    When the user sends the create catalogue solution order item request
    Then the order with orderId C000014-01 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |

@7840
Scenario: 4. Create catalogue solution order item with invalid order ID
    Given the user creates a request to add a new catalogue solution order item to the order with ID 'INVALID'
    And the user enters the 'complete' create catalogue solution order item request payload
    When the user sends the create catalogue solution order item request
    Then a response with status code 404 is returned

@7840
Scenario: 5. If a user is not authorised then they cannot create a catalogue solution order item
    Given no user is logged in
    And the user creates a request to add a new catalogue solution order item to the order with ID 'C000014-01'
    And the user enters the 'complete' create catalogue solution order item request payload
    When the user sends the create catalogue solution order item request
    Then a response with status code 401 is returned

@7840
Scenario: 6. A non buyer user cannot create a catalogue solution order item
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    And the user creates a request to add a new catalogue solution order item to the order with ID 'C000014-01'
    And the user enters the 'complete' create catalogue solution order item request payload
    When the user sends the create catalogue solution order item request
    Then a response with status code 403 is returned

@7840
Scenario: 7. A buyer user cannot create a catalogue solution order item for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    And the user creates a request to add a new catalogue solution order item to the order with ID 'C000014-01'
    And the user enters the 'complete' create catalogue solution order item request payload
    When the user sends the create catalogue solution order item request
    Then a response with status code 403 is returned

@7840
Scenario: 8. Service Failure
    Given the call to the database will fail
    And the user creates a request to add a new catalogue solution order item to the order with ID 'C000014-01'
    And the user enters the 'complete' create catalogue solution order item request payload
    When the user sends the create catalogue solution order item request
    Then a response with status code 500 is returned
