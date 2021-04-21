Feature: service instance ID
    As a buyer user
    I want to view the service instance ID for each item of an order
    So that the contractual requirements of my order are met

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | IsDeleted | Description | LastUpdatedBy                        | OrderingPartyId                      |
        | 10001   | false     | Order 1     | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
        | 10002   | false     | Order 2     | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    And pricing units exist
        | Name    | Description |
        | patient | per patient |
    And service recipients exist
        | OdsCode | Name             |
        | A12345  | Recipient A12345 |
        | B54321  | Recipient B54321 |
        | C67890  | Recipient C67890 |
    And catalogue items exist
        | Id            | Name                 | CatalogueItemType | ParentCatalogueItemId |
        | 10000-001     | Solution 1           | Solution          | NULL                  |
        | 10000-002     | Solution 2           | Solution          | NULL                  |
        | 10000-001-A01 | Additional Service 1 | AdditionalService | 10000-001             |
        | 10000-001-A02 | Additional Service 2 | AdditionalService | 10000-001             |
        | 10000-002-A01 | Additional Service 3 | AdditionalService | 10000-002             |
        | 10000-002-A02 | Additional Service 4 | AdditionalService | 10000-002             |
        | 10000-S-001   | Associated Service 1 | AssociatedService | NULL                  |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@10039
Scenario: each solution for a particular recipient has its own service instance ID
    Given order items exist
        | OrderId | CatalogueItemId |
        | 10001   | 10000-001       |
        | 10001   | 10000-002       |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 10000-001       | A12345  |
        | 10001   | 10000-002       | A12345  |
    And the user creates a request to retrieve the details of an order by ID 10001
    When the user sends the get order request
    Then a response with status code 200 is returned
    And each order item in the order has the expected service instance ID as follows
        | CatalogueItemId | OdsCode | ServiceInstanceId |
        | 10000-001       | A12345  | SI1-A12345        |
        | 10000-002       | A12345  | SI2-A12345        |

@10039
Scenario: each service recipient of a solution has its own service instance ID
    Given order items exist
        | OrderId | CatalogueItemId |
        | 10001   | 10000-001       |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 10000-001       | A12345  |
        | 10001   | 10000-001       | B54321  |
    And the user creates a request to retrieve the details of an order by ID 10001
    When the user sends the get order request
    Then a response with status code 200 is returned
    And each order item in the order has the expected service instance ID as follows
        | CatalogueItemId | OdsCode | ServiceInstanceId |
        | 10000-001       | A12345  | SI1-A12345        |
        | 10000-001       | B54321  | SI1-B54321        |

@10511
Scenario: an additional service ordered for the same recipient as its parent solution shares the solution service instance ID
    Given order items exist
        | OrderId | CatalogueItemId |
        | 10001   | 10000-001       |
        | 10001   | 10000-001-A01   |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 10000-001       | A12345  |
        | 10001   | 10000-001-A01   | A12345  |
    And the user creates a request to retrieve the details of an order by ID 10001
    When the user sends the get order request
    Then a response with status code 200 is returned
    And each order item in the order has the expected service instance ID as follows
        | CatalogueItemId | OdsCode | ServiceInstanceId |
        | 10000-001       | A12345  | SI1-A12345        |
        | 10000-001-A01   | A12345  | SI1-A12345        |

@10511
Scenario: an additional service ordered for a recipient that is not receiving its parent solution does not have a service instance ID
    Given order items exist
        | OrderId | CatalogueItemId |
        | 10001   | 10000-001       |
        | 10001   | 10000-001-A01   |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 10000-001       | A12345  |
        | 10001   | 10000-001-A01   | B54321  |
    And the user creates a request to retrieve the details of an order by ID 10001
    When the user sends the get order request
    Then a response with status code 200 is returned
    And each order item in the order has the expected service instance ID as follows
        | CatalogueItemId | OdsCode | ServiceInstanceId |
        | 10000-001       | A12345  | SI1-A12345        |
        | 10000-001-A01   | B54321  | NULL              |

