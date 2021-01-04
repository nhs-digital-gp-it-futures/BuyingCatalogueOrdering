Feature: Service Instance ID
    As a buyer user
    I want to view the service instance ID for each item of an order
    So that the contractual requirements of my order are met

Background:
    Given Orders exist
        | OrderId    | IsDeleted | Description      | LastUpdatedBy                        | OrganisationId                       |
        | C000014-01 | false     | Order 1          | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
        | C000014-02 | false     | Order 2          | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    Given Service Recipients exist
        | OrderId    | OdsCode | Name             |
        | C000014-01 | A12345  | Recipient A12345 |
        | C000014-01 | B54321  | Recipient B54321 |
        | C000014-01 | C67890  | Recipient C67890 |
        | C000014-02 | A12345  | Recipient A12345 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@10039
Scenario: Each solution for a particular recipient has its own service instance ID
    Given Order items exist
        | OrderId    | CatalogueItemId | ParentCatalogueItemId | CatalogueItemType | OdsCode |
        | C000014-01 | 10000-001       | NULL                  | Solution          | A12345  |
        | C000014-01 | 10000-002       | NULL                  | Solution          | A12345  |
    When the user makes a request to retrieve a list of order items for the order with ID C000014-01
    And the user sends the retrieve a list of order items request
    Then a response with status code 200 is returned
    And each order item has the expected service instance ID as follows
        | OrderItemId | ServiceInstanceId |
        | 1           | SI1-A12345        |
        | 2           | SI2-A12345        |

@10039
Scenario: Each service recipient of a solution has its own service instance ID
    Given Order items exist
        | OrderId    | CatalogueItemId | ParentCatalogueItemId | CatalogueItemType | OdsCode |
        | C000014-01 | 10000-001       | NULL                  | Solution          | A12345  |
        | C000014-01 | 10000-001       | NULL                  | Solution          | B54321  |
    When the user makes a request to retrieve a list of order items for the order with ID C000014-01
    And the user sends the retrieve a list of order items request
    Then a response with status code 200 is returned
    And each order item has the expected service instance ID as follows
        | OrderItemId | ServiceInstanceId |
        | 1           | SI1-A12345        |
        | 2           | SI1-B54321        |

@10511
Scenario: An additional service ordered for the same recipient as its parent solution shares the solution service instance ID
    Given Order items exist
        | OrderId    | CatalogueItemId | ParentCatalogueItemId | CatalogueItemType | OdsCode |
        | C000014-01 | 10000-001       | NULL                  | Solution          | A12345  |
        | C000014-01 | 10000-001-A01   | 10000-001             | AdditionalService | A12345  |
    When the user makes a request to retrieve a list of order items for the order with ID C000014-01
    And the user sends the retrieve a list of order items request
    Then a response with status code 200 is returned
    And each order item has the expected service instance ID as follows
        | OrderItemId | ServiceInstanceId |
        | 1           | SI1-A12345        |
        | 2           | SI1-A12345        |

@10511
Scenario: An additional service ordered for a recipient that is not receiving its parent solution does not have a service instance ID
    Given Order items exist
        | OrderId    | CatalogueItemId | ParentCatalogueItemId | CatalogueItemType | OdsCode |
        | C000014-01 | 10000-001       | NULL                  | Solution          | A12345  |
        | C000014-01 | 10000-001-A01   | 10000-001             | AdditionalService | B54321  |
    When the user makes a request to retrieve a list of order items for the order with ID C000014-01
    And the user sends the retrieve a list of order items request
    Then a response with status code 200 is returned
    And each order item has the expected service instance ID as follows
        | OrderItemId | ServiceInstanceId |
        | 1           | SI1-A12345        |
        | 2           | NULL              |

