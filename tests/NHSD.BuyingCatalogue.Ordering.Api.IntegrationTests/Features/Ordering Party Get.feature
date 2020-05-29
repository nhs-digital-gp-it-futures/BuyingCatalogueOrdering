Feature: Displaying the Ordering Party in a Buyer Section
    As a Buyer
    I want to view an Ordering Party
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
        | C000014-02 | 4af62b99-638c-4247-875e-965239cd0c48 | Hampshire CC     | 432432              | LS 1 3AP                    |                          | Some Description | 1             | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4616
Scenario: 1. Get an ordering party
    When the user makes a request to retrieve the ordering-party section with the ID C000014-01
    Then a response with status code 200 is returned
    And the ordering-party is returned
        | Name         | OdsCode |
        | Hampshire CC | 432432  |
    And the Address section is returned
        | Line1 | Line2      | Line3      | Line4          | Line5          | Town      | County  | Postcode | Country        |
        | 4     | Upper Flat | Rocks Lane | Little Village | Bigger Village | Some Town | W Yorks | LS 1 3AP | United Kingdom |
    And the Contact section primaryContact is returned
        | FirstName | LastName | EmailAddress            | TelephoneNumber |
        | Fred      | Robinson | Fred.robinson@email.com | 12312543212     |

@4616
Scenario: 2. Get an empty ordering party
    When the user makes a request to retrieve the ordering-party section with the ID C000014-02
    Then a response with status code 200 is returned
    And the response contains no data

@4616
Scenario: 3. A non existent orderId returns not found
    When the user makes a request to retrieve the ordering-party section with the ID INVALID
    Then a response with status code 404 is returned

@4616
Scenario: 4. If a user is not authorised then they cannot access the ordering-party
    Given no user is logged in
    When the user makes a request to retrieve the ordering-party section with the ID C000014-01
    Then a response with status code 401 is returned

@4616
Scenario: 5. A non buyer user cannot access the ordering-party
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve the ordering-party section with the ID C000014-01
    Then a response with status code 403 is returned

@4616
Scenario: 6. A buyer user cannot access the ordering-party for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve the ordering-party section with the ID C000014-01
    Then a response with status code 403 is returned

@4616
Scenario: 7. Service Failure
    Given the call to the database will fail
    When the user makes a request to retrieve the ordering-party section with the ID C000014-01
    Then a response with status code 500 is returned
