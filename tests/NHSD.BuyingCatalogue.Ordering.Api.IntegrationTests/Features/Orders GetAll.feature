Feature: display all of the orders in an authority section
    As an authority User
    I want to view all of the orders for a given organisation
    So that I can ensure that the information is correct

Background:
    Given ordering parties exist
        | Id                                   | OdsCode |
        | 4af62b99-638c-4247-875e-965239cd0c48 | OP1     |
        | e6ea864e-ef1b-41aa-a4d5-04fc6fce0933 | OP2     |
    Given orders exist
        | OrderId | Description               | OrderStatus | IsDeleted | Created    | LastUpdated | LastUpdatedByName | LastUpdatedBy                        | OrderingPartyId                      | Completed  | FundingSourceOnlyGMS |
        | 10001   | Some Description          | Complete    |           | 11/05/2020 | 11/05/2020  | Bob Smith         | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 | 11/05/2020 | NULL                 |
        | 10002   | Another Description       | Incomplete  |           | 05/05/2020 | 09/05/2020  | Alice Smith       | a11a46f9-ce6f-448a-95c2-fde6e61c804a | 4af62b99-638c-4247-875e-965239cd0c48 |            | False                |
        | 10003   | One more Description      | Incomplete  |           | 15/05/2020 | 19/05/2020  | Alice Smith       | a11a46f9-ce6f-448a-95c2-fde6e61c804a | e6ea864e-ef1b-41aa-a4d5-04fc6fce0933 |            | True                 |
        | 10004   | Deleted Order description | Incomplete  | true      | 05/05/2020 | 09/05/2020  | Alice Smith       | a11a46f9-ce6f-448a-95c2-fde6e61c804a | 4af62b99-638c-4247-875e-965239cd0c48 |            | NULL                 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4826
Scenario: get all of the orders from an existing organisation excludes deleted orders
    When a GET request is made for a list of orders with organisationId 4af62b99-638c-4247-875e-965239cd0c48
    Then a response with status code 200 is returned
    And the orders list is returned with the following values
        | OrderId    | Description         | Status     | Created    | LastUpdated | LastUpdatedByName | Completed  | FundingSourceOnlyGMS |
        | C010001-01 | Some Description    | Complete   | 11/05/2020 | 11/05/2020  | Bob Smith         | 11/05/2020 | NULL                 |
        | C010002-01 | Another Description | Incomplete | 05/05/2020 | 09/05/2020  | Alice Smith       |            | False                |

@4826
Scenario: get all of the orders from an invalid organisation
    Given the user is logged in with the Buyer role for organisation 3a72c6ab-0be8-4faa-8cb0-3b2c1f077eeb
    When a GET request is made for a list of orders with organisationId 3a72c6ab-0be8-4faa-8cb0-3b2c1f077eeb
    Then a response with status code 200 is returned
    And an empty list is returned

@4826
Scenario: if a user is not authorised then they cannot access the orders
    Given no user is logged in
    When a GET request is made for a list of orders with organisationId 4af62b99-638c-4247-875e-965239cd0c48
    Then a response with status code 401 is returned

@4826
Scenario: a non-buyer user cannot access the orders
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When a GET request is made for a list of orders with organisationId 4af62b99-638c-4247-875e-965239cd0c48
    Then a response with status code 403 is returned

@4826
Scenario: a buyer user cannot access the orders for an organisation they do not belong to
    When a GET request is made for a list of orders with organisationId e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    Then a response with status code 403 is returned

@4826
Scenario: service failure
    Given the call to the database will fail
    When a GET request is made for a list of orders with organisationId 4af62b99-638c-4247-875e-965239cd0c48
    Then a response with status code 500 is returned
