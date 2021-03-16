Feature: display the order supplier section
    As a buyer
    I want to view an order's supplier details
    So that I can ensure that the information is correct

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And contacts exist
        | Id | FirstName | LastName | Email                   | Phone       |
        | 1  | Fred      | Robinson | Fred.robinson@email.com | 12312543212 |
    And addresses exist
        | Id | Line1 | Line2      | Line3      | Line4          | Line5          | Town      | County  | Postcode | Country        |
        | 1  | 4     | Upper Flat | Rocks Lane | Little Village | Bigger Village | Some Town | W Yorks | LS15 3AP | United Kingdom |
    And suppliers exist
        | Id  | Name               | AddressId |
        | 123 | Some supplier name | 1         |
    And orders exist
        | OrderId | Description      | OrderingPartyId                      | SupplierId | SupplierContactId |
        | 10001   | Some Description | 4af62b99-638c-4247-875e-965239cd0c48 | 123        | 1                 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4621
Scenario: get the order supplier section details
    When the user makes a request to retrieve the order supplier section with the ID 10001
    Then a response with status code 200 is returned
    And the response contains the following supplier details
        | SupplierId | SupplierName       |
        | 123        | Some supplier name |
    And the response contains the following primary supplier contact details
        | FirstName | LastName | Email                   | Phone       |
        | Fred      | Robinson | Fred.robinson@email.com | 12312543212 |
    And the response contains the following supplier address
        | Line1 | Line2      | Line3      | Line4          | Line5          | Town      | County  | Postcode | Country        |
        | 4     | Upper Flat | Rocks Lane | Little Village | Bigger Village | Some Town | W Yorks | LS15 3AP | United Kingdom |

@4621
Scenario: a non-existent order ID returns not found
    When the user makes a request to retrieve the order supplier section with the ID 10000
    Then a response with status code 404 is returned

@4621
Scenario: if a user is not authorised then they cannot access the order supplier section
    Given no user is logged in
    When the user makes a request to retrieve the order supplier section with the ID 10001
    Then a response with status code 401 is returned

Scenario: a non-buyer user cannot access the order supplier section
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve the order supplier section with the ID 10001
    Then a response with status code 403 is returned

@4621
Scenario: a buyer user cannot access the order supplier for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve the order supplier section with the ID 10001
    Then a response with status code 403 is returned

@4621
Scenario: service failure
    Given the call to the database will fail
    When the user makes a request to retrieve the order supplier section with the ID 10001
    Then a response with status code 500 is returned
