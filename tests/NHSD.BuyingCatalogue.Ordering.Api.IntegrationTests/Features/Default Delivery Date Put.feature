Feature: default delivery date PUT
    As a buyer
    I want to be able to set the default delivery date of a catalogue item
    To reduce the amount of effort required to place an order

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | Description            | OrderingPartyId                      | CommencementDate |
        | 10001   | Some Description       | 4af62b99-638c-4247-875e-965239cd0c48 | 15/12/2020       |
        | 10002   | Some Other Description | 4af62b99-638c-4247-875e-965239cd0c48 | 01/10/2020       |
        | 10003   | Some Description       | 4af62b99-638c-4247-875e-965239cd0c48 |                  |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@8952
Scenario: set the default delivery date of a catalogue item
    Given the user sets the default delivery date using the following details
        | OrderId | CatalogueItemId | DeliveryDate |
        | 10002   | 10001-001       | 31/10/2020   |
    When the user confirms the default delivery date
    Then a response with status code 201 is returned
    And the default delivery date is set correctly

@8952
Scenario: update the default delivery date of a catalogue item
    Given the following default delivery dates have already been set
        | OrderId | CatalogueItemId | DeliveryDate |
        | 10001   | 10001-001       | 31/10/2020   |
        | 10001   | 10001-002       | 31/10/2020   |
    And the user sets the default delivery date using the following details
        | OrderId | CatalogueItemId | DeliveryDate |
        | 10001   | 10001-002       | 01/01/2021   |
    When the user confirms the default delivery date
    Then a response with status code 200 is returned
    And the default delivery date is set correctly
    And the following default delivery dates remain unchanged
        | OrderId | CatalogueItemId | DeliveryDate |
        | 10001   | 10001-001       | 31/10/2020   |

@8952
Scenario: if a user is not authorized then they cannot set a default delivery date
    Given no user is logged in
    And the user sets the default delivery date using the following details
        | OrderId | CatalogueItemId | DeliveryDate |
        | 10001   | 10001-001       | 31/10/2020   |
    When the user confirms the default delivery date
    Then a response with status code 401 is returned

@8952
Scenario: a non-buyer user cannot set a default delivery date
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    And the user sets the default delivery date using the following details
        | OrderId | CatalogueItemId | DeliveryDate |
        | 10001   | 10001-001       | 31/10/2020   |
    When the user confirms the default delivery date
    Then a response with status code 403 is returned

@8952
Scenario: a user with read only permissions cannot set a default delivery date
    Given the user is logged in with the Read-only Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    And the user sets the default delivery date using the following details
        | OrderId | CatalogueItemId | DeliveryDate |
        | 10001   | 10001-001       | 31/10/2020   |
    When the user confirms the default delivery date
    Then a response with status code 403 is returned

@8952
Scenario: set a default delivery date for an order that does not exist
    Given the user sets the default delivery date using the following details
        | OrderId | CatalogueItemId | DeliveryDate |
        | 10004   | 10001-001       | 31/10/2020   |
    When the user confirms the default delivery date
    Then a response with status code 404 is returned

@8952
Scenario: set a default delivery date for an order that does not have a commencement date
    Given the user sets the default delivery date using the following details
        | OrderId | CatalogueItemId | DeliveryDate |
        | 10003   | 10001-001       | 31/10/2020   |
    When the user confirms the default delivery date
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id                       |
        | CommencementDateRequired |

@8952
Scenario: a default delivery date is not specified
    Given the user sets the default delivery date using the following details
        | OrderId | CatalogueItemId |
        | 10001   | 10001-001       |
    When the user confirms the default delivery date
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id                   | field        |
        | DeliveryDateRequired | DeliveryDate |

@8952
Scenario: set a default delivery date before the commencement date of an order
    Given the user sets the default delivery date using the following details
        | OrderId | CatalogueItemId | DeliveryDate |
        | 10002   | 10001-001       | 30/09/2020   |
    When the user confirms the default delivery date
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id                                | field        |
        | DeliveryDateOutsideDeliveryWindow | DeliveryDate |

@8952
Scenario: set a default delivery date five years after the commencement date of an order
    Given the user sets the default delivery date using the following details
        | OrderId | CatalogueItemId | DeliveryDate |
        | 10002   | 10001-001       | 31/12/2025   |
    When the user confirms the default delivery date
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id                                | field        |
        | DeliveryDateOutsideDeliveryWindow | DeliveryDate |

@8952
Scenario: a buyer user cannot set the default delivery of an order for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    And the user sets the default delivery date using the following details
        | OrderId | CatalogueItemId | DeliveryDate |
        | 10001   | 10001-001       | 31/10/2020   |
    When the user confirms the default delivery date
    Then a response with status code 403 is returned

@8952
Scenario: a service failure causes the expected response to be returned when setting a default delivery date
    Given the call to the database will fail
    And the user sets the default delivery date using the following details
        | OrderId | CatalogueItemId | DeliveryDate |
        | 10001   | 10001-001       | 31/10/2020   |
    When the user confirms the default delivery date
    Then a response with status code 500 is returned
