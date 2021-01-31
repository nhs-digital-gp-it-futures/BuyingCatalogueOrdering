Feature: Update order items (bulk)
    As a buyer user
    I want to update order items for a given order
    So that I can complete my order

Background:
    Given Orders exist
        | OrderId    | Description      | LastUpdatedBy                        | LastUpdatedByName | OrganisationId                       | CommencementDate |
        | C000014-01 | Some Description | 335392e4-4bb1-413b-9de5-36a85c9c0422 | Tom Smith         | 4af62b99-638c-4247-875e-965239cd0c48 | 01/01/2021       |
    And Service Recipients exist
        | OdsCode | Name        | OrderId    |
        | ODS1    | Recipient 1 | C000014-01 |
        | ODS2    | Recipient 2 | C000014-01 |
        | ODS3    | Recipient 3 | C000014-01 |
    And Order items exist
        | OrderId    | OdsCode | CatalogueItemName | CatalogueItemType | ProvisioningType |
        | C000014-01 | ODS1    | Item A            | Solution          | OnDemand         |
        | C000014-01 | ODS2    | Item B            | AdditionalService | Declarative      |
        | C000014-01 | ODS3    | Item C            | AssociatedService | Patient          |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@10516
Scenario: Edit all order items
    Given the user creates a request to update the order with ID 'C000014-01' with the following items
        | OrderItemId | ItemType           | PayloadType    | CatalogueItemName | OdsCode | ServiceRecipientName |
        | 1           | catalogue solution | complete       | Item A            | ODS1    | Recipient 1          |
        | 2           | additional service | high-boundary  | Item B            | ODS2    | Recipient 2          |
        | 3           | associated service | low-boundary   | Item C            | ODS3    | Recipient 3          |
    When the user sends the edit order items request
    Then a response with status code 204 is returned
    And the expected order items exist
    And the persisted service recipients are
        | OrderId    | OdsCode | Name        |
        | C000014-01 | ODS1    | Recipient 1 |
        | C000014-01 | ODS2    | Recipient 2 |
        | C000014-01 | ODS3    | Recipient 3 |

@10516
Scenario: Add an order item
    Given the user creates a request to update the order with ID 'C000014-01' with the following items
        | OrderItemId | ItemType           | PayloadType | CatalogueItemName | OdsCode | ServiceRecipientName |
        |             | catalogue solution | complete    | Item D            | ODS4    | Recipient 4          |
    When the user sends the edit order items request
    Then a response with status code 204 is returned
    And the following order items exist
        | OrderItemId | OrderId    | OdsCode | CatalogueItemName | CatalogueItemType |
        | 1           | C000014-01 | ODS1    | Item A            | Solution          |
        | 2           | C000014-01 | ODS2    | Item B            | AdditionalService |
        | 3           | C000014-01 | ODS3    | Item C            | AssociatedService |
        | 4           | C000014-01 | ODS4    | Item D            | Solution          |
    And the persisted service recipients are
        | OrderId    | OdsCode | Name        |
        | C000014-01 | ODS1    | Recipient 1 |
        | C000014-01 | ODS2    | Recipient 2 |
        | C000014-01 | ODS3    | Recipient 3 |
        | C000014-01 | ODS4    | Recipient 4 |

@10516
Scenario: Edit order items and the order audit information is updated
    Given the user creates a request to update the order with ID 'C000014-01' with the following items
        | OrderItemId | ItemType           | PayloadType | CatalogueItemName | OdsCode | ServiceRecipientName |
        | 1           | catalogue solution | complete    | Item A            | ODS2    | Recipient 1          |
        | 2           | catalogue solution | complete    | Item B            | ODS3    | Recipient 2          |
    When the user sends the edit order items request
    Then the order with orderId C000014-01 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |

@10516
Scenario: Edit order items with an invalid order ID
    Given the user creates a request to update the order with ID 'INVALID' with the following items
        | OrderItemId | ItemType           | PayloadType | CatalogueItemName | OdsCode | ServiceRecipientName |
        | 1           | catalogue solution | complete    | Item A            | ODS2    | Recipient 1          |
    When the user sends the edit order items request
    Then a response with status code 404 is returned

@10516
Scenario: If a user is not authorized then they cannot edit order items
    Given no user is logged in
    And the user creates a request to update the order with ID 'C000014-01' with the following items
        | OrderItemId | ItemType           | PayloadType | CatalogueItemName | OdsCode | ServiceRecipientName |
        | 1           | catalogue solution | complete    | Item A            | ODS2    | Recipient 1          |
    When the user sends the edit order items request
    Then a response with status code 401 is returned

@10516
Scenario: A non buyer user cannot edit order items
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    And the user creates a request to update the order with ID 'C000014-01' with the following items
        | OrderItemId | ItemType           | PayloadType | CatalogueItemName | OdsCode | ServiceRecipientName |
        | 1           | catalogue solution | complete    | Item A            | ODS2    | Recipient 1          |
    When the user sends the edit order items request
    Then a response with status code 403 is returned

@10516
Scenario: Service failure
    Given the call to the database will fail
    And the user creates a request to update the order with ID 'C000014-01' with the following items
        | OrderItemId | ItemType           | PayloadType | CatalogueItemName | OdsCode | ServiceRecipientName |
        | 1           | catalogue solution | complete    | Item A            | ODS2    | Recipient 1          |
    When the user sends the edit order items request
    Then a response with status code 500 is returned
