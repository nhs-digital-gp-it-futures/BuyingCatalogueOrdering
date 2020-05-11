Feature: Display all of the Orders in an Authority Section
    As an Authority User
    I want to view all of the Orders in the solution
    So that I can ensure that the information is correct

Background:
    Given Orders Exit
        | OrderId    | Description         | OrderStatusId | Created    | LastUpdated | LastUpdatedBy                        |
        | C000014-01 | Some Description    | 1             | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 |
        | C000014-02 | Another Description | 2             | 05/05/2020 | 09/05/2020  | a11a46f9-ce6f-448a-95c2-fde6e61c804a |

@4826
Scenario: 1. Get all of the orders
    When a GET request is made for orders
    Then a response with status code 200 is returned
    And the orders list is returned with the following values
        | OrderId    | Description         | Status      | Created    | LastUpdated | LastUpdatedBy                        |
        | C000014-01 | Some Description    | Submitted   | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 |
        | C000014-02 | Another Description | Unsubmitted | 05/05/2020 | 09/05/2020  | a11a46f9-ce6f-448a-95c2-fde6e61c804a |

@ignore
@4826
Scenario: 2. If a user is not authorised then they cannot access the orders
    #This will not have the logging in
    When a GET request is made for orders
    Then a response with status code 401 is returned

@ignore
@4826
Scenario: 3. A non authority user cannot access the orders
    #This will have a non authority user logged in
    When a GET request is made for orders
    Then a response with status code 403 is returned

@4826
Scenario: 4. Service Failure
    Given the call to the database will fail
    When a GET request is made for orders
    Then a response with status code 500 is returned