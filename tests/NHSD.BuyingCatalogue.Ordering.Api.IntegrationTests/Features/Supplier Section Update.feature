Feature: update a supplier section
    As a buyer
    I want to update an supplier
    So that I can ensure that it is kept up to date

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And addresses exist
        | Id | Line1 | Line2      | Line3      | Line4          | Line5          | Town      | County  | Postcode | Country        |
        | 1  | 4     | Upper Flat | Rocks Lane | Little Village | Bigger Village | Some Town | W Yorks | LS 1 3AP | United Kingdom |
    And contacts exist
        | Id | FirstName | LastName | Email                   | Phone       |
        | 1  | Fred      | Robinson | Fred.robinson@email.com | 12312543212 |
    And suppliers exist
        | Id    | Name               | AddressId |
        | SupId | Some supplier name | 1         |
    And orders exist
        | OrderId | OrderingPartyId                      | SupplierId | SupplierContactEmail | Description      | Created    | LastUpdated | LastUpdatedBy                        |
        | 10001   | 4af62b99-638c-4247-875e-965239cd0c48 | SupId      | 1                    | Some Description | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4621
Scenario: update a supplier section
    Given the user wants to update the supplier address section
        | Line1     | Line2      | Line3       | Line4          | Line5           | Town         | County  | Postcode | Country        |
        | New Line1 | Lower Flat | Rocks Close | Larger Village | Massive Village | Another Town | N Yorks | YO11 1AP | United Kingdom |
    And the user wants to update the supplier contact section
        | FirstName | LastName | EmailAddress         | TelephoneNumber |
        | Greg      | Smith    | Greg.smith@email.com | 23456234521     |
    When the user makes a request to update the supplier with order ID 10001
        | SupplierId | SupplierName     |
        | SupId      | Updated Supplier |
    Then a response with status code 204 is returned
    And the supplier address for order 10001 is
        | Line1     | Line2      | Line3       | Line4          | Line5           | Town         | County  | Postcode | Country        |
        | New Line1 | Lower Flat | Rocks Close | Larger Village | Massive Village | Another Town | N Yorks | YO11 1AP | United Kingdom |
    And the supplier contact for order 10001 is
        | FirstName | LastName | Email                | Phone       |
        | Greg      | Smith    | Greg.smith@email.com | 23456234521 |
    And the supplier for order 10001 is updated
        | SupplierId | SupplierName     |
        | SupId      | Updated Supplier |
    And the order with ID 10001 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with ID 10001 has LastUpdated time present and it is the current time

@4621
Scenario: updating a supplier section with boundary values
    Given the user wants to update the supplier address section
        | Line1     | Line2      | Line3       | Line4          | Line5           | Town         | County  | Postcode | Country        |
        | New Line1 | Lower Flat | Rocks Close | Larger Village | Massive Village | Another Town | N Yorks | YO11 1AP | United Kingdom |
    And the user wants to update the supplier contact section
        | FirstName                | LastName                 | EmailAddress                  | TelephoneNumber         |
        | #A string of length 100# | #A string of length 100# | #A string of length 251#@.com | #A string of length 35# |
    When the user makes a request to update the supplier with order ID 10001
        | SupplierId | SupplierName     |
        | SupId      | Updated Supplier |
    Then a response with status code 204 is returned
    And the lastUpdatedName is updated in the database to Bob Smith for the order with ID 10001
    And the supplier address for order 10001 is
        | Line1     | Line2      | Line3       | Line4          | Line5           | Town         | County  | Postcode | Country        |
        | New Line1 | Lower Flat | Rocks Close | Larger Village | Massive Village | Another Town | N Yorks | YO11 1AP | United Kingdom |
    And the supplier contact for order 10001 is
        | FirstName                | LastName                 | Email                         | Phone                   |
        | #A string of length 100# | #A string of length 100# | #A string of length 251#@.com | #A string of length 35# |
    And the supplier for order 10001 is updated
        | SupplierId | SupplierName     |
        | SupId      | Updated Supplier |
    And the order with ID 10001 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with ID 10001 has LastUpdated time present and it is the current time

@4621
Scenario: updating a supplier section, with a non existent model returns not found
    When the user makes a request to update the supplier with order ID 10001 with no model
    Then a response with status code 400 is returned

@4621
Scenario: if a user is not authorised, then they cannot update the supplier
    Given no user is logged in
    When the user makes a request to update the supplier with order ID 10001
        | SupplierId | SupplierName     |
        | SupId      | Updated Supplier |
    Then a response with status code 401 is returned

@4621
Scenario: a non-buyer user cannot update the supplier section
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to update the supplier with order ID 10001
        | SupplierId | SupplierName     |
        | SupId      | Updated Supplier |
    Then a response with status code 403 is returned

@4621
Scenario: a buyer user cannot update a supplier section for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to update the supplier with order ID 10001
        | SupplierId | SupplierName     |
        | SupId      | Updated Supplier |
    Then a response with status code 403 is returned

@4621
Scenario: a user with read only permissions cannot update a supplier section
    Given the user is logged in with the Read-only Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to update the supplier with order ID 10001
        | SupplierId | SupplierName     |
        | SupId      | Updated Supplier |
    Then a response with status code 403 is returned

@4621
Scenario: service failure
    Given the call to the database will fail
    When the user makes a request to update the supplier with order ID 10001
        | SupplierId | SupplierName     |
        | SupId      | Updated Supplier |
    Then a response with status code 500 is returned
