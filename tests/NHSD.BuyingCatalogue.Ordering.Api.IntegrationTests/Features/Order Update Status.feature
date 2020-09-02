Feature: Order Status Update
    As a Buyer User
    I want to be able to update the status of an order
    So that I can ensure that the information is kept up to date

Background:
    Given Orders exist
        | OrderId    | Description            | SupplierId | SupplierName | OrganisationId                       | OrganisationOdsCode | OrganisationName       | CommencementDate | CatalogueSolutionsViewed | AssociatedServicesViewed | FundingSourceOnlyGMS | Completed |
        | C000014-01 | Some Description       | 10101      | Supplier 1   | 4af62b99-638c-4247-875e-965239cd0c48 | OrgOds              | NHS NORTHUMBERLAND CCG | 15/12/2020       | True                     | True                     | True                 | NULL      |
        | C000014-02 | Some Other Description | 10101      | Supplier 1   | 4af62b99-638c-4247-875e-965239cd0c48 | OrgOds              | NHS NORTHUMBERLAND CCG | 01/10/2020       | True                     | True                     | NULL                 | NULL      |
        | C000014-03 | Another Description    | 10101      | Supplier 1   | 4af62b99-638c-4247-875e-965239cd0c48 | OrgOds              | NHS NORTHUMBERLAND CCG | 01/01/2020       | False                    | True                     | False                | NULL      |
        | C000014-04 | A Description          | 10101      | Supplier 1   | 4af62b99-638c-4247-875e-965239cd0c48 | OrgOds              | NHS NORTHUMBERLAND CCG | 07/05/2020       | True                     | True                     | True                 | NULL      |
    And Service Recipients exist
        | OrderId    | OdsCode | Name    |
        | C000014-01 | eu      | EU Test |
        | C000014-02 | eu      | EU Test |
        | C000014-03 | eu      | EU Test |
        | C000014-04 | au      | AU Test |
    And Order items exist
        | OrderId    | CatalogueItemName | CatalogueItemType | OdsCode | PriceTimeUnit | EstimationPeriod | DeliveryDate | Price  | ProvisioningType |
        | C000014-01 | Item 1            | Solution          | eu      | Month         | Month            | 05/09/2021   | 599.99 | Patient          |
        | C000014-01 | Item 2            | Solution          | eu      | Year          | Year             | 24/12/2021   | NULL   | Patient          |
        | C000014-02 | Item 3            | Solution          | eu      | Month         | Month            | NULL         | NULL   | Declarative      |
        | C000014-03 | Item 4            | Solution          | eu      | Month         | Month            | 05/09/2021   | 793.21 | OnDemand         |
        | C000014-04 | Item 5            | AdditionalService | au      | Month         | Month            | 05/09/2021   | 599.99 | Patient          |
        | C000014-04 | Item 6            | AssociatedService | au      | Year          | Year             | 24/12/2021   | NULL   | Declarative      |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5145
Scenario: 1. Update an order status
    Given the user creates a request to update the order status for the order with ID 'C000014-01'
    And the user enters the '<StatusPayload>' update order status request payload
    When the user sends the update order status request
    Then a response with status code 204 is returned
    And the order status is set correctly

    Examples:
        | StatusPayload         |
        | order-status-complete |

@5145
Scenario: 2. Update an order status sets last updated correctly
    Given the user creates a request to update the order status for the order with ID 'C000014-01'
    And the user enters the '<StatusPayload>' update order status request payload
    When the user sends the update order status request
    Then a response with status code 204 is returned
    And the order with orderId C000014-01 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with orderId C000014-01 has LastUpdated time present and it is the current time

    Examples:
        | StatusPayload         |
        | order-status-complete |

@5145
Scenario: 3. An invalid order status provides an appropriate error message
    Given the user creates a request to update the order status for the order with ID 'C000014-01'
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
Scenario: 4. Updating the status of an unfinished order provides an appropriate error message
    Given the user creates a request to update the order status for the order with ID 'C000014-02'
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id               | field |
        | OrderNotComplete | NULL  |

@5322
Scenario: 5. If a user is not authorised, then they cannot update the orders description
    Given no user is logged in
    When the user makes a request to update the description with the ID C000014-01
        | Description         |
        | Another Description |
    Then a response with status code 401 is returned

@5322
Scenario: 6. A non buyer user cannot update an orders status
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    Given the user creates a request to update the order status for the order with ID 'C000014-01'
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 403 is returned

@5322
Scenario: 7. A buyer user cannot update an orders status for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    Given the user creates a request to update the order status for the order with ID 'C000014-01'
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 403 is returned

@5322
Scenario: 8. A user with read only permissions, cannot update an orders status
    Given the user is logged in with the Readonly-Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    Given the user creates a request to update the order status for the order with ID 'C000014-01'
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 403 is returned

@5322
Scenario: 9. Service Failure
    Given the call to the database will fail
    Given the user creates a request to update the order status for the order with ID 'C000014-01'
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 500 is returned

@6859
Scenario: 10. Update an order status sets the complete date
    Given the user creates a request to update the order status for the order with ID 'C000014-01'
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 204 is returned
    And the order completed date is set

@9283
Scenario: 11. When an order is complete, and the funding source is false, no email is sent
    Given the user creates a request to update the order status for the order with ID 'C000014-03'
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 204 is returned
    And no email is sent

@9585
Scenario: 12. When an order is complete, an the funding source is true, but the order does not contain patient numbers, and email is sent containing a CSV for price types
    Given the user creates a request to update the order status for the order with ID 'C000014-04'
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 204 is returned
    And only one email is sent
    And the email contains the following information
        | From                           | To                          | Subject                                      | Text                                 |
        | noreply@buyingcatalogue.nhs.uk | gpitfutures.finance@nhs.net | INTEGRATION_TEST New Order C000014-04_OrgOds | Thank you for completing your order. |
    And the email contains the following attachments
        | Filename               |
        | PurchasingDocument.csv |
    And the price type attachment contains the correct information

@9585
Scenario: 13. When an order is complete, and the funding source is true, the order only has patient numbers, an email is sent containing two CSV's
    Given the user creates a request to update the order status for the order with ID 'C000014-01'
    And the user enters the 'order-status-complete' update order status request payload
    When the user sends the update order status request
    Then a response with status code 204 is returned
    And only one email is sent
    And the email contains the following information
        | From                           | To                          | Subject                                      | Text                                 |
        | noreply@buyingcatalogue.nhs.uk | gpitfutures.finance@nhs.net | INTEGRATION_TEST New Order C000014-01_OrgOds | Thank you for completing your order. |
    And the email contains the following attachments
        | Filename               |
        | PurchasingDocument.csv |
        | PatientNumbers.csv     |
    And the patient numbers price type attachment contains the correct information
    And the price type attachment contains the correct information