@10039
Scenario: an associated service does not have a service instance ID
    Given order items exist
        | OrderId | CatalogueItemId |
        | 10001   | 10000-001       |
        | 10001   | 10000-S-001     |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 10000-001       | A12345  |
        | 10001   | 10000-S-001     | A12345  |
    And the user creates a request to retrieve the details of an order by ID 10001
    When the user sends the get order request
    Then a response with status code 200 is returned
    And each order item in the order has the expected service instance ID as follows
        | CatalogueItemId | OdsCode | ServiceInstanceId |
        | 10000-001       | A12345  | SI1-A12345        |
        | 10000-S-001     | A12345  | NULL              |

@10039
@10511
Scenario: the service instance ID is calculated per order
    Given order items exist
        | OrderId | CatalogueItemId |
        | 10001   | 10000-001       |
        | 10001   | 10000-002       |
        | 10001   | 10000-001-A01   |
        | 10002   | 10000-001       |
        | 10002   | 10000-001-A01   |
        | 10002   | 10000-S-001     |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 10000-001       | A12345  |
        | 10001   | 10000-002       | A12345  |
        | 10001   | 10000-001-A01   | A12345  |
        | 10002   | 10000-001       | A12345  |
        | 10002   | 10000-001-A01   | A12345  |
        | 10002   | 10000-S-001     | A12345  |
    And the user creates a request to retrieve the details of an order by ID 10001
    When the user sends the get order request
    Then a response with status code 200 is returned
    And each order item in the order has the expected service instance ID as follows
        | CatalogueItemId | OdsCode | ServiceInstanceId |
        | 10000-001       | A12345  | SI1-A12345        |
        | 10000-002       | A12345  | SI2-A12345        |
        | 10000-001-A01   | A12345  | SI1-A12345        |
    Given the user creates a request to retrieve the details of an order by ID 10002
    When the user sends the get order request
    Then a response with status code 200 is returned
    And each order item in the order has the expected service instance ID as follows
        | CatalogueItemId | OdsCode | ServiceInstanceId |
        | 10000-001       | A12345  | SI1-A12345        |
        | 10000-001-A01   | A12345  | SI1-A12345        |
        | 10000-S-001     | A12345  | NULL              |

@10039
@10511
Scenario: the correct service instance ID is generated for each order item in a multiple item and recipient order
    Given order items exist
        | OrderId | CatalogueItemId |
        | 10001   | 10000-001       |
        | 10001   | 10000-001-A01   |
        | 10001   | 10000-001-A02   |
        | 10001   | 10000-002       |
        | 10001   | 10000-002-A01   |
        | 10001   | 10000-002-A02   |
        | 10001   | 10000-S-001     |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 10000-001       | A12345  |
        | 10001   | 10000-001       | B54321  |
        | 10001   | 10000-001       | C67890  |
        | 10001   | 10000-001-A01   | A12345  |
        | 10001   | 10000-001-A01   | B54321  |
        | 10001   | 10000-001-A01   | C67890  |
        | 10001   | 10000-001-A02   | A12345  |
        | 10001   | 10000-002       | A12345  |
        | 10001   | 10000-002       | B54321  |
        | 10001   | 10000-002-A01   | A12345  |
        | 10001   | 10000-002-A01   | C67890  |
        | 10001   | 10000-002-A02   | A12345  |
        | 10001   | 10000-S-001     | B54321  |
    And the user creates a request to retrieve the details of an order by ID 10001
    When the user sends the get order request
    Then a response with status code 200 is returned
    And each order item in the order has the expected service instance ID as follows
        | CatalogueItemId | OdsCode | ServiceInstanceId |
        | 10000-001       | A12345  | SI1-A12345        |
        | 10000-001       | B54321  | SI1-B54321        |
        | 10000-001       | C67890  | SI1-C67890        |
        | 10000-001-A01   | A12345  | SI1-A12345        |
        | 10000-001-A01   | B54321  | SI1-B54321        |
        | 10000-001-A01   | C67890  | SI1-C67890        |
        | 10000-001-A02   | A12345  | SI1-A12345        |
        | 10000-002       | A12345  | SI2-A12345        |
        | 10000-002       | B54321  | SI2-B54321        |
        | 10000-002-A01   | A12345  | SI2-A12345        |
        | 10000-002-A01   | C67890  | NULL              |
        | 10000-002-A02   | A12345  | SI2-A12345        |
        | 10000-S-001     | B54321  | NULL              |
