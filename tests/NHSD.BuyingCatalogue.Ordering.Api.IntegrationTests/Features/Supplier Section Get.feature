Feature: Display the Order Supplier section
	As a Buyer
	I want to view an orders Supplier details
	So that I can ensure that the information is correct

Background:
    Given Orders exist
        | OrderId    | Description         | OrganisationId                       | SupplierId | SupplierName       |
        | C000014-01 | Some Description    | 4af62b99-638c-4247-875e-965239cd0c48 | 123        | Some supplier name |
        | C000015-01 | Another Description | 4af62b99-638c-4247-875e-965239cd0c48 |            |                    |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4621
Scenario: 1. Get the order supplier section details
    When the user makes a request to retrieve the order supplier section with the ID C000014-01
    Then a response with status code 200 is returned
    And the response contains the following supplier details
        | SupplierId | SupplierName       |
        | 123        | Some supplier name |
    And the response contains the following primary supplier contact details
        | FirstName | LastName | EmailAddress       | PhoneNumber |
        | Tom       | Smith    | tomsmith@email.com | 0123456789  |

@4621
Scenario: 2. A non existent orderId returns not found
    When the user makes a request to retrieve the order supplier section with the ID C000016-01
    Then a response with status code 404 is returned

@4621
Scenario: 3. If a user is not authorised then they cannot access the order supplier section
    Given no user is logged in
    When the user makes a request to retrieve the order supplier section with the ID C000014-01
    Then a response with status code 401 is returned

@4621
Scenario: 5. A buyer user cannot access the order supplier for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve the order supplier section with the ID C000014-01
    Then a response with status code 403 is returned

@4621
Scenario: 6. Service Failure
    Given the call to the database will fail
    When the user makes a request to retrieve the order supplier section with the ID C000014-01
    Then a response with status code 500 is returned
