﻿Feature: Update an Supplier Section
    As a Buyer
    I want to update an supplier
    So that I can ensure that it is kept up to date

Background:
    Given Addresses exist
        | Line1 | Line2      | Line3      | Line4          | Line5          | Town      | County  | Postcode | Country        |
        | 4     | Upper Flat | Rocks Lane | Little Village | Bigger Village | Some Town | W Yorks | LS 1 3AP | United Kingdom |
    Given Contacts exist
        | FirstName | LastName | EmailAddress            | TelephoneNumber |
        | Fred      | Robinson | Fred.robinson@email.com | 12312543212     |
    Given Orders exist
        | OrderId    | OrganisationId                       | OrganisationName | OrganisationOdsCode | SupplierId | SupplierAddressPostcode | SupplierContactEmail    | Description      | Created    | LastUpdated | LastUpdatedBy                        |
        | C000014-01 | 4af62b99-638c-4247-875e-965239cd0c48 | Hampshire CC     | 432432              | SupId      | LS 1 3AP                | Fred.robinson@email.com | Some Description | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4621
Scenario: Update a supplier section
    Given the user wants to update the supplier address section
        | Line1     | Line2      | Line3       | Line4          | Line5           | Town         | County  | Postcode | Country        |
        | New Line1 | Lower Flat | Rocks Close | Larger Village | Massive Village | Another Town | N Yorks | YO11 1AP | United Kingdom |
    And the user wants to update the supplier contact section
        | FirstName | LastName | EmailAddress         | TelephoneNumber |
        | Greg      | Smith    | Greg.smith@email.com | 23456234521     |
    When the user makes a request to update the supplier with order ID C000014-01
        | SupplierId | SupplierName     |
        | Sup3       | Updated Supplier |
    Then a response with status code 204 is returned
    And the supplier address for order C000014-01 is
        | Line1     | Line2      | Line3       | Line4          | Line5           | Town         | County  | Postcode | Country        |
        | New Line1 | Lower Flat | Rocks Close | Larger Village | Massive Village | Another Town | N Yorks | YO11 1AP | United Kingdom |
    And the supplier contact for order C000014-01 is
        | FirstName | LastName | Email                | Phone       |
        | Greg      | Smith    | Greg.smith@email.com | 23456234521 |
    And the supplier for order C000014-01 is updated
        | SupplierId | SupplierName     |
        | Sup3       | Updated Supplier |
    And the order with orderId C000014-01 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with orderId C000014-01 has LastUpdated time present and it is the current time

@4621
Scenario: Updating a supplier section with boundary values
    Given the user wants to update the supplier address section
        | Line1     | Line2      | Line3       | Line4          | Line5           | Town         | County  | Postcode | Country        |
        | New Line1 | Lower Flat | Rocks Close | Larger Village | Massive Village | Another Town | N Yorks | YO11 1AP | United Kingdom |
    And the user wants to update the supplier contact section
        | FirstName                | LastName                 | EmailAddress                  | TelephoneNumber         |
        | #A string of length 100# | #A string of length 100# | #A string of length 251#@.com | #A string of length 35# |
    When the user makes a request to update the supplier with order ID C000014-01
        | SupplierId | SupplierName     |
        | Sup3       | Updated Supplier |
    Then a response with status code 204 is returned
    And the lastUpdatedName is updated in the database to Bob Smith with orderId C000014-01
    And the supplier address for order C000014-01 is
        | Line1     | Line2      | Line3       | Line4          | Line5           | Town         | County  | Postcode | Country        |
        | New Line1 | Lower Flat | Rocks Close | Larger Village | Massive Village | Another Town | N Yorks | YO11 1AP | United Kingdom |
    And the supplier contact for order C000014-01 is
        | FirstName                | LastName                 | Email                         | Phone                   |
        | #A string of length 100# | #A string of length 100# | #A string of length 251#@.com | #A string of length 35# |
    And the supplier for order C000014-01 is updated
        | SupplierId | SupplierName     |
        | Sup3       | Updated Supplier |
    And the order with orderId C000014-01 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with orderId C000014-01 has LastUpdated time present and it is the current time

@4621
Scenario: Updating a supplier section, with a non existent model returns not found
    When the user makes a request to update the supplier with order ID C000014-01 with no model
    Then a response with status code 400 is returned

@4621
Scenario: If a user is not authorised, then they cannot update the supplier
    Given no user is logged in
    When the user makes a request to update the supplier with order ID C000014-01
        | SupplierId | SupplierName     |
        | Sup3       | Updated Supplier |
    Then a response with status code 401 is returned

@4621
Scenario: A non buyer user cannot update the supplier section
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to update the supplier with order ID C000014-01
        | SupplierId | SupplierName     |
        | Sup3       | Updated Supplier |
    Then a response with status code 403 is returned

@4621
Scenario: A buyer user cannot update a supplier section for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to update the supplier with order ID C000014-01
        | SupplierId | SupplierName     |
        | Sup3       | Updated Supplier |
    Then a response with status code 403 is returned

@4621
Scenario: A user with read only permissions cannot update a supplier section
    Given the user is logged in with the Readonly-Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to update the supplier with order ID C000014-01
        | SupplierId | SupplierName     |
        | Sup3       | Updated Supplier |
    Then a response with status code 403 is returned

@4621
Scenario: Service Failure
    Given the call to the database will fail
    When the user makes a request to update the supplier with order ID C000014-01
        | SupplierId | SupplierName     |
        | Sup3       | Updated Supplier |
    Then a response with status code 500 is returned
