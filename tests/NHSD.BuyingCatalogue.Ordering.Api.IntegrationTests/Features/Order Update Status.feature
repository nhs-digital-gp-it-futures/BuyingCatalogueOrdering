Feature: order status update
    As a buyer user
    I want to be able to update the status of an order
    So that I can ensure that the information is kept up to date

Background:
    Given addresses exist
        | Id | Line1        | Town      | Postcode |
        | 1  | 1 The Avenue | Some Town | LS1 3AP  |
    And ordering parties exist
        | Id                                   | OdsCode | Name                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 | OrgOds  | NHS NORTHUMBERLAND CCG |
    And suppliers exist
        | Id    | Name       | Address Id |
        | 10101 | Supplier 1 | 1          |
    And pricing units exist
        | Name         | Description      |
        | patient      | per patient      |
        | bed          | per bed          |
        | consultation | per consultation |
    And orders exist
        | OrderId | Description            | OrderingPartyId                      | SupplierId | CommencementDate | FundingSourceOnlyGMS | Completed | Created    |
        | 10001   | Some Description       | 4af62b99-638c-4247-875e-965239cd0c48 | 10101      | 15/12/2020       | True                 | NULL      | 05/05/2020 |
        | 10002   | Some Other Description | 4af62b99-638c-4247-875e-965239cd0c48 | 10101      | 01/10/2020       | NULL                 | NULL      | 05/05/2020 |
        | 10003   | Another Description    | 4af62b99-638c-4247-875e-965239cd0c48 | 10101      | 01/01/2020       | False                | NULL      | 05/05/2020 |
        | 10004   | A Description          | 4af62b99-638c-4247-875e-965239cd0c48 | 10101      | 07/05/2020       | True                 | NULL      | 05/05/2020 |
        | 10005   | Yet another one        | 4af62b99-638c-4247-875e-965239cd0c48 | 10101      | 18/08/2020       | True                 | NULL      | 05/05/2020 |
    And order progress exists
        | OrderId | CatalogueSolutionsViewed | AssociatedServicesViewed |
        | 10001   | True                     | True                     |
        | 10002   | True                     | True                     |
        | 10003   | False                    | True                     |
        | 10004   | True                     | True                     |
        | 10005   | True                     | True                     |
    And service recipients exist
        | OdsCode | Name    |
        | eu      | EU Test |
        | au      | AU Test |
    And catalogue items exist
        | Id        | Name   | CatalogueItemType |
        | 10101-001 | Item 1 | Solution          |
        | 10101-002 | Item 2 | Solution          |
        | 10101-003 | Item 3 | Solution          |
        | 10101-004 | Item 4 | Solution          |
        | 10101-005 | Item 5 | Solution          |
        | 10101-006 | Item 6 | AssociatedService |
        | 10101-007 | Item 7 | Solution          |
    And order items exist
        | OrderId | CatalogueItemId | PriceTimeUnit | EstimationPeriod | Price    | ProvisioningType | CataloguePriceUnitName |
        | 10001   | 10101-001       | Month         | Month            | 599.9999 | Patient          | patient                |
        | 10001   | 10101-002       | Year          | Year             | NULL     | Patient          | patient                |
        | 10002   | 10101-003       | Month         | Month            | NULL     | Declarative      | bed                    |
        | 10003   | 10101-004       | Month         | Month            | 793.2199 | OnDemand         | consultation           |
        | 10004   | 10101-005       | Month         | Month            | 599.9999 | Declarative      | bed                    |
        | 10004   | 10101-006       | Year          | Year             | NULL     | Declarative      | bed                    |
        | 10005   | 10101-007       | Year          | Year             | 405.38   | OnDemand         | consultation           |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode | Quantity | DeliveryDate |
        | 10001   | 10101-001       | eu      | 1        | 05/09/2021   |
        | 10001   | 10101-002       | au      | 1        | 24/12/2021   |
        | 10003   | 10101-004       | eu      | 1        | 05/09/2021   |
        | 10004   | 10101-005       | au      | 1        | 05/09/2021   |
        | 10004   | 10101-006       | au      | 1        | 24/12/2021   |
        | 10005   | 10101-007       | au      | 1        | 17/10/2021   |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5145
Scenario: update an order status
    Given the user creates a request to update the order status for the order with ID 10001
    And the user enters the '<StatusPayload>' update order status request payload
    When the user sends the update order status request
    Then a response with status code 204 is returned
    And the order status is set correctly

    Examples:
        | StatusPayload         |
        | order-status-complete |

