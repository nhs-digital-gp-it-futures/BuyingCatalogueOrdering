Feature: Updating the Ordering Party in a Buyer Section
    As a Buyer
    I want to update an Ordering Party
    So that I can ensure the information is correct

Background:
    Given Addresses exist
        | Line1 | Line2      | Line3      | Line4          | Line5          | Town      | County  | Postcode | Country        |
        | 4     | Upper Flat | Rocks Lane | Little Village | Bigger Village | Some Town | W Yorks | LS 1 3AP | United Kingdom |
    Given Contacts exist
        | FirstName | LastName | EmailAddress            | TelephoneNumber |
        | Fred      | Robinson | Fred.robinson@email.com | 12312543212     |
    Given Orders exist
        | OrderId    | OrganisationId                       | OrganisationName | OrganisationOdsCode | OrganisationAddressPostcode | OrganisationContactEmail | Description      | OrderStatusId | Created    | LastUpdated | LastUpdatedBy                        |
        | C000014-01 | 4af62b99-638c-4247-875e-965239cd0c48 | Hampshire CC     | 432432              | LS 1 3AP                    | Fred.robinson@email.com  | Some Description | 1             | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4616
Scenario: 1. Update an ordering party results in persisted data
    Given an order party update request exist for order ID C000014-01
    And the update request for order ID C000014-01 has a organisation contact
        | FirstName     | LastName     | EmailAddress        | TelephoneNumber |
        | TestFirstName | TestLastName | TestEmail@email.com | TestNumber      |
    And the order party update request for order ID C000014-01 has a organisation address
        | Line1 | Line2    | Line3     | Line4        | Line5           | Town         | County        | Postcode | Country        |
        | 4     | TestRoad | Test Lane | Test Village | Testing Village | Testing Town | TestingCounty | TE 1 ST  | United Kingdom |
    And the order party update request for order ID C000014-01 has a organisation Name of TestCare Center
    And the order party update request for order ID C000014-01 has a organisation OdsCode of ODS1
    When the user makes a request to update the order party on the order with the ID C000014-01
    Then a response with status code 204 is returned
    And the order with orderId C000014-01 is updated and has a primary contact with data
        | FirstName     | LastName     | Email               | Phone      |
        | TestFirstName | TestLastName | TestEmail@email.com | TestNumber |
    And the order with orderId C000014-01 is updated and has a Organisation Address with data
        | Line1 | Line2    | Line3     | Line4        | Line5           | Town         | County        | Postcode | Country        |
        | 4     | TestRoad | Test Lane | Test Village | Testing Village | Testing Town | TestingCounty | TE 1 ST  | United Kingdom |
    And the order with orderId C000014-01 is updated in the database with data
        | OrganisationName |
        | TestCare Center  |
    And the order with orderId C000014-01 is updated in the database with data
        | OrganisationOdsCode |
        | ODS1                |
    And the order with orderId C000014-01 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with orderId C000014-01 has LastUpdated time present and it is the current time

@4616
Scenario: 2. A non existent orderId returns not found
    When the user makes a request to retrieve the ordering-party section with the ID INVALID
    Then a response with status code 404 is returned

@4616
Scenario: 3. If a user is not authorised then they cannot access the ordering-party
    Given no user is logged in
    When the user makes a request to retrieve the ordering-party section with the ID C000014-01
    Then a response with status code 401 is returned

@4616
Scenario: 4. A non buyer user cannot access the ordering-party
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve the ordering-party section with the ID C000014-01
    Then a response with status code 403 is returned

@4616
Scenario: 5. A buyer user cannot access the ordering-party for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve the ordering-party section with the ID C000014-01
    Then a response with status code 403 is returned

@4616
Scenario: 6. Service Failure
    Given the call to the database will fail
    When the user makes a request to retrieve the ordering-party section with the ID C000014-01
    Then a response with status code 500 is returned