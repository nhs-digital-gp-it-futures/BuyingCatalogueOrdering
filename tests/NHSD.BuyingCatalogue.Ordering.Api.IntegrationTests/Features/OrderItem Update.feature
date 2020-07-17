Feature: Update the order item for an order
    As a Buyer User
    I want to update the order item for a given order
    So that I can keep my order up to date

Background:
    Given Orders exist
        | OrderId    | Description      | LastUpdatedBy                        | LastUpdatedByName | OrganisationId                       | CommencementDate |
        | C000014-01 | Some Description | 335392e4-4bb1-413b-9de5-36a85c9c0422 | Tom Smith         | 4af62b99-638c-4247-875e-965239cd0c48 | 01/01/2021       |
    And Service Recipients exist
        | OdsCode | Name                | OrderId    |
        | ODS1    | Service Recipient   | C000014-01 |
        | ODS2    | Service Recipient 2 | C000014-01 |
    And Order items exist
        | OrderId    | OdsCode | CatalogueItemName | CatalogueItemType | ProvisioningType |
        | C000014-01 | ODS1    | Order Item 1      | Solution          | OnDemand         |
        | C000014-01 | ODS2    | Order Item 2      | AdditionalService | Declarative      |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@7840
Scenario: 1. Update an order item
    Given the user creates a request to change the order item ('Order Item 1') for the order with ID 'C000014-01'
    And the user enters the '<payload-type>' update order item request payload
    When the user sends the update order item request
    Then a response with status code 204 is returned
    And the order item is updated

    Examples: Request payloads
        | payload-type  |
        | complete      |
        | high-boundary |
        | low-boundary  |

@7840
Scenario: 2. Update an order item that is not on demand does not update the estimation period
    Given the user creates a request to change the order item ('Order Item 2') for the order with ID 'C000014-01'
    And the user enters the 'missing-estimation-period' update order item request payload
    When the user sends the update order item request
    Then a response with status code 204 is returned
    And the order item is updated

@7840
Scenario: 3. Update a order item and the order audit information is updated
    Given the user creates a request to change the order item ('Order Item 1') for the order with ID 'C000014-01'
    And the user enters the 'complete' update order item request payload
    When the user sends the update order item request
    Then the order with orderId C000014-01 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |

@7840
Scenario: 4. Update a order item with invalid order ID should return not found
    Given the user creates a request to change the order item ('Order Item 1') for the order with ID 'INVALID'
    And the user enters the 'complete' update order item request payload
    When the user sends the update order item request
    Then a response with status code 404 is returned

@7840
Scenario: 5. If a user is not authorised then they cannot update a order item
    Given no user is logged in
    And the user creates a request to change the order item ('Order Item 1') for the order with ID 'C000014-01'
    And the user enters the 'complete' update order item request payload
    When the user sends the update order item request
    Then a response with status code 401 is returned

@7840
Scenario: 6. A non buyer user cannot update a order item
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    And the user creates a request to change the order item ('Order Item 1') for the order with ID 'C000014-01'
    And the user enters the 'complete' update order item request payload
    When the user sends the update order item request
    Then a response with status code 403 is returned

@7840
Scenario: 7. A buyer user cannot update a order item for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    And the user creates a request to change the order item ('Order Item 1') for the order with ID 'C000014-01'
    And the user enters the 'complete' update order item request payload
    When the user sends the update order item request
    Then a response with status code 403 is returned

@7840
Scenario: 8. Service Failure
    Given the call to the database will fail
    And the user creates a request to change the order item ('Order Item 1') for the order with ID 'C000014-01'
    And the user enters the 'complete' update order item request payload
    When the user sends the update order item request
    Then a response with status code 500 is returned