@5145
Scenario: update an order status sets last updated correctly
    Given the user creates a request to update the order status for the order with ID 10001
    And the user enters the '<StatusPayload>' update order status request payload
    When the user sends the update order status request
    Then a response with status code 204 is returned
    And the order with ID 10001 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with ID 10001 has LastUpdated time present and it is the current time

    Examples:
        | StatusPayload         |
        | order-status-complete |

@5145
Scenario: an invalid order status provides an appropriate error message
    Given the user creates a request to update the order status for the order with ID 10001
    And the user enters the '<StatusPayload>' update order status request payload
    When the user sends the update order status request
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id        | field        |
        | <ErrorId> | <ErrorField> |

    Examples:
        | StatusPayload           | ErrorId        | ErrorField |
        | order-status-missing    | StatusRequired | Status     |
        | order-status-incomplete | InvalidStatus  | Status     |
        | order-status-invalid    | InvalidStatus  | Status     |

@5145
Scenario: updating the status of an unfinished order provides an appropriate error message
    Given the user creates a request to update the order status for the order with ID 10002
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id               | field |
        | OrderNotComplete | NULL  |

@5322
Scenario: if a user is not authorised, then they cannot update the orders description
    Given no user is logged in
    When the user makes a request to update the description for the order with ID 10001
        | Description         |
        | Another Description |
    Then a response with status code 401 is returned

@5322
Scenario: a non-buyer user cannot update an orders status
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    Given the user creates a request to update the order status for the order with ID 10001
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 403 is returned

@5322
Scenario: a buyer user cannot update an orders status for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    Given the user creates a request to update the order status for the order with ID 10001
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 403 is returned

@5322
Scenario: a user with read only permissions, cannot update an orders status
    Given the user is logged in with the Read-only Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    Given the user creates a request to update the order status for the order with ID 10001
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 403 is returned

@5322
Scenario: service failure
    Given the call to the database will fail
    Given the user creates a request to update the order status for the order with ID 10001
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 500 is returned

@6859
Scenario: update an order status sets the complete date
    Given the user creates a request to update the order status for the order with ID 10001
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 204 is returned
    And the order completed date is set

@9283
Scenario: when an order is complete, and the funding source is false, no email is sent
    Given the user creates a request to update the order status for the order with ID 10003
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 204 is returned
    And no email is sent

@9585
Scenario: when an order is complete, and the funding source is true, but the order does not contain patient numbers, and email is sent containing a CSV file
    Given the user creates a request to update the order status for the order with ID 10004
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 204 is returned
    And only one email is sent
    And the email contains the following information
        | FromAddress                    | FromName              | ToAddress                   | ToName                           | Subject                                      | Text                                 |
        | noreply@buyingcatalogue.nhs.uk | Buying Catalogue Team | gpitfutures.finance@nhs.net | Buying Catalogue Finance Partner | INTEGRATION_TEST New Order C010004-01_OrgOds | Thank you for completing your order. |
    And the email contains the following attachments
        | Filename                   |
        | C010004-01_OrgOds_Full.csv |
    And all attachments use UTF8 encoding
    And the 'full' attachment contains the following data
        | Call Off Agreement ID | Call Off Ordering Party ID | Call Off Ordering Party Name | Call Off Commencement Date | Service Recipient ID | Service Recipient Name | Service Recipient Item ID | Supplier ID | Supplier Name | Product ID | Product Name | Product Type       | Quantity Ordered | Unit of Order | Unit Time | Estimation Period | Price    | Order Type | Funding Type | M1 planned (Delivery Date) | Actual M1 date | Buyer verification date (M2) | Cease Date |
        | C010004-01            | OrgOds                     | NHS NORTHUMBERLAND CCG       | 07/05/2020                 | au                   | AU Test                | C010004-01-au-1           | 10101       | Supplier 1    | 10101-005  | Item 5       | Catalogue Solution | 1                | per bed       | per month |                   | 599.9999 | 2          | Central      | 05/09/2021                 |                |                              |            |
        | C010004-01            | OrgOds                     | NHS NORTHUMBERLAND CCG       | 07/05/2020                 | au                   | AU Test                | C010004-01-au-2           | 10101       | Supplier 1    | 10101-006  | Item 6       | Associated Service | 1                | per bed       | per year  |                   | 0        | 2          | Central      | 24/12/2021                 |                |                              |            |

