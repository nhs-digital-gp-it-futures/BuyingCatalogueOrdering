Feature: order update commencement date
    As a buyer
    I want to update an order's commencement date
    So that I can keep an order up to date

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | Description      | OrderingPartyId                      | Created    |
        | 10001   | Some Description | 4af62b99-638c-4247-875e-965239cd0c48 | 10/03/2021 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4619
Scenario: updating an orders commencement date to be today
    Given the user sets the commencement date to today
    When the user makes a request to update the commencement date with the ID 10001
    Then a response with status code 204 is returned
    And the order commencement date for the order with ID 10001 is set to today
    And the lastUpdatedName is updated in the database to Bob Smith for the order with ID 10001
    And the order with ID 10001 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with ID 10001 has LastUpdated time present and it is the current time

@4619
Scenario: updating an orders commencement date to be in the future
    Given the user sets the commencement date to 59 days in the future
    When the user makes a request to update the commencement date with the ID 10001
    Then a response with status code 204 is returned
    And the order commencement date for the order with ID 10001 is set to 59 days in the future
    And the lastUpdatedName is updated in the database to Bob Smith for the order with ID 10001
    And the order with ID 10001 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with ID 10001 has LastUpdated time present and it is the current time

@4619
Scenario: updating an orders commencement date to be in the allowable past
    Given the user sets the commencement date to 59 days in the past
    When the user makes a request to update the commencement date with the ID 10001
    Then a response with status code 204 is returned
    And the order commencement date for the order with ID 10001 is set to 59 days in the past
    And the lastUpdatedName is updated in the database to Bob Smith for the order with ID 10001
    And the order with ID 10001 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with ID 10001 has LastUpdated time present and it is the current time

@4619
Scenario: updating an order, with no commencement date, returns a relevant error message
    Given the user sets the commencement date to nothing
    When the user makes a request to update the commencement date with the ID 10001
    Then a response with status code 400 is returned
    And the response contains the following errors
        | Id                       | Field            |
        | CommencementDateRequired | CommencementDate |

@4619
Scenario: updating an order, with commencement days 60 days in the past, returns a relevant error message
    Given the user sets the commencement date to 60 days in the past
    When the user makes a request to update the commencement date with the ID 10001
    Then a response with status code 400 is returned
    And the response contains the following errors
        | Id                          | Field            |
        | CommencementDateGreaterThan | CommencementDate |

@4619
Scenario: a non-buyer user cannot update an orders commencement date
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    And the user sets the commencement date to today
    When the user makes a request to update the commencement date with the ID 10001
    Then a response with status code 403 is returned

# TODO: fix. Suspect param name check in auth filter is problem
@ignore
@4619
Scenario: a buyer user cannot update an orders commencement date for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    And the user sets the commencement date to today
    When the user makes a request to update the commencement date with the ID 10001
    Then a response with status code 403 is returned

@4619
Scenario: service failure
    Given the call to the database will fail
    And the user sets the commencement date to today
    When the user makes a request to update the commencement date with the ID 10001
    Then a response with status code 500 is returned
