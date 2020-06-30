Feature: Get the details of a single order
    As an Buyer user
    I want to be able to view a preview of a given order
    So that I can ensure that the information is complete

Background:
    Given Orders exist
        | OrderId    | Description   | OrganisationId                       |
        | C000014-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5321
Scenario: 1. Get an order
    Given the user creates a request to retrieve the details of an order by ID 'C000014-01'
    When the user sends the get order request
    Then a response with status code 200 is returned
    And the get order response displays the expected order