@9585
Scenario: when an order is complete, and the funding source is true, the email is sent containing a CSV file without UnitTime for OnDemand provisioning
    Given the user creates a request to update the order status for the order with ID 10005
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 204 is returned
    And only one email is sent
    And the email contains the following information
        | FromAddress                    | FromName              | ToAddress                   | ToName                           | Subject                                      | Text                                 |
        | noreply@buyingcatalogue.nhs.uk | Buying Catalogue Team | gpitfutures.finance@nhs.net | Buying Catalogue Finance Partner | INTEGRATION_TEST New Order C010005-01_OrgOds | Thank you for completing your order. |
    And the email contains the following attachments
        | Filename                   |
        | C010005-01_OrgOds_Full.csv |
    And all attachments use UTF8 encoding
    And the 'full' attachment contains the following data
        | Call Off Agreement ID | Call Off Ordering Party ID | Call Off Ordering Party Name | Call Off Commencement Date | Service Recipient ID | Service Recipient Name | Service Recipient Item ID | Supplier ID | Supplier Name | Product ID | Product Name | Product Type       | Quantity Ordered | Unit of Order    | Unit Time | Estimation Period | Price    | Order Type | Funding Type | M1 planned (Delivery Date) | Actual M1 date | Buyer verification date (M2) | Cease Date |
        | C010005-01            | OrgOds                     | NHS NORTHUMBERLAND CCG       | 18/08/2020                 | au                   | AU Test                | C010005-01-au-1           | 10101       | Supplier 1    | 10101-007  | Item 7       | Catalogue Solution | 1                | per consultation |           | per year          | 405.3800 | 3          | Central      | 17/10/2021                 |                |                              |            |

@9585
Scenario: when an order is complete, and the funding source is true, the order only has patient numbers, an email is sent containing two CSV files
    Given the user creates a request to update the order status for the order with ID 10001
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 204 is returned
    And only one email is sent
    And the email contains the following information
        | FromAddress                    | FromName              | ToAddress                   | ToName                           | Subject                                      | Text                                 |
        | noreply@buyingcatalogue.nhs.uk | Buying Catalogue Team | gpitfutures.finance@nhs.net | Buying Catalogue Finance Partner | INTEGRATION_TEST New Order C010001-01_OrgOds | Thank you for completing your order. |
    And the email contains the following attachments
        | Filename                       |
        | C010001-01_OrgOds_Full.csv     |
        | C010001-01_OrgOds_Patients.csv |
    And all attachments use UTF8 encoding
    And the 'patients' attachment contains the following data
        | Call Off Agreement ID | Call Off Ordering Party ID | Call Off Ordering Party Name | Call Off Commencement Date | Service Recipient ID | Service Recipient Name | Service Recipient Item ID | Supplier ID | Supplier Name | Product ID | Product Name | Product Type       | Quantity Ordered | Unit of Order | Price    | Funding Type | M1 planned (Delivery Date) | Actual M1 date | Buyer verification date (M2) | Cease Date |
        | C010001-01            | OrgOds                     | NHS NORTHUMBERLAND CCG       | 15/12/2020                 | eu                   | EU Test                | C010001-01-eu-1           | 10101       | Supplier 1    | 10101-001  | Item 1       | Catalogue Solution | 1                | per patient   | 599.9999 | Central      | 05/09/2021                 |                |                              |            |
        | C010001-01            | OrgOds                     | NHS NORTHUMBERLAND CCG       | 15/12/2020                 | au                   | AU Test                | C010001-01-au-2           | 10101       | Supplier 1    | 10101-002  | Item 2       | Catalogue Solution | 1                | per patient   | 0        | Central      | 24/12/2021                 |                |                              |            |
    And the 'full' attachment contains the following data
        | Call Off Agreement ID | Call Off Ordering Party ID | Call Off Ordering Party Name | Call Off Commencement Date | Service Recipient ID | Service Recipient Name | Service Recipient Item ID | Supplier ID | Supplier Name | Product ID | Product Name | Product Type       | Quantity Ordered | Unit of Order | Unit Time | Estimation Period | Price    | Order Type | Funding Type | M1 planned (Delivery Date) | Actual M1 date | Buyer verification date (M2) | Cease Date |
        | C010001-01            | OrgOds                     | NHS NORTHUMBERLAND CCG       | 15/12/2020                 | eu                   | EU Test                | C010001-01-eu-1           | 10101       | Supplier 1    | 10101-001  | Item 1       | Catalogue Solution | 1                | per patient   | per month | per month         | 599.9999 | 1          | Central      | 05/09/2021                 |                |                              |            |
        | C010001-01            | OrgOds                     | NHS NORTHUMBERLAND CCG       | 15/12/2020                 | au                   | AU Test                | C010001-01-au-2           | 10101       | Supplier 1    | 10101-002  | Item 2       | Catalogue Solution | 1                | per patient   | per year  | per year          | 0        | 1          | Central      | 24/12/2021                 |                |                              |            |
