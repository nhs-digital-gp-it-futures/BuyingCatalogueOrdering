Feature: Update the funding source for an order
    As a Buyer
    I want to  update the funding source for an order
    So that I can ensure the information is correct

Background:
    Given Orders exist
        | OrderId    | Description         | OrganisationId                       |
        | C000014-01 | Some Description    | 4af62b99-638c-4247-875e-965239cd0c48 |
        | C000014-02 | Another Description | 4af62b99-638c-4247-875e-965239cd0c48 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@mytag
Scenario: 1. Update funding source
    Given the user creates a request to update the funding source for the order with ID 'C000014-02'
    And the user enters the '<FundingSourcePayload>' update funding source request payload
    When the user sends the update funding source request
    Then a response with status code 204 is returned
    And the funding source is set correctly
Examples:
    | FundingSourcePayload |
    | funding-source-true  |
    | funding-source-false |
