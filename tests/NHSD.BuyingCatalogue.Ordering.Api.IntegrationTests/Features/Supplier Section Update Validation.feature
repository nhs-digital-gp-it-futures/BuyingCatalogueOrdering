Feature: Supplier Section Update Validation
    As a Buyer
    I want to only update a supplier if the validation requirements are correct
    So that I can make sure correct information is stored

Background:
    Given Addresses exist
        | Line1 | Line2      | Line3      | Line4          | Line5          | Town      | County  | Postcode | Country        |
        | 4     | Upper Flat | Rocks Lane | Little Village | Bigger Village | Some Town | W Yorks | LS 1 3AP | United Kingdom |
    Given Contacts exist
        | FirstName | LastName | EmailAddress            | TelephoneNumber |
        | Fred      | Robinson | Fred.robinson@email.com | 12312543212     |
    Given Orders exist
        | OrderId    | OrganisationId                       | OrganisationName | OrganisationOdsCode | SupplierId | SupplierAddressPostcode | SupplierContactEmail    | Description      | OrderStatusId | Created    | LastUpdated | LastUpdatedBy                        |
        | C000014-01 | 4af62b99-638c-4247-875e-965239cd0c48 | Hampshire CC     | 432432              | SupId      | LS 1 3AP                | Fred.robinson@email.com | Some Description | 1             | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4621
Scenario: 1. Updating a supplier section with not filling in required fields produces the relavent error message
    Given the user wants to update the SupplierAddress section for the address
        | Line1     | Line2      | Line3       | Line4          | Line5           | Town         | County  | Postcode | Country        |
        | New Line1 | Lower Flat | Rocks Close | Larger Village | Massive Village | Another Town | N Yorks | YO11 1AP | United Kingdom |
    And the user wants to update the SupplierContact section for the contact
        | FirstName | LastName | EmailAddress | TelephoneNumber |
        | NULL      | NULL     | NULL         | NULL            |
    When the user makes a request to update the supplier with order ID C000014-01
        | SupplierId | SupplierName     |
        | Sup3       | Updated Supplier |
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id                      | field           |
        | FirstNameRequired       | FirstName       |
        | LastNameRequired        | LastName        |
        | EmailAddressRequired    | EmailAddress    |
        | TelephoneNumberRequired | TelephoneNumber |

@4621
Scenario: 2. Updating a supplier section, and exceeding the maxLength fields, produces the relavent error message
    Given the user wants to update the SupplierAddress section for the address
        | Line1     | Line2      | Line3       | Line4          | Line5           | Town         | County  | Postcode | Country        |
        | New Line1 | Lower Flat | Rocks Close | Larger Village | Massive Village | Another Town | N Yorks | YO11 1AP | United Kingdom |
    And the user wants to update the SupplierContact section for the contact
        | FirstName                | LastName                 | EmailAddress                  | TelephoneNumber         |
        | #A string of length 101# | #A string of length 101# | #A string of length 252#@.com | #A string of length 36# |
    When the user makes a request to update the supplier with order ID C000014-01
        | SupplierId | SupplierName     |
        | Sup3       | Updated Supplier |
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id                     | field           |
        | FirstNameTooLong       | FirstName       |
        | LastNameTooLong        | LastName        |
        | EmailAddressTooLong    | EmailAddress    |
        | TelephoneNumberTooLong | TelephoneNumber |

@4621
Scenario: 3. Updating a supplier section, and not providing a correct email address format, produces the relavent error message
    Given the user wants to update the SupplierAddress section for the address
        | Line1     | Line2      | Line3       | Line4          | Line5           | Town         | County  | Postcode | Country        |
        | New Line1 | Lower Flat | Rocks Close | Larger Village | Massive Village | Another Town | N Yorks | YO11 1AP | United Kingdom |
    And the user wants to update the SupplierContact section for the contact
        | FirstName | LastName | EmailAddress | TelephoneNumber |
        | Greg      | Smith    | INVALID      | 23456234521     |
    When the user makes a request to update the supplier with order ID C000014-01
        | SupplierId | SupplierName     |
        | Sup3       | Updated Supplier |
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id                        | field        |
        | EmailAddressInvalidFormat | EmailAddress |
