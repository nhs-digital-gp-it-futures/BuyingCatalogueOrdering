Feature: Update an Orders Description in an Buyer Section
    As a Buyer
    I want to update an order's description
    So that I can keep an order up to date

Background:
    Given Orders exist
        | OrderId    | Description         | OrderStatusId | Created    | LastUpdated | LastUpdatedByName | LastUpdatedBy                        | OrganisationId                       |
        | C000014-01 | Some Description    | 1             | 11/05/2020 | 11/05/2020  | Bob Smith         | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
        | C000014-02 | Another Description | 2             | 05/05/2020 | 09/05/2020  | Alice Smith       | a11a46f9-ce6f-448a-95c2-fde6e61c804a | 4af62b99-638c-4247-875e-965239cd0c48 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5322
Scenario: 1. Updating an orders description
    When the user makes a request to update the description with the ID C000014-01
        | Description         |
        | Another Description |
    Then a response with status code 204 is returned
    And the order description for order with id C000014-01 is set to
        | Description         |
        | Another Description |
    And the lastUpdatedName is updated in the database to Bob Smith with orderId C000014-01

@5322
Scenario: 2. Updating an orders description and with a changed user name
    When the user makes a request to update the description with the ID C000014-02
        | Description      |
        | Test Description |
    Then a response with status code 204 is returned
    And the order description for order with id C000014-02 is set to
        | Description      |
        | Test Description |
    And the lastUpdatedName is updated in the database to Bob Smith with orderId C000014-02

@5322
Scenario: 3. Updating an order, with a non existent model returns not found
    When the user makes a request to update the description with the ID C000014-01 with no model
    Then a response with status code 400 is returned

@5322
Scenario: 4. Updating an order, with no description, returns a relevant error message
    When the user makes a request to update the description with the ID C000014-01
        | Description |
        | NULL        |
    Then a response with status code 400 is returned
    And the response contains the following errors
        | Id                       | Field       |
        | OrderDescriptionRequired | Description |

@5322
Scenario: 5. Updating an order, with a description, exceeding it's maximum limit, returns a relevant error message
    When the user makes a request to update the description with the ID C000014-01
        | Description              |
        | #A string of length 101# |
    Then a response with status code 400 is returned
    And the response contains the following errors
        | Id                      | Field       |
        | OrderDescriptionTooLong | Description |

@5322
Scenario: 6. If a user is not authorised, then they cannot update the orders description
    Given no user is logged in
    When the user makes a request to update the description with the ID C000014-01
        | Description         |
        | Another Description |
    Then a response with status code 401 is returned

@5322
Scenario: 7. A non buyer user cannot update an orders description
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to update the description with the ID C000014-01
        | Description         |
        | Another Description |
    Then a response with status code 403 is returned

@5322
Scenario: 8. A buyer user cannot update an orders description for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to update the description with the ID C000014-01
        | Description         |
        | Another Description |
    Then a response with status code 403 is returned

@5322
Scenario: 9. A user with read only permissions, cannot update an orders description
    Given the user is logged in with the Readonly-Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to update the description with the ID C000014-01
        | Description         |
        | Another Description |
    Then a response with status code 403 is returned

@5322
Scenario: 10. Service Failure
    Given the call to the database will fail
    When the user makes a request to update the description with the ID C000014-01
        | Description         |
        | Another Description |
    Then a response with status code 500 is returned

@5322
Scenario: 11. Update order description to 100 characters should be successful
    When the user makes a request to update the description with the ID C000014-02
        | Description              |
        | #A string of length 100# |
    Then a response with status code 204 is returned
    And the order description for order with id C000014-02 is set to
        | Description              |
        | #A string of length 100# |
