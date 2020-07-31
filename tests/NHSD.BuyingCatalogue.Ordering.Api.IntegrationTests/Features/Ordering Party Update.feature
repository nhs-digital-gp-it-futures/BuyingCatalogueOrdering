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
        | OrderId    | OrganisationId                       | OrganisationName | OrganisationOdsCode | OrganisationAddressPostcode | OrganisationContactEmail | Description      | Created    | LastUpdated | LastUpdatedBy                        |
        | C000014-01 | 4af62b99-638c-4247-875e-965239cd0c48 | Hampshire CC     | 432432              | LS 1 3AP                    | Fred.robinson@email.com  | Some Description | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4616
Scenario: 1. Update an ordering party results in persisted data
    Given an order party update request exist for order ID C000014-01
    And the update request for order ID C000014-01 has a contact
        | FirstName     | LastName     | EmailAddress        | TelephoneNumber |
        | TestFirstName | TestLastName | TestEmail@email.com | TestNumber      |
    And the order party update request for order ID C000014-01 has a address
        | Line1 | Line2    | Line3     | Line4        | Line5           | Town         | County        | Postcode | Country        |
        | 4     | TestRoad | Test Lane | Test Village | Testing Village | Testing Town | TestingCounty | TE 1 ST  | United Kingdom |
    And the order party update request for order ID C000014-01 has a Name of TestCare Center
    And the order party update request for order ID C000014-01 has a OdsCode of ODS1
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
Scenario: 2. Updating an ordering party with boundary values results in persisted data
    Given an order party update request exist for order ID C000014-01
    And the update request for order ID C000014-01 has a contact
        | FirstName                | LastName                 | EmailAddress                  | TelephoneNumber         |
        | #A string of length 100# | #A string of length 100# | #A string of length 251#@.com | #A string of length 35# |
    And the order party update request for order ID C000014-01 has a address
        | Line1 | Line2    | Line3     | Line4        | Line5           | Town         | County        | Postcode | Country        |
        | 4     | TestRoad | Test Lane | Test Village | Testing Village | Testing Town | TestingCounty | TE 1 ST  | United Kingdom |
    And the order party update request for order ID C000014-01 has a Name of TestCare Center
    And the order party update request for order ID C000014-01 has a OdsCode of ODS1
    When the user makes a request to update the order party on the order with the ID C000014-01
    Then a response with status code 204 is returned
    And the order with orderId C000014-01 is updated and has a primary contact with data
        | FirstName                | LastName                 | Email                         | Phone                   |
        | #A string of length 100# | #A string of length 100# | #A string of length 251#@.com | #A string of length 35# |
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
Scenario: 3. Updating an ordering party, with a non existent model returns not found
    When the user makes a request to update the order party with order ID C000014-01 with no model
    Then a response with status code 400 is returned

@4616
Scenario: 4. If a user is not authorised then they cannot update the ordering-party
    Given no user is logged in
    And an order party update request exist for order ID C000014-01
    And the order party update request for order ID C000014-01 has a Name of TestCare Center
    When the user makes a request to update the order party on the order with the ID C000014-01
    Then a response with status code 401 is returned

@4616
Scenario: 5. A non buyer user cannot update the ordering-party
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    And an order party update request exist for order ID C000014-01
    And the order party update request for order ID C000014-01 has a Name of TestCare Center
    When the user makes a request to update the order party on the order with the ID C000014-01
    Then a response with status code 403 is returned

@4616
Scenario: 6. A buyer user cannot update the ordering-party for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    And an order party update request exist for order ID C000014-01
    And the order party update request for order ID C000014-01 has a Name of TestCare Center
    When the user makes a request to update the order party on the order with the ID C000014-01
    Then a response with status code 403 is returned

@4616
Scenario: 7. A user with read only permissions, cannot update an ordering-party
    Given the user is logged in with the Readonly-Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    And an order party update request exist for order ID C000014-01
    And the order party update request for order ID C000014-01 has a Name of TestCare Center
    When the user makes a request to update the order party on the order with the ID C000014-01
    Then a response with status code 403 is returned

@4616
Scenario: 8. Service Failure
    Given the call to the database will fail
    When the user makes a request to retrieve the ordering-party section with the ID C000014-01
    Then a response with status code 500 is returned
