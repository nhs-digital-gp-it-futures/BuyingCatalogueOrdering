Feature: Updates the Order Section Status
    As an Buyer User
    I want to be able to update the section status for a given order section
    So that I can ensure that the status is complete

Background:
    Given Orders exist
        | OrderId    | Description      | AdditionalServicesViewed | CatalogueSolutionsViewed | AssociatedServicesViewed | LastUpdated | LastUpdatedBy                        | OrganisationId                       |
        | C000014-01 | Some Description | False                    | False                    | False                    | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5124
Scenario: 1. Set a section to be completed
    When the user makes a request to complete order section with order Id C000014-01 section Id <section-id>
    Then a response with status code 204 is returned
    And the order with ID C000014-01 has additional services viewed set to <additional-services-viewed>
    And the order with ID C000014-01 has catalogue solutions viewed set to <catalogue-solutions-viewed>
    And the order with ID C000014-01 has associated services viewed set to <associated-services-viewed>
    And the order with orderId C000014-01 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with orderId C000014-01 has LastUpdated time present and it is the current time

    Examples: Sections
        | section-id          | additional-services-viewed | catalogue-solutions-viewed | associated-services-viewed |
        | additional-services | True                       | False                      | False                      |
        | catalogue-solutions | False                      | True                       | False                      |
        | associated-services | False                      | False                      | True                       |

@5124
Scenario: 2. A non existent orderId returns not found
    When the user makes a request to complete order section with order Id INVALID section Id catalogue-solutions
    Then a response with status code 404 is returned

@5124
Scenario: 3. If a user is not authorised then they cannot update the order section viewed status
    Given no user is logged in
    When the user makes a request to complete order section with order Id C000014-01 section Id catalogue-solutions
    Then a response with status code 401 is returned

@5124
Scenario: 4. A non buyer user cannot update the order section viewed status
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to complete order section with order Id C000014-01 section Id catalogue-solutions
    Then a response with status code 403 is returned

@5124
Scenario: 5. A buyer user cannot update the order section viwed status for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to complete order section with order Id C000014-01 section Id catalogue-solutions
    Then a response with status code 403 is returned

@5124
Scenario: 6. Service Failure
    Given the call to the database will fail
    When the user makes a request to complete order section with order Id C000014-01 section Id catalogue-solutions
    Then a response with status code 500 is returned
