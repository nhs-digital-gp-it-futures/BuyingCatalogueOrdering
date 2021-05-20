Feature: create order
    As a buyer user
    I want to create an order for a given organisation
    So that I can add information to the order

Background:
    Given the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@6739
Scenario: a user can create an order and data is persisted to the database
    When a POST request is made to create an order
        | OrganisationId                       | Description                         |
        | 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 1 |
    Then a response with status code 201 is returned
    And the order is created in the database with ID 10001 and data
        | Id    | Description                         | OrderStatus | OrderingPartyId                      | LastUpdatedBy                        | LastUpdatedByName |
        | 10001 | This is an order for organisation 1 | Incomplete  | 4af62b99-638c-4247-875e-965239cd0c48 | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with ID 10001 has LastUpdated time present and it is the current time
    And the order with ID 10001 has Created time present and it is the current time
    And the order with ID 10001 has revision 1
    And the order with ID 10001 has call-off ID C010001-01

@6739
Scenario: a user creates an order when existing orders are present, the order is created with a incremented order ID
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | Description      | Created    | LastUpdated | LastUpdatedBy                        | OrderingPartyId                      |
        | 10001   | Some Description | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    When a POST request is made to create an order
        | OrganisationId                       | Description                         |
        | 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 1 |
    Then a response with status code 201 is returned
    And a create order response is returned with the OrderId C010002-01
    And the order is created in the database with ID 10002 and data
        | Id     | Description                         | OrderStatus | OrderingPartyId                      | LastUpdatedBy                        | LastUpdatedByName |
        | 10002  | This is an order for organisation 1 | Incomplete  | 4af62b99-638c-4247-875e-965239cd0c48 | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with ID 10002 has LastUpdated time present and it is the current time
    And the order with ID 10002 has Created time present and it is the current time
    And the order with ID 10002 has revision 1
    And the order with ID 10002 has call-off ID C010002-01

@6739
Scenario: a user creates mutiple orders and the order ID is incremented multiple times and returned
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | Description      | Created    | LastUpdated | LastUpdatedBy                        | OrderingPartyId                      |
        | 10001   | Some Description | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    When a POST request is made to create an order
        | OrganisationId                       | Description                        |
        | 4af62b99-638c-4247-875e-965239cd0c48 | This is order 2 for organisation 1 |
    And a POST request is made to create an order
        | OrganisationId                       | Description                        |
        | 4af62b99-638c-4247-875e-965239cd0c48 | This is order 3 for organisation 1 |
    Then a response with status code 201 is returned
    And a create order response is returned with the OrderId C010003-01

@6739
Scenario: a user can create an order when no orders exist and a default order ID is returned
    When a POST request is made to create an order
        | OrganisationId                       | Description                         |
        | 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 1 |
    Then a response with status code 201 is returned
    And a create order response is returned with the OrderId C010001-01

@6739
Scenario: a user creates an order without specifing an ordering party ID and a status code of 403 is returned
    When a POST request is made to create an order
        | Description                         |
        | This is an order for organisation 1 | 
    Then a response with status code 403 is returned

@6739
Scenario: a user creates an order with an empty GUID as the ordering party ID and a status code of 400 is returned
    When a POST request is made to create an order
        | OrganisationId                       | Description |
        | 00000000-0000-0000-0000-000000000000 | Empty GUID  |
    Then a response with status code 400 is returned
    And the response contains the following errors
        | Id                     | Field          |
        | OrganisationIdRequired | OrganisationId |

@6739
Scenario: a user creates an order without specifing a description and a status code of 400 is returned
    When a POST request is made to create an order
        | OrganisationId                       |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    Then a response with status code 400 is returned
    And the response contains the following errors
        | Id                  | Field       |
        | DescriptionRequired | Description |

@6739
Scenario: a user attempts to create an order with a description that is too long and a status code of 400 is returned
    When a POST request is made to create an order
        | OrganisationId                       | Description              |
        | 4af62b99-638c-4247-875e-965239cd0c48 | #A string of length 101# |
    Then a response with status code 400 is returned
    And the response contains the following errors
        | Id                      | Field       |
        | DescriptionTooLong      | Description |

@6739
Scenario: if a user is not authorised, then they cannot create the order
    Given no user is logged in
    When a POST request is made to create an order
        | OrganisationId                       | Description                         |
        | 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 1 |
    Then a response with status code 401 is returned

@6739
Scenario: a non-buyer user cannot create an order
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When a POST request is made to create an order
        | OrganisationId                       | Description                         |
        | 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 1 |
    Then a response with status code 403 is returned

@6739
Scenario: service failure
    Given the call to the database will fail
    When a POST request is made to create an order
        | OrganisationId                       | Description                         |
        | 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 1 |
    Then a response with status code 500 is returned

@5287
Scenario: create an order after deleting one
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | Description        | LastUpdatedBy                        | OrderingPartyId                      | IsDeleted |
        | 10001   | Some Description 1 | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 | False     |
        | 10002   | Some Description 2 | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 | True      |
    When a POST request is made to create an order
        | OrganisationId                       | Description                         |
        | 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 1 |
    Then a response with status code 201 is returned
    And a create order response is returned with the OrderId C010003-01
