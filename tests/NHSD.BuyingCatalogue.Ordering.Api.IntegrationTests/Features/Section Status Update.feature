Feature: updates the order section status
    As a buyer user
    I want to be able to update the section status for a given order section
    So that I can ensure that the status is complete

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | Description      | Created    | LastUpdated | LastUpdatedBy                        | OrderingPartyId                      |
        | 10001   | Some Description | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    And order progress exists
        | OrderId |
        | 10001   |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5124
Scenario: set a section to be completed
    When the user makes a request to complete order section with order Id 10001 section Id <section-id>
    Then a response with status code 204 is returned
    And the order with ID 10001 has additional services viewed set to <additional-services-viewed>
    And the order with ID 10001 has catalogue solutions viewed set to <catalogue-solutions-viewed>
    And the order with ID 10001 has associated services viewed set to <associated-services-viewed>
#   And the order with ID 10001 is updated in the database with data
#       | LastUpdatedBy                        | LastUpdatedByName |
#       | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
#   And the order with ID 10001 has LastUpdated time present and it is the current time

    Examples: Sections
        | section-id          | additional-services-viewed | catalogue-solutions-viewed | associated-services-viewed |
        | additional-services | True                       | False                      | False                      |
        | catalogue-solutions | False                      | True                       | False                      |
        | associated-services | False                      | False                      | True                       |

@5124
Scenario: a non-existent order ID returns not found
    When the user makes a request to complete order section with order Id 10000 section Id catalogue-solutions
    Then a response with status code 404 is returned

@5124
Scenario: if a user is not authorised then they cannot update the order section viewed status
    Given no user is logged in
    When the user makes a request to complete order section with order Id 10001 section Id catalogue-solutions
    Then a response with status code 401 is returned

@5124
Scenario: a non-buyer user cannot update the order section viewed status
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to complete order section with order Id 10001 section Id catalogue-solutions
    Then a response with status code 403 is returned

@5124
Scenario: a buyer user cannot update the order section viwed status for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to complete order section with order Id 10001 section Id catalogue-solutions
    Then a response with status code 403 is returned

@5124
Scenario: service failure
    Given the call to the database will fail
    When the user makes a request to complete order section with order Id 10001 section Id catalogue-solutions
    Then a response with status code 500 is returned
