Feature: displaying the ordering party in a buyer section
    As a buyer
    I want to view an ordering party
    So that I can ensure the information is correct

Background:
    Given addresses exist
        | Id | Line1 | Line2      | Line3      | Line4          | Line5          | Town      | County  | Postcode | Country        |
        | 1  | 4     | Upper Flat | Rocks Lane | Little Village | Bigger Village | Some Town | W Yorks | LS 1 3AP | United Kingdom |
    And contacts exist
        | Id | FirstName | LastName | Email                   | Phone       |
        | 1  | Fred      | Robinson | Fred.robinson@email.com | 12312543212 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4616
Scenario: get an ordering party
    Given ordering parties exist
        | Id                                   | OdsCode | Name         | AddressId |
        | 4af62b99-638c-4247-875e-965239cd0c48 | 432432  | Hampshire CC | 1         |
    And orders exist
        | OrderId | OrderingPartyId                      | OrderingPartyContactId | Description      | Created    | LastUpdated | LastUpdatedBy                        |
        | 10001   | 4af62b99-638c-4247-875e-965239cd0c48 | 1                      | Some Description | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 |
    When the user makes a request to retrieve the ordering-party section for the order with ID 10001
    Then a response with status code 200 is returned
    And the ordering-party is returned
        | Name         | OdsCode |
        | Hampshire CC | 432432  |
    And the address is returned
        | Line1 | Line2      | Line3      | Line4          | Line5          | Town      | County  | Postcode | Country        |
        | 4     | Upper Flat | Rocks Lane | Little Village | Bigger Village | Some Town | W Yorks | LS 1 3AP | United Kingdom |
    And the contact section primaryContact is returned
        | FirstName | LastName | EmailAddress            | TelephoneNumber |
        | Fred      | Robinson | Fred.robinson@email.com | 12312543212     |

@4616
Scenario: get an empty ordering party
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | OrderingPartyId                      | Description      | Created    | LastUpdated | LastUpdatedBy                        |
        | 10002   | 4af62b99-638c-4247-875e-965239cd0c48 | Some Description | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 |
    When the user makes a request to retrieve the ordering-party section for the order with ID 10002
    Then a response with status code 200 is returned
    And the response contains no data

@4616
Scenario: a non-existent order ID returns not found
    When the user makes a request to retrieve the ordering-party section for the order with ID 10000
    Then a response with status code 404 is returned

@4616
Scenario: if a user is not authorised then they cannot access the ordering-party
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | OrderingPartyId                      | Description      | Created    | LastUpdated | LastUpdatedBy                        |
        | 10002   | 4af62b99-638c-4247-875e-965239cd0c48 | Some Description | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 |
    And no user is logged in
    When the user makes a request to retrieve the ordering-party section for the order with ID 10001
    Then a response with status code 401 is returned

@4616
Scenario: a non-buyer user cannot access the ordering-party
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | OrderingPartyId                      | Description      | Created    | LastUpdated | LastUpdatedBy                        |
        | 10002   | 4af62b99-638c-4247-875e-965239cd0c48 | Some Description | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 |
    And the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve the ordering-party section for the order with ID 10001
    Then a response with status code 403 is returned

@4616
Scenario: a buyer user cannot access the ordering-party for an organisation they don't belong to
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | OrderingPartyId                      | Description      | Created    | LastUpdated | LastUpdatedBy                        |
        | 10003   | 4af62b99-638c-4247-875e-965239cd0c48 | Some Description | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 |
    And the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve the ordering-party section for the order with ID 10003
    Then a response with status code 403 is returned

@4616
Scenario: service failure
    Given the call to the database will fail
    When the user makes a request to retrieve the ordering-party section for the order with ID 10001
    Then a response with status code 500 is returned
