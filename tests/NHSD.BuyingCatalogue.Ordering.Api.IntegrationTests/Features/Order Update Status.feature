Feature: Order Status Update
    As a Buyer User
    I want to be able to update the status of an order
    So that I can ensure that the information is kept up to date

Background:
    Given Orders exist
        | OrderId    | Description      | OrganisationId                       |
        | C000014-01 | Some Description | 4af62b99-638c-4247-875e-965239cd0c48 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5145
Scenario: 1. Update an order status
    Given the user creates a request to update the order status for the order with ID 'C000014-01'
    And the user enters the '<StatusPayload>' update order status request payload
    When the user sends the update order status request
    Then a response with status code 204 is returned
    And the order status is set correctly
    And the order with orderId C000014-01 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with orderId C000014-01 has LastUpdated time present and it is the current time

    Examples:
        | StatusPayload           |
        | order-status-complete   |
        | order-status-incomplete |

@5145
Scenario: 2. An invalid order status provides an appropriate error message
    Given the user creates a request to update the order status for the order with ID 'C000014-01'
    And the user enters the '<StatusPayload>' update order status request payload
    When the user sends the update order status request
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id        | field        |
        | <ErrorId> | <ErrorField> |

    Examples:
        | StatusPayload        | ErrorId        | ErrorField |
        | order-status-missing | StatusRequired | Status     |
        | order-status-invalid | InvalidStatus  | Status     |
