Feature: ordering party update validation
    As a buyer
    I want to only update an ordering party if the validation requirements are correct
    So that I can make sure correct information is stored

Background:
    Given addresses exist
        | Id | Line1 | Line2      | Line3      | Line4          | Line5          | Town      | County  | Postcode | Country        |
        | 1  | 4     | Upper Flat | Rocks Lane | Little Village | Bigger Village | Some Town | W Yorks | LS 1 3AP | United Kingdom |
    And contacts exist
        | Id | FirstName | LastName | Email                   | Phone       |
        | 1  | Fred      | Robinson | Fred.robinson@email.com | 12312543212 |
    And ordering parties exist
        | Id                                   | OdsCode | Name         | AddressId |
        | 4af62b99-638c-4247-875e-965239cd0c48 | 432432  | Hampshire CC | 1         |
    Given orders exist
        | OrderId | OrderingPartyId                      | OrderingPartyContactId | Description      | Created    | LastUpdated | LastUpdatedBy                        |
        | 10001   | 4af62b99-638c-4247-875e-965239cd0c48 | 1                      | Some Description | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4621
Scenario: updating an ordering party with not filling in required fields produces the relavent error message
    Given an order party update request exist for order ID 10001
    And the update request for order ID 10001 has a contact
        | FirstName | LastName | EmailAddress | TelephoneNumber |
        | NULL      | NULL     | NULL         | NULL            |
    And the order party update request for order ID 10001 has an address
        | Line1 | Line2    | Line3     | Line4        | Line5           | Town         | County        | Postcode | Country        |
        | 4     | TestRoad | Test Lane | Test Village | Testing Village | Testing Town | TestingCounty | TE 1 ST  | United Kingdom |
    And the order party update request for order ID 10001 has a Name of TestCare Centre
    And the order party update request for order ID 10001 has a OdsCode of 432432
    When the user makes a request to update the order party on the order with the ID 10001
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id                      | field           |
        | FirstNameRequired       | FirstName       |
        | LastNameRequired        | LastName        |
        | EmailAddressRequired    | EmailAddress    |
        | TelephoneNumberRequired | TelephoneNumber |

@4621
Scenario: updating an ordering party, and exceeding the maxLength fields, produces the relavent error message
    Given an order party update request exist for order ID 10001
    And the update request for order ID 10001 has a contact
        | FirstName                | LastName                 | EmailAddress                  | TelephoneNumber         |
        | #A string of length 101# | #A string of length 101# | #A string of length 252#@.com | #A string of length 36# |
    And the order party update request for order ID 10001 has an address
        | Line1 | Line2    | Line3     | Line4        | Line5           | Town         | County        | Postcode | Country        |
        | 4     | TestRoad | Test Lane | Test Village | Testing Village | Testing Town | TestingCounty | TE 1 ST  | United Kingdom |
    And the order party update request for order ID 10001 has a Name of TestCare Centre
    And the order party update request for order ID 10001 has a OdsCode of 432432
    When the user makes a request to update the order party on the order with the ID 10001
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id                     | field           |
        | FirstNameTooLong       | FirstName       |
        | LastNameTooLong        | LastName        |
        | EmailAddressTooLong    | EmailAddress    |
        | TelephoneNumberTooLong | TelephoneNumber |

@4621
Scenario: updating an ordering party, and not providing a correct email address format, produces the relavent error message
    Given an order party update request exist for order ID 10001
    And the update request for order ID 10001 has a contact
        | FirstName | LastName | EmailAddress   | TelephoneNumber |
        | Greg      | Smith    | <EmailAddress> | 23456234521     |
    And the order party update request for order ID 10001 has an address
        | Line1 | Line2    | Line3     | Line4        | Line5           | Town         | County        | Postcode | Country        |
        | 4     | TestRoad | Test Lane | Test Village | Testing Village | Testing Town | TestingCounty | TE 1 ST  | United Kingdom |
    And the order party update request for order ID 10001 has a Name of TestCare Center
    And the order party update request for order ID 10001 has a OdsCode of 432432
    When the user makes a request to update the order party on the order with the ID 10001
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
