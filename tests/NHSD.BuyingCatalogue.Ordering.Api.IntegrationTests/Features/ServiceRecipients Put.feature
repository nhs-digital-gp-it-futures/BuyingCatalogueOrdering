Feature: update the selected service recipients for an order
    As a buyer
    I want to  update the service recipients for an order
    So that I can ensure the information is correct

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | Description         | OrderingPartyId                      |
        | 10001   | Some Description    | 4af62b99-638c-4247-875e-965239cd0c48 |
        | 10002   | Another Description | 4af62b99-638c-4247-875e-965239cd0c48 |
    And service recipients exist
        | OdsCode | Name        |
        | ODS1    | Recipient 1 |
        | ODS2    | Recipient 2 |
        | ODS3    | Recipient 3 |
        | ODS4    | Recipient 4 |
    And selected service recipients exist
        | OrderId | OdsCode |
        | 10002   | ODS1    |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@7412
Scenario: the user selects service recipients when no other recipients exist
    When the user makes a request to set the service-recipients section with order ID 10001
        | OdsCode |
        | ODS2    |
        | ODS3    |
    Then a response with status code 204 is returned
    And the persisted selected service recipients are
        | OrderId | OdsCode |
        | 10001   | ODS2    |
        | 10001   | ODS3    |
        | 10002   | ODS1    |

@7412
Scenario: the user selects service recipients excluding an existing recipient
    Given selected service recipients exist
        | OrderId | OdsCode |
        | 10001   | ODS2    |
    When the user makes a request to set the service-recipients section with order ID 10001
        | OdsCode |
        | ODS3    |
    Then a response with status code 204 is returned
    And the persisted selected service recipients are
        | OrderId | OdsCode |
        | 10001   | ODS3    |
        | 10002   | ODS1    |

@7412
Scenario: the user selects service recipients including an existing recipient
    Given selected service recipients exist
        | OrderId | OdsCode |
        | 10001   | ODS2    |
    When the user makes a request to set the service-recipients section with order ID 10001
        | OdsCode |
        | ODS2    |
        | ODS3    |
    Then a response with status code 204 is returned
    And the persisted selected service recipients are
        | OrderId | OdsCode |
        | 10001   | ODS2    |
        | 10001   | ODS3    |
        | 10002   | ODS1    |

@7412
Scenario: the user selects no services recipients all recipients are removed
    Given selected service recipients exist
        | OrderId | OdsCode |
        | 10001   | ODS2    |
    And order progress exists
        | OrderId | ServiceRecipientsViewed |
        | 10001   | true                    |
    When the user makes a request to set the service-recipients section with order ID 10001
        | OdsCode |
    Then a response with status code 204 is returned
    And the persisted selected service recipients are
        | OrderId | OdsCode |
        | 10002   | ODS1    |
    And the order with ID 10001 has catalogue solutions viewed set to false

@7412
Scenario: the user selects service recipients and the order is updated with the users details
    Given orders exist
        | OrderId | Description      | OrderingPartyId                      | LastUpdatedByName | LastUpdatedBy                        | Created    |
        | 10003   | Some Description | 4af62b99-638c-4247-875e-965239cd0c48 | OldUserName       | 3e66fe27-2115-4de5-ae75-03aca134610d | 08/03/2021 |
    When the user makes a request to set the service-recipients section with order ID 10003
        | OdsCode |
        | ODS4    |
    Then a response with status code 204 is returned
    And the persisted selected service recipients are
        | OrderId | OdsCode |
        | 10002   | ODS1    |
        | 10003   | ODS4    |
#   And the order with ID 10003 is updated in the database with data
#       | LastUpdatedByName | LastUpdatedBy                        |
#       | Bob Smith         | 7B195137-6A59-4854-B118-62B39A3101EF |
#   And the order with ID 10003 has LastUpdated time present and it is the current time

@5350
Scenario: the user selects service recipients where ODS codes are shared across orders
    When the user makes a request to set the service-recipients section with order ID 10001
        | OdsCode |
        | ODS1    |
    Then a response with status code 204 is returned
     And the persisted selected service recipients are
        | OrderId | OdsCode |
        | 10001   | ODS1    |
        | 10002   | ODS1    |

@5350
Scenario: the user selects service recipients any removed recipients are only removed from the selected order
    Given selected service recipients exist
        | OrderId | OdsCode |
        | 10001   | ODS1    |
    When the user makes a request to set the service-recipients section with order ID 10001
        | OdsCode |
        | ODS2    |
    Then a response with status code 204 is returned
     And the persisted selected service recipients are
        | OrderId | OdsCode |
        | 10001   | ODS2    |
        | 10002   | ODS1    |

@7412
Scenario: if an order does not exist, return not found
    When the user makes a request to set the service-recipients section with order ID 10000
        | OdsCode |
        | ODS1    |
    Then a response with status code 404 is returned

@7412
Scenario: if a user is not authorised then they cannot access the service recipients section
    Given no user is logged in
    When the user makes a request to set the service-recipients section with order ID 10002
        | OdsCode |
        | ODS1    |
    Then a response with status code 401 is returned

@7412
Scenario: a non-buyer user cannot access the service recipients section
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to set the service-recipients section with order ID 10002
        | OdsCode |
        | ODS1    |
    Then a response with status code 403 is returned

@7412
Scenario: a buyer user cannot access the service recipients section for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to set the service-recipients section with order ID 10002
        | OdsCode |
        | ODS1    |
    Then a response with status code 403 is returned

@7412
Scenario: service failure
    Given the call to the database will fail
    When the user makes a request to set the service-recipients section with order ID 10002
        | OdsCode |
        | ODS1    |
    Then a response with status code 500 is returned

@7412
Scenario: the user selects service recipients and the order marks the service recipient section as viewed
    Given order progress exists
        | OrderId | ServiceRecipientsViewed |
        | 10002   | false                   |
    When the user makes a request to set the service-recipients section with order ID 10002
        | OdsCode |
        | ODS4    |
    Then a response with status code 204 is returned
    And the order with ID 10002 has service recipients viewed set to true
