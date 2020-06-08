Feature: Update the Service Recipients for an order
    As a Buyer
    I want to  update the service recipients for an order
    So that I can ensure the information is correct

Background:
    Given Orders exist
        | OrderId    | Description         | OrganisationId                       |
        | C000014-01 | Some Description    | 4af62b99-638c-4247-875e-965239cd0c48 |
        | C000014-02 | Another Description | 4af62b99-638c-4247-875e-965239cd0c48 |
    And Service Recipients exist
        | OdsCode | Name         | OrderId    |
        | Ods2    | Another Name | C000014-02 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@7412
Scenario: 1. the user selects service recipients when no other recipients exist
    When the user makes a request to set the service-recipients section with order ID C000014-01
        | OdsCode | Name                |
        | Ods3    | Service Recipients  |
        | Ods4    | Service Recipients2 |
    Then a response with status code 204 is returned
     And the persisted service recipients are
        | OrderId    | OdsCode | Name                |
        | C000014-01 | Ods3    | Service Recipients  |
        | C000014-01 | Ods4    | Service Recipients2 |
        | C000014-02 | Ods2    | Another Name        |

@7412
Scenario: 2. the user selects service recipients excluding an existing recipient
    Given Service Recipients exist
        | OdsCode | Name               | OrderId    |
        | Ods3    | Service Recipients | C000014-01 |
    When the user makes a request to set the service-recipients section with order ID C000014-01
        | OdsCode | Name                |
        | Ods4    | Service Recipients2 |
    Then a response with status code 204 is returned
     And the persisted service recipients are
        | OrderId    | OdsCode | Name                |
        | C000014-01 | Ods4    | Service Recipients2 |
        | C000014-02 | Ods2    | Another Name        |

@7412
Scenario: 3. the user selects service recipients including an existing recipient
    Given Service Recipients exist
        | OdsCode | Name               | OrderId    |
        | Ods3    | Service Recipients | C000014-01 |
    When the user makes a request to set the service-recipients section with order ID C000014-01
        | OdsCode | Name                |
        | Ods3    | Service Recipients  |
        | Ods4    | Service Recipients2 |
    Then a response with status code 204 is returned
     And the persisted service recipients are
        | OrderId    | OdsCode | Name                |
        | C000014-01 | Ods3    | Service Recipients  |
        | C000014-01 | Ods4    | Service Recipients2 |
        | C000014-02 | Ods2    | Another Name        |

@7412
Scenario: 4. the user selects  no services recipients all recipients are removed
    Given Service Recipients exist
        | OdsCode | Name               | OrderId    |
        | Ods3    | Service Recipients | C000014-01 |
    When the user makes a request to set the service-recipients section with order ID C000014-01
        | OdsCode | Name                |
    Then a response with status code 204 is returned
     And the persisted service recipients are
        | OrderId    | OdsCode | Name         |
        | C000014-02 | Ods2    | Another Name |

@7412
Scenario: 5. the user selects service recipients and the order is updated with the users details 
    Given Orders exist
        | OrderId    | Description      | OrganisationId                       | LastUpdatedByName | LastUpdatedBy                        |
        | C000014-03 | Some Description | 4af62b99-638c-4247-875e-965239cd0c48 | OldUserName       | 3e66fe27-2115-4de5-ae75-03aca134610d |
    When the user makes a request to set the service-recipients section with order ID C000014-03
        | OdsCode | Name                |
        | Ods7    | Service Recipients2 |
    Then a response with status code 204 is returned
     And the persisted service recipients are
        | OrderId    | OdsCode | Name                |
        | C000014-02 | Ods2    | Another Name        |
        | C000014-03 | Ods7    | Service Recipients2 |
     And the order with orderId C000014-03 is updated in the database with data
        | LastUpdatedByName | LastUpdatedBy                        |
        | Bob Smith         | 7B195137-6A59-4854-B118-62B39A3101EF |
     And the order with orderId C000014-03 has LastUpdated time present and it is the current time

@7412
Scenario: 6. If an order does not exist, return not found
    When the user makes a request to set the service-recipients section with order ID INVALID
        | OdsCode | Name         |
        | Ods2    | Another Name |
    Then a response with status code 404 is returned

@7412
Scenario: 7. If a user is not authorised then they cannot access the service recipients section
    Given no user is logged in
    When the user makes a request to set the service-recipients section with order ID C000014-02
        | OdsCode | Name         |
        | Ods2    | Another Name |
    Then a response with status code 401 is returned

@7412
Scenario: 8. A non buyer user cannot access the service recipients section
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to set the service-recipients section with order ID C000014-02
        | OdsCode | Name         |
        | Ods2    | Another Name |
    Then a response with status code 403 is returned

@7412
Scenario: 9. A buyer user cannot access the service recipients section for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to set the service-recipients section with order ID C000014-02
        | OdsCode | Name         |
        | Ods2    | Another Name |
    Then a response with status code 403 is returned

@7412
Scenario: 10. Service Failure
    Given the call to the database will fail
    When the user makes a request to set the service-recipients section with order ID C000014-02
        | OdsCode | Name         |
        | Ods2    | Another Name |
    Then a response with status code 500 is returned
