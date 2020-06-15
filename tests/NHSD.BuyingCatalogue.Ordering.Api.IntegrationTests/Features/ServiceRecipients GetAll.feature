Feature: Display all of the Service Recipients for an order
    As a Buyer
    I want to view all of the service recipients for an order
    So that I can ensure the information is correct

Background:
    Given Orders exist
        | OrderId    | Description         | LastUpdatedByName | LastUpdatedBy                        | OrganisationId                       |
        | C000014-01 | Some Description    | Bob Smith         | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
        | C000014-02 | Another Description | Alice Smith       | a11a46f9-ce6f-448a-95c2-fde6e61c804a | 4af62b99-638c-4247-875e-965239cd0c48 |
    Given Service Recipients exist
        | OdsCode | Name                      | OrderId    |
        | Ods1    | Updated Service Recipient | C000014-01 |
        | Ods2    | Another Name              | C000014-02 |
        | Ods3    | Service Recipients        | C000014-01 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@7412
Scenario: 1. Get the service recipients from an exisiting ordering ID
    When the user makes a request to retrieve the service-recipients section with order ID C000014-01
    Then a response with status code 200 is returned
    And the service recipients are returned
        | OdsCode | Name                      |
        | Ods1    | Updated Service Recipient |
        | Ods3    | Service Recipients        |

@7412
Scenario: 2. If an order does not exist, return not found
    When the user makes a request to retrieve the service-recipients section with order ID INVALID
    Then a response with status code 404 is returned

@7412
Scenario: 3. If a user is not authorised then they cannot access the service recipients section
    Given no user is logged in
    When the user makes a request to retrieve the service-recipients section with order ID C000014-01
    Then a response with status code 401 is returned

@7412
Scenario: 4. A non buyer user cannot access the service recipients section
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve the service-recipients section with order ID C000014-01
    Then a response with status code 403 is returned

@7412
Scenario: 5. A buyer user cannot access the service recipients section for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve the service-recipients section with order ID C000014-01
    Then a response with status code 403 is returned

@7412
Scenario: 6. Service Failure
    Given the call to the database will fail
    When the user makes a request to retrieve the service-recipients section with order ID C000014-01
    Then a response with status code 500 is returned
