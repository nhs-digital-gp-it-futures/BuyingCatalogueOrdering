Feature: supplier section update validation
    As a buyer
    I want to only update a supplier if the validation requirements are correct
    So that I can make sure correct information is stored

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
        | OrderId | OrderingPartyId                      | SupplierId | SupplierContactId | Description      | Created    | LastUpdated | LastUpdatedBy                        |
        | 10001   | 4af62b99-638c-4247-875e-965239cd0c48 | SupId      | 1                 | Some Description | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4621
Scenario: updating a supplier section with not filling in required fields produces the relevant error message
    Given the user wants to update the supplier address section
        | Line1     | Line2      | Line3       | Line4          | Line5           | Town         | County  | Postcode | Country        |
        | New Line1 | Lower Flat | Rocks Close | Larger Village | Massive Village | Another Town | N Yorks | YO11 1AP | United Kingdom |
    And the user wants to update the supplier contact section
        | FirstName | LastName | EmailAddress | TelephoneNumber |
        | NULL      | NULL     | NULL         | NULL            |
    When the user makes a request to update the supplier with order ID 10001
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
Scenario: updating a supplier section, and exceeding the maxLength fields, produces the relevant error message
    Given the user wants to update the supplier address section
        | Line1     | Line2      | Line3       | Line4          | Line5           | Town         | County  | Postcode | Country        |
        | New Line1 | Lower Flat | Rocks Close | Larger Village | Massive Village | Another Town | N Yorks | YO11 1AP | United Kingdom |
    And the user wants to update the supplier contact section
        | FirstName                | LastName                 | EmailAddress                  | TelephoneNumber         |
        | #A string of length 101# | #A string of length 101# | #A string of length 252#@.com | #A string of length 36# |
    When the user makes a request to update the supplier with order ID 10001
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
Scenario: updating a supplier section, and not providing a correct email address format, produces the relevant error message
    Given the user wants to update the supplier address section
        | Line1     | Line2      | Line3       | Line4          | Line5           | Town         | County  | Postcode | Country        |
        | New Line1 | Lower Flat | Rocks Close | Larger Village | Massive Village | Another Town | N Yorks | YO11 1AP | United Kingdom |
    And the user wants to update the supplier contact section
        | FirstName | LastName | EmailAddress   | TelephoneNumber |
        | Greg      | Smith    | <EmailAddress> | 23456234521     |
    When the user makes a request to update the supplier with order ID 10001
        | SupplierId | SupplierName     |
        | Sup3       | Updated Supplier |
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id                        | field        |
        | EmailAddressInvalidFormat | EmailAddress |

    Examples:
        | EmailAddress        |
        | INVALID             |
        | @Bobsmith.email.com |
        | Bobsmith.email.com@ |
        | Bobsmith.email.com  |
        | @                   |
        | Bob@smith@email.com |
