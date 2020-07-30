Feature: Order Update Commencement Date
    As a Buyer
    I want to update an order's commencement date
    So that I can keep an order up to date

Background:
    Given Orders exist
        | OrderId    | Description         | Created    | LastUpdated | LastUpdatedByName | LastUpdatedBy                        | OrganisationId                       |
        | C000014-01 | Some Description    | 11/05/2020 | 11/05/2020  | Bob Smith         | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4619
Scenario: 1. Updating an orders commencement date to be today
    Given the user sets the commencement date to today
    When the user makes a request to update the commencement date with the ID C000014-01
    Then a response with status code 204 is returned
    And the order commencement date for order with id C000014-01 is set to today
    And the lastUpdatedName is updated in the database to Bob Smith with orderId C000014-01
    And the order with orderId C000014-01 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with orderId C000014-01 has LastUpdated time present and it is the current time

@4619
Scenario: 2. Updating an orders commencement date to be in the future
    Given the user sets the commencement date to 59 days in the future
    When the user makes a request to update the commencement date with the ID C000014-01
    Then a response with status code 204 is returned
    And the order commencement date for order with id C000014-01 is set to 59 days in the future
    And the lastUpdatedName is updated in the database to Bob Smith with orderId C000014-01
    And the order with orderId C000014-01 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with orderId C000014-01 has LastUpdated time present and it is the current time

@4619
Scenario: 2. Updating an orders commencement date to be in the allowable past
    Given the user sets the commencement date to 59 days in the past
    When the user makes a request to update the commencement date with the ID C000014-01
    Then a response with status code 204 is returned
    And the order commencement date for order with id C000014-01 is set to 59 days in the past
    And the lastUpdatedName is updated in the database to Bob Smith with orderId C000014-01
    And the order with orderId C000014-01 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with orderId C000014-01 has LastUpdated time present and it is the current time

@4619
Scenario: 3. Updating an order, with no commencement date, returns a relevant error message
    Given the user sets the commencement date to nothing
    When the user makes a request to update the commencement date with the ID C000014-01
    Then a response with status code 400 is returned
    And the response contains the following errors
        | Id                       | Field            |
        | CommencementDateRequired | CommencementDate |

@4619
Scenario: 4. Updating an order, with commencement days 60 days in the past, returns a relevant error message
    Given the user sets the commencement date to 60 days in the past
    When the user makes a request to update the commencement date with the ID C000014-01
    Then a response with status code 400 is returned
    And the response contains the following errors
        | Id                          | Field            |
        | CommencementDateGreaterThan | CommencementDate |

@4619
Scenario: 5. A non buyer user cannot update an orders commencement date
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    And the user sets the commencement date to today
    When the user makes a request to update the commencement date with the ID C000014-01
    Then a response with status code 403 is returned

@4619
Scenario: 6. A buyer user cannot update an orders commencement date for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    And the user sets the commencement date to today
    When the user makes a request to update the commencement date with the ID C000014-01
    Then a response with status code 403 is returned

@4619
Scenario: 7. Service Failure
    Given the call to the database will fail
    And the user sets the commencement date to today
    When the user makes a request to update the commencement date with the ID C000014-01
    Then a response with status code 500 is returned
