Feature: Delete an already existing order
    As an Buyer user
    I want to be able to delete a given order
    So that I can... delete my order?

Background:
    Given Orders exist
        | OrderId    | Description   | OrganisationId                       | OrganisationName | OrganisationOdsCode | OrganisationContactEmail | SupplierAddressPostcode | SupplierContactEmail    |
        | C000014-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | Hampshire CC     | 432432              | Fred.robinson@email.com  | LS 1 3AP                | Fred.robinson@email.com |
        And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5287
Scenario: Delete an existing order
    Given the user creates a request to delete the order with ID 'C000014-01'
    When the user sends the delete order request
    Then a response with status code 204 is returned
    And the expected order is deleted

@5287
Scenario: Deleting an order updates the order's last updated information
    Given the user creates a request to delete the order with ID 'C000014-01'
    When the user sends the delete order request
    Then a response with status code 204 is returned
    And the order with orderId C000014-01 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |

@5287
Scenario: A non existent order ID returns not found
    Given the user creates a request to delete the order with ID 'INVALID'
    When the user sends the delete order request
    Then a response with status code 404 is returned

@5287
Scenario: A previously deleted order ID returns not found
    Given the user creates a request to delete the order with ID 'C000014-01'
    When the user sends the delete order request
    Then a response with status code 204 is returned
    When the user sends the delete order request
    Then a response with status code 404 is returned

@5287
Scenario: If a user is not authorised then they cannot delete an order
    Given no user is logged in
    Given the user creates a request to delete the order with ID 'C000014-01'
    When the user sends the delete order request
    Then a response with status code 401 is returned

@5287
Scenario: A non buyer user cannot delete an order
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    Given the user creates a request to delete the order with ID 'C000014-01'
    When the user sends the delete order request
    Then a response with status code 403 is returned

@5287
Scenario: A buyer user cannot delete an order for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    Given the user creates a request to delete the order with ID 'C000014-01'
    When the user sends the delete order request
    Then a response with status code 403 is returned

@5287
Scenario: Service Failure
    Given the call to the database will fail
    Given the user creates a request to delete the order with ID 'C000014-01'
    When the user sends the delete order request
    Then a response with status code 500 is returned
