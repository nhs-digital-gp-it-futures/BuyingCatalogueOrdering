Feature: Update an Orders Description in an Buyer Section
    As a Buyer
    I want to update an order's description
    So that I can keep an order up to date

Background:
    Given Orders exist
        | OrderId    | Description      | OrderStatusId | Created    | LastUpdated | LastUpdatedBy                        | OrganisationId                       |
        | C000014-01 | Some Description | 1             | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5322
Scenario: 1. Updating an orders description
    When the user makes a request to update the description with the ID C000014-01
        | Description         |
        | Another Description |
    Then a response with status code 204 is returned
    When the user makes a request to retrieve the order description section with the ID C000014-01
    Then a response with status code 200 is returned
    And the order description is returned
        | Description         |
        | Another Description |

@5322
Scenario: 2. Updating an order, with a non existent model returns not found
    When the user makes a request to update the description with the ID C000014-01 with no model
    Then a response with status code 400 is returned

@5322
Scenario: 3. Updating an order, with no description, returns a relevant error message
    When the user makes a request to update the description with the ID C000014-01
        | Description |
        | NULL        |
    Then a response with status code 400 is returned
    And the response contains the following errors
        | Id                       | Field       |
        | OrderDescriptionRequired | Description |

@5322
Scenario: 4. Updating an order, with a description, exceeding it's maximum limit, returns a relevant error message
    When the user makes a request to update the description with the ID C000014-01
        | Description              |
        | #A string of length 101# |
    Then a response with status code 400 is returned
    And the response contains the following errors
        | Id                      | Field       |
        | OrderDescriptionTooLong | Description |

@5322
Scenario: 5. If a user is not authorised, then they cannot update the orders description
    Given no user is logged in
    When the user makes a request to update the description with the ID C000014-01
        | Description         |
        | Another Description |
    Then a response with status code 401 is returned

@5322
Scenario: 6. A non buyer user cannot update an orders description
     Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to update the description with the ID C000014-01
        | Description         |
        | Another Description |
    Then a response with status code 403 is returned

@5322
Scenario: 7. A buyer user cannot update an orders description for an organisation they don't belong to
     Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to update the description with the ID C000014-01
        | Description         |
        | Another Description |
    Then a response with status code 403 is returned

@5322
Scenario: 8. Service Failure
    Given the call to the database will fail
    When the user makes a request to update the description with the ID C000014-01
        | Description         |
        | Another Description |
    Then a response with status code 500 is returned