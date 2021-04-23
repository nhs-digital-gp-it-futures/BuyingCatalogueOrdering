Feature: order item DELETE
    As a buyer user
    I want to update the order items for a given order
    So that I can keep my order up to date

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    Given orders exist
        | OrderId | Description      | OrderingPartyId                      | LastUpdatedBy                        | LastUpdatedByName | CommencementDate | FundingSourceOnlyGMS |
        | 10001   | Some Description | 4af62b99-638c-4247-875e-965239cd0c48 | 335392e4-4bb1-413b-9de5-36a85c9c0422 | Tom Smith         | 01/01/2021       | True                 |
    And pricing units exist
        | Name    | Description |
        | patient | per patient |
    And service recipients exist
        | OdsCode | Name                |
        | ODS1    | Service Recipient 1 |
        | ODS2    | Service Recipient 2 |
        | ODS3    | Service Recipient 3 |
    And catalogue items exist
        | Id             | Name   | CatalogueItemType | ParentCatalogueItemId |
        | 100001-001     | Item 1 | Solution          | NULL                  |
        | 200002-002     | Item 2 | Solution          | NULL                  |
        | 100001-001A001 | Item 3 | AdditionalService | 100001-001            |
    And order items exist
        | OrderId | CatalogueItemId |
        | 10001   | 100001-001      |
        | 10001   | 200002-002      |
        | 10001   | 100001-001A001  |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 100001-001      | ODS1    |
        | 10001   | 100001-001      | ODS2    |
        | 10001   | 200002-002      | ODS2    |
        | 10001   | 100001-001A001  | ODS1    |
        | 10001   | 100001-001A001  | ODS3    |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@9038
Scenario: delete a solution
    Given the user creates a request to delete order item with catalogue item ID 200002-002 for the order with ID 10001
    When the user sends the delete order item request
    Then a response with status code 204 is returned
    And the following order items exist for the order with ID 10001
        | OrderId | CatalogueItemId |
        | 10001   | 100001-001      |
        | 10001   | 100001-001A001  |
    And the following order item recipients exist for the order with ID 10001
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 100001-001      | ODS1    |
        | 10001   | 100001-001      | ODS2    |
        | 10001   | 100001-001A001  | ODS1    |
        | 10001   | 100001-001A001  | ODS3    |

@9038
Scenario: deleting all order items should set funding source to null
    Given the user creates a request to delete order item with catalogue item ID 100001-001 for the order with ID 10001
    When the user sends the delete order item request
    Given the user creates a request to delete order item with catalogue item ID 200002-002 for the order with ID 10001
    When the user sends the delete order item request
    Then the fundingSourceOnlyGms value is null

@9040
Scenario: delete a catalogue solution with an additional service
    Given the user creates a request to delete order item with catalogue item ID 100001-001 for the order with ID 10001
    When the user sends the delete order item request
    Then a response with status code 204 is returned
    And the following order items exist for the order with ID 10001
        | OrderId | CatalogueItemId |
        | 10001   | 200002-002      |
    And the following order item recipients exist for the order with ID 10001
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 200002-002      | ODS2    |

@9038
Scenario: delete a catalogue item that does not exist
    Given the user creates a request to delete order item with catalogue item ID 234567-890 for the order with ID 10001
    When the user sends the delete order item request
    Then a response with status code 404 is returned
    And the following order items exist for the order with ID 10001
        | OrderId | CatalogueItemId |
        | 10001   | 100001-001      |
        | 10001   | 200002-002      |
        | 10001   | 100001-001A001  |
    And the following order item recipients exist for the order with ID 10001
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 100001-001      | ODS1    |
        | 10001   | 100001-001      | ODS2    |
        | 10001   | 200002-002      | ODS2    |
        | 10001   | 100001-001A001  | ODS1    |
        | 10001   | 100001-001A001  | ODS3    |

@9038
Scenario: if a user is not authorized then they cannot delete a catalogue item
    Given no user is logged in
    And the user creates a request to delete order item with catalogue item ID 200002-002 for the order with ID 10001
    When the user sends the delete order item request
    Then a response with status code 401 is returned

@9038
Scenario: a non-buyer user cannot delete a catalogue item
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    And the user creates a request to delete order item with catalogue item ID 200002-002 for the order with ID 10001
    When the user sends the delete order item request
    Then a response with status code 403 is returned

@9038
Scenario: a buyer user cannot delete a catalogue item for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    And the user creates a request to delete order item with catalogue item ID 200002-002 for the order with ID 10001
    When the user sends the delete order item request
    Then a response with status code 403 is returned

@9038
Scenario: service failure
    Given the call to the database will fail
    And the user creates a request to delete order item with catalogue item ID 200002-002 for the order with ID 10001
    When the user sends the delete order item request
    Then a response with status code 500 is returned