@10039
Scenario: An associated service does not have a service instance ID
    Given Order items exist
        | OrderId    | CatalogueItemId | ParentCatalogueItemId | CatalogueItemType | OdsCode |
        | C000014-01 | 10000-001       | NULL                  | Solution          | A12345  |
        | C000014-01 | 10000-S-001     | NULL                  | AssociatedService | A12345  |
    When the user makes a request to retrieve a list of order items for the order with ID C000014-01
    And the user sends the retrieve a list of order items request
    Then a response with status code 200 is returned
    And each order item has the expected service instance ID as follows
        | OrderItemId | ServiceInstanceId |
        | 1           | SI1-A12345        |
        | 2           | NULL              |

@10039
@10511
Scenario: The service instance ID is calculated per order
    Given Order items exist
        | OrderId    | CatalogueItemId | ParentCatalogueItemId | CatalogueItemType | OdsCode |
        | C000014-01 | 10000-001       | NULL                  | Solution          | A12345  |
        | C000014-01 | 10000-002       | NULL                  | Solution          | A12345  |
        | C000014-01 | 10000-001-A01   | 10000-001             | AdditionalService | A12345  |
        | C000014-02 | 10000-001       | NULL                  | Solution          | A12345  |
        | C000014-02 | 10000-001-A01   | 10000-001             | AdditionalService | A12345  |
        | C000014-02 | 10000-001       | NULL                  | AssociatedService | A12345  |
    When the user makes a request to retrieve a list of order items for the order with ID C000014-01
    And the user sends the retrieve a list of order items request
    Then a response with status code 200 is returned
    And each order item has the expected service instance ID as follows
        | OrderItemId | ServiceInstanceId |
        | 1           | SI1-A12345        |
        | 2           | SI2-A12345        |
        | 3           | SI1-A12345        |
    When the user makes a request to retrieve a list of order items for the order with ID C000014-02
    And the user sends the retrieve a list of order items request
    Then a response with status code 200 is returned
    And each order item has the expected service instance ID as follows
        | OrderItemId | ServiceInstanceId |
        | 4           | SI1-A12345        |
        | 5           | SI1-A12345        |
        | 6           | NULL              |

@10039
@10511
Scenario: The correct service instance ID is generated for each order item in a multiple item and recipient order
    Given Order items exist
        | OrderId    | CatalogueItemId | ParentCatalogueItemId | CatalogueItemType | OdsCode |
        | C000014-01 | 10000-001       | NULL                  | Solution          | A12345  |
        | C000014-01 | 10000-002       | NULL                  | Solution          | A12345  |
        | C000014-01 | 10000-001-A01   | 10000-001             | AdditionalService | A12345  |
        | C000014-01 | 10000-001-A02   | 10000-001             | AdditionalService | A12345  |
        | C000014-01 | 10000-002-A01   | 10000-002             | AdditionalService | A12345  |
        | C000014-01 | 10000-002-A02   | 10000-002             | AdditionalService | A12345  |
        | C000014-01 | 10000-001       | NULL                  | Solution          | B54321  |
        | C000014-01 | 10000-002       | NULL                  | Solution          | B54321  |
        | C000014-01 | 10000-001-A01   | 10000-001             | AdditionalService | B54321  |
        | C000014-01 | 10000-S-001     | NULL                  | AssociatedService | B54321  |
        | C000014-01 | 10000-001       | NULL                  | Solution          | C67890  |
        | C000014-01 | 10000-001-A01   | 10000-001             | AdditionalService | C67890  |
        | C000014-01 | 10000-002-A01   | 10000-002             | AdditionalService | C67890  |
    When the user makes a request to retrieve a list of order items for the order with ID C000014-01
    And the user sends the retrieve a list of order items request
    Then a response with status code 200 is returned
    And each order item has the expected service instance ID as follows
        | OrderItemId | ServiceInstanceId |
        | 1           | SI1-A12345        |
        | 2           | SI2-A12345        |
        | 3           | SI1-A12345        |
        | 4           | SI1-A12345        |
        | 5           | SI2-A12345        |
        | 6           | SI2-A12345        |
        | 7           | SI1-B54321        |
        | 8           | SI2-B54321        |
        | 9           | SI1-B54321        |
        | 10          | NULL              |
        | 11          | SI1-C67890        |
        | 12          | SI1-C67890        |
        | 13          | NULL              |
