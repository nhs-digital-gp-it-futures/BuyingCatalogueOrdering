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

@4621
Scenario: 2. A non existent orderId returns not found
    When the user makes a request to retrieve the order supplier section with the ID C000016-01
    Then a response with status code 404 is returned

@4621
Scenario: 3. If a user is not authorised then they cannot access the order supplier section
    Given no user is logged in
    When the user makes a request to retrieve the order supplier section with the ID C000014-01
    Then a response with status code 401 is returned
