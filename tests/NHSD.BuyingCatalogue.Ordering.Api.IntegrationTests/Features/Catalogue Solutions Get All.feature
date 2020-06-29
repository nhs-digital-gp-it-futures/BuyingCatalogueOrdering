Feature: Get all of the catalogue solutions for an order
    As a Buyer User
    I want to view all of the catalogue solutions for a given order
    So that I can ensure that the information is correct

Background:
    Given Orders exist
        | OrderId    | Description         | OrderStatusId | LastUpdatedBy                        | OrganisationId                       |
        | C000014-01 | Some Description    | 1             | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
        | C000014-02 | Another Description | 1             | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    Given Service Recipients exist
        | OrderId    | OdsCode | Name     |
        | C000014-01 | eu      | EU Test  |
        | C000014-01 | auz     | AUZ Test |
        | C000014-01 | ods     | NULL     |
    Given Order Items exist
        | OrderId    | CatalogueItemName | CatalogueItemTypeEnum | OdsCode |
        | C000014-01 | Sol 1             | Solution              | eu      |
        | C000014-01 | Sol 2             | Solution              | auz     |
        | C000014-01 | Sol 3             | Solution              | ods     |
        | C000014-01 | Add Serv 1        | AdditionalService     | eu      |
        | C000014-01 | Ass Serv 1        | AssociatedService     | auz     |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4631
Scenario: 1. Get an orders catalogue solutions
    When the user makes a request to retrieve the order catalogue solutions section with the ID C000014-01
    Then a response with status code 200 is returned
    And the catalogue solutions response contains the order description Some Description
    And the catalogue solutions response contains solutions
        | SolutionName | ServiceRecipientName | ServiceRecipientOdsCode |
        | Sol 1        | EU Test              | eu                      |
        | Sol 2        | AUZ Test             | auz                     |
        | Sol 3        | NULL                 | ods                     |

@4621
Scenario: 2. Get an orders catalouge solution that contains no solutions
    When the user makes a request to retrieve the order catalogue solutions section with the ID C000014-02
    Then a response with status code 200 is returned
    And the catalogue solutions response contains the order description Another Description
    And the catalogue solutions response contains no solutions

@4631
Scenario: 3. A non existent orderId returns not found
    When the user makes a request to retrieve the order catalogue solutions section with the ID INVALID
    Then a response with status code 404 is returned

@4631
Scenario: 4. If a user is not authorised then they cannot access the order catalogue solutions
    Given no user is logged in
    When the user makes a request to retrieve the order catalogue solutions section with the ID C000014-01
    Then a response with status code 401 is returned

@4631
Scenario: 5. A non buyer user cannot access the order catalogue solutions
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve the order catalogue solutions section with the ID C000014-01
    Then a response with status code 403 is returned

@4631
Scenario: 6. A buyer user cannot access the order catalogue solutions for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve the order catalogue solutions section with the ID C000014-01
    Then a response with status code 403 is returned

@4631
Scenario: 7. Service Failure
    Given the call to the database will fail
    When the user makes a request to retrieve the order catalogue solutions section with the ID C000014-01
    Then a response with status code 500 is returned
