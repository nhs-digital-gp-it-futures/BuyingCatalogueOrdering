Feature: Display the Order Supplier section
    As a Buyer
    I want to view an orders Supplier details
    So that I can ensure that the information is correct

Background:
    Given Contacts exist
        | FirstName | LastName | EmailAddress            | TelephoneNumber |
        | Fred      | Robinson | Fred.robinson@email.com | 12312543212     |
    Given Addresses exist
        | Line1 | Line2      | Line3      | Line4          | Line5          | Town      | County  | Postcode | Country        |
        | 4     | Upper Flat | Rocks Lane | Little Village | Bigger Village | Some Town | W Yorks | LS15 3AP | United Kingdom |
    And Orders exist
        | OrderId    | Description      | OrganisationId                       | SupplierId | SupplierName       | SupplierContactEmail    | SupplierAddressPostcode |
        | C000014-01 | Some Description | 4af62b99-638c-4247-875e-965239cd0c48 | 123        | Some supplier name | Fred.robinson@email.com | LS15 3AP                |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4621
Scenario: Get the order supplier section details
    When the user makes a request to retrieve the order supplier section with the ID C000014-01
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
Scenario: A non existent orderId returns not found
    When the user makes a request to retrieve the order supplier section with the ID C000016-01
    Then a response with status code 404 is returned

@4621
Scenario: If a user is not authorised then they cannot access the order supplier section
    Given no user is logged in
    When the user makes a request to retrieve the order supplier section with the ID C000014-01
    Then a response with status code 401 is returned

Scenario: A non buyer user cannot access the order supplier section
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve the order supplier section with the ID C000014-01
    Then a response with status code 403 is returned

@4621
Scenario: A buyer user cannot access the order supplier for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve the order supplier section with the ID C000014-01
    Then a response with status code 403 is returned

@4621
Scenario: Service Failure
    Given the call to the database will fail
    When the user makes a request to retrieve the order supplier section with the ID C000014-01
    Then a response with status code 500 is returned
