Feature: Create Order
    As a Buyer User
    I want to create an orders for a given organisation
    So that I can add information to the order

Background:
    Given the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@6739
Scenario: 1. A user can create a order and data is persisted to the database;
    When a POST request is made to create an order
        | OrganisationId                       | Description                         |
        | 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 2 |
    Then a response with status code 201 is returned
    And the order is created in the database with orderId C010000-01 and data
        | OrderId    | Description                         | OrderStatus | OrganisationId                       | LastUpdatedBy                        | LastUpdatedByName |
        | C010000-01 | This is an order for organisation 2 | Incomplete  | 4af62b99-638c-4247-875e-965239cd0c48 | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with orderId C010000-01 has LastUpdated time present and it is the current time
    And the order with orderId C010000-01 has Created time present and it is the current time

@6739
Scenario: 2. A user creates an order when existing orders are present, The order is created with a incremented orderId
    Given Orders exist
        | OrderId    | Description      | Created    | LastUpdated | LastUpdatedBy                        | OrganisationId                       |
        | C000014-01 | Some Description | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    When a POST request is made to create an order
        | OrganisationId                       | Description                         |
        | 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 2 |
    Then a response with status code 201 is returned
    And a create order response is returned with the OrderId C000015-01
    And the order is created in the database with orderId C000015-01 and data
        | OrderId    | Description                         | OrderStatus | OrganisationId                       | LastUpdatedBy                        | LastUpdatedByName |
        | C000015-01 | This is an order for organisation 2 | Incomplete  | 4af62b99-638c-4247-875e-965239cd0c48 | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |
    And the order with orderId C000015-01 has LastUpdated time present and it is the current time
    And the order with orderId C000015-01 has Created time present and it is the current time

@6739
Scenario: 3. A user creates mutiple orders and order id is incremented multiple times returned;
    Given Orders exist
        | OrderId    | Description      | Created    | LastUpdated | LastUpdatedBy                        | OrganisationId                       |
        | C000014-01 | Some Description | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    When a POST request is made to create an order
        | OrganisationId                       | Description                         |
        | 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 2 |
    And a POST request is made to create an order
        | OrganisationId                       | Description                         |
        | 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 2 |
    Then a response with status code 201 is returned
    And a create order response is returned with the OrderId C000016-01

@6739
Scenario: 4. A user can create a order when no orders exist and a defualt OrderId is returned;
    When a POST request is made to create an order
        | OrganisationId                       | Description                         |
        | 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 2 |
    Then a response with status code 201 is returned
    And a create order response is returned with the OrderId C010000-01

@6739
Scenario: 5. A user creates an order without specifing an Organisation Id a Status Code of 403 is returned
    When a POST request is made to create an order
        | Description                         |
        | This is an order for organisation 2 |
    Then a response with status code 403 is returned

@6739
Scenario: 6. A user create an order without specifing a description a Status Code of 400 is returned
    When a POST request is made to create an order
        | OrganisationId                       |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    Then a response with status code 400 is returned
    And the response contains the following errors
        | Id                       | Field       |
        | OrderDescriptionRequired | Description |

@6739
Scenario: 7. A user attempts to create a order with a description that is too long, a Status Code of 400 is returned;
    When a POST request is made to create an order
        | OrganisationId                       | Description              |
        | 4af62b99-638c-4247-875e-965239cd0c48 | #A string of length 101# |
    Then a response with status code 400 is returned
    And the response contains the following errors
        | Id                      | Field       |
        | OrderDescriptionTooLong | Description |

@6739
Scenario: 8. If a user is not authorised, then they cannot create the order
    Given no user is logged in
    When a POST request is made to create an order
        | OrganisationId                       | Description                         |
        | 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 2 |
    Then a response with status code 401 is returned

@6739
Scenario: 9. A non buyer user cannot create an order
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When a POST request is made to create an order
        | OrganisationId                       | Description                         |
        | 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 2 |
    Then a response with status code 403 is returned

@6739
Scenario: 10. Service Failure
    Given the call to the database will fail
    When a POST request is made to create an order
        | OrganisationId                       | Description                         |
        | 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 2 |
    Then a response with status code 500 is returned

@5287
Scenario: 11. Create an order after deleting one
    Given Orders exist
        | OrderId    | Description        | LastUpdatedBy                        | OrganisationId                       | IsDeleted |
        | C010098-01 | Some Description 1 | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 | False     |
        | C010099-01 | Some Description 2 | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 | True      |
    When a POST request is made to create an order
        | OrganisationId                       | Description                         |
        | 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 2 |
    Then a response with status code 201 is returned
    And a create order response is returned with the OrderId C010100-01
