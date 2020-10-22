Feature: Create order items (bulk)
    As a buyer user
    I want to create order items for a given order
    So that I can complete my order

Background:
    Given Orders exist
        | OrderId    | OrganisationOdsCode | Description      | OrganisationId                       | LastUpdatedByName | LastUpdatedBy                        | CommencementDate | CatalogueSolutionsViewed |
        | C000014-01 | ODS1                | Some Description | 4af62b99-638c-4247-875e-965239cd0c48 | Tom Smith         | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 01/01/2021       | false                    |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@6036
Scenario: Create order items
    Given the user creates a request to add the following items to the order with ID 'C000014-01'
        | ItemType           | PayloadType | CatalogueItemName | OdsCode | ServiceRecipientName |
        | catalogue solution | complete    | Item A            | ODS2    | Recipient 1          |
        | catalogue solution | complete    | Item B            | ODS3    | Recipient 2          |
    When the user sends the create order items request
    Then a response with status code 201 is returned
    And the expected order items are created
    And the persisted service recipients are
        | OrderId    | OdsCode | Name        |
        | C000014-01 | ODS2    | Recipient 1 |
        | C000014-01 | ODS3    | Recipient 2 |

@6036
Scenario: Create order items and the order audit information is updated
    Given the user creates a request to add the following items to the order with ID 'C000014-01'
        | ItemType           | PayloadType | CatalogueItemName | OdsCode | ServiceRecipientName |
        | catalogue solution | complete    | Item A            | ODS2    | Recipient 1          |
        | catalogue solution | complete    | Item B            | ODS3    | Recipient 2          |
    When the user sends the create order items request
    Then the order with orderId C000014-01 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |

@6036
Scenario: Create order items with an invalid order ID
    Given the user creates a request to add the following items to the order with ID 'INVALID'
        | ItemType           | PayloadType | CatalogueItemName | OdsCode | ServiceRecipientName |
        | catalogue solution | complete    | Item A            | ODS2    | Recipient 1          |
    When the user sends the create order items request
    Then a response with status code 404 is returned

@6036
Scenario: If a user is not authorized then they cannot create order items
    Given no user is logged in
    And the user creates a request to add the following items to the order with ID 'INVALID'
        | ItemType           | PayloadType | CatalogueItemName | OdsCode | ServiceRecipientName |
        | catalogue solution | complete    | Item A            | ODS2    | Recipient 1          |
    When the user sends the create order items request
    Then a response with status code 401 is returned

@6036
Scenario: A non buyer user cannot create order items
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    And the user creates a request to add the following items to the order with ID 'INVALID'
        | ItemType           | PayloadType | CatalogueItemName | OdsCode | ServiceRecipientName |
        | catalogue solution | complete    | Item A            | ODS2    | Recipient 1          |
    When the user sends the create order items request
    Then a response with status code 403 is returned

@6036
Scenario: Service failure
    Given the call to the database will fail
    And the user creates a request to add the following items to the order with ID 'INVALID'
        | ItemType           | PayloadType | CatalogueItemName | OdsCode | ServiceRecipientName |
        | catalogue solution | complete    | Item A            | ODS2    | Recipient 1          |
    When the user sends the create order items request
    Then a response with status code 500 is returned
