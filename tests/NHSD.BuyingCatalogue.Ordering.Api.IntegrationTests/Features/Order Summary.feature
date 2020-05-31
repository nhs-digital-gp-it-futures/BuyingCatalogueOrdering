Feature: Display the Order Summary in an Authority Section
    As an Authority User
    I want to be able to view the summary for a given order
    So that I can ensure that the information is complete

Background:
    Given Contacts exist
        | FirstName | LastName | EmailAddress            | TelephoneNumber |
        | Fred      | Robinson | Fred.robinson@email.com | 12312543212     |        
    Given Orders exist
        | OrderId    | Description   | OrganisationId                       | OrganisationContactEmail | SupplierContactEmail    |
        | C000014-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 |                          |                         |
        | C000015-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | Fred.robinson@email.com  |                         |
        | C000015-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 |                          | Fred.robinson@email.com |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5321
Scenario Outline: 1. Get the order summary
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 200 is returned
    And the order summary is returned with the following values
        | OrderId    | OrganisationId                       | Description   |
        | C000014-01 | 4af62b99-638c-4247-875e-965239cd0c48 | A Description |
    And the order Summary Sections have the following values
        | Id                  | Status     |
        | description         | complete   |
        | ordering-party      | incomplete |
        | supplier            | incomplete |
        | commencement-date   | incomplete |
        | associated-services | incomplete |
        | service-recipients  | incomplete |
        | catalogue-solutions | incomplete |
        | additional-services | incomplete |
        | funding-source      | incomplete |

Scenario Outline: 2. Get the order summary
    When the user makes a request to retrieve the order summary with the ID <OrderId>
    Then a response with status code 200 is returned
    And the response contains these complete order summary <Sections>

    Examples:
        | OrderId    | Sections                      |
        | C000015-01 | description,ordering-party    |
        | C000016-01 | description,supplier          |
        | C000017-01 | description,commencement-date |

Scenario: 2. Get the order summary when the order has an ordering party contact

    When the user makes a request to retrieve the order summary with the ID C000015-01
    Then a response with status code 200 is returned
    And the order summary is returned with the following values
        | OrderId    | OrganisationId                       | Description   |
        | C000015-01 | 4af62b99-638c-4247-875e-965239cd0c48 | A Description |
    And the order Summary Sections have the following values
        | Id                  | Status     |
        | description         | complete   |
        | ordering-party      | complete   |
        | supplier            | incomplete |
        | commencement-date   | incomplete |
        | associated-services | incomplete |
        | service-recipients  | incomplete |
        | catalogue-solutions | incomplete |
        | additional-services | incomplete |
        | funding-source      | incomplete |

Scenario: 2. Get the order summary when the order has a primary supplier contact
    Given Contacts exist
        | FirstName | LastName | EmailAddress            | TelephoneNumber |
        | Fred      | Robinson | Fred.robinson@email.com | 12312543212     |
    And Orders exist
        | OrderId    | Description   | OrganisationId                       | SupplierContactEmail    |
        | C000015-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | Fred.robinson@email.com |
    When the user makes a request to retrieve the order summary with the ID C000015-01
    Then a response with status code 200 is returned
    And the order summary is returned with the following values
        | OrderId    | OrganisationId                       | Description   |
        | C000015-01 | 4af62b99-638c-4247-875e-965239cd0c48 | A Description |
    And the order Summary Sections have the following values
        | Id                  | Status     |
        | description         | complete   |
        | ordering-party      | incomlete  |
        | supplier            | complete   |
        | commencement-date   | incomplete |
        | associated-services | incomplete |
        | service-recipients  | incomplete |
        | catalogue-solutions | incomplete |
        | additional-services | incomplete |
        | funding-source      | incomplete |

@4619
Scenario: 3. Get the order summary when the order has a commencement date
    Given Orders exist
        | OrderId    | Description   | OrganisationId                       | CommencementDate |
        | C000015-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | 31/05/2020       |
    When the user makes a request to retrieve the order summary with the ID C000015-01
    Then a response with status code 200 is returned
    And the order summary is returned with the following values
        | OrderId    | OrganisationId                       | Description   |
        | C000015-01 | 4af62b99-638c-4247-875e-965239cd0c48 | A Description |
    And the order Summary Sections have the following values
        | Id                  | Status     |
        | description         | complete   |
        | ordering-party      | incomplete |
        | supplier            | incomplete |
        | commencement-date   | complete   |
        | associated-services | incomplete |
        | service-recipients  | incomplete |
        | catalogue-solutions | incomplete |
        | additional-services | incomplete |
        | funding-source      | incomplete |

@5321
Scenario: 4. If the order ID does not exist, return not found
    When the user makes a request to retrieve the order summary with the ID INVALID
    Then a response with status code 404 is returned

@5321
Scenario: 5. If a user is not authorised then they cannot access the order summary
    Given no user is logged in
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 401 is returned

@5321
Scenario: 6. A non buyer user cannot access the order summary
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 403 is returned

@5321
Scenario: 7. A buyer user cannot access the order summary for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 403 is returned

@5321
Scenario: 8. Service Failure
    Given the call to the database will fail
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 500 is returned