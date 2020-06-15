Feature: Display the Order Summary in an Authority Section
    As an Authority User
    I want to be able to view the summary for a given order
    So that I can ensure that the information is complete

Background:
    Given Orders exist
        | OrderId    | Description   | OrganisationId                       |
        | C000014-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5321
Scenario: 1. Get the order summary
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 200 is returned
    And the order summary is returned with the following values
        | OrderId    | OrganisationId                       | Description   |
        | C000014-01 | 4af62b99-638c-4247-875e-965239cd0c48 | A Description |
    And the order Summary Sections have the following values
        | Id                  | Status     | Count |
        | description         | complete   |       |
        | ordering-party      | incomplete |       |
        | supplier            | incomplete |       |
        | commencement-date   | incomplete |       |
        | associated-services | incomplete |       |
        | service-recipients  | incomplete | 0     |
        | catalogue-solutions | incomplete |       |
        | additional-services | incomplete |       |
        | funding-source      | incomplete |       |

@4619
Scenario: 2. Get the order summary when the order has a primary ordering party contact
    Given Contacts exist
        | FirstName | LastName | EmailAddress            | TelephoneNumber |
        | Fred      | Robinson | Fred.robinson@email.com | 12312543212     |
    And Orders exist
        | OrderId    | Description   | OrganisationId                       | OrganisationContactEmail |
        | C000015-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | Fred.robinson@email.com  |
    When the user makes a request to retrieve the order summary with the ID C000015-01
    Then a response with status code 200 is returned
    And the order summary is returned with the following values
        | OrderId    | OrganisationId                       | Description   |
        | C000015-01 | 4af62b99-638c-4247-875e-965239cd0c48 | A Description |
    And the order Summary Sections have the following values
        | Id                  | Status     | Count |
        | description         | complete   |       |
        | ordering-party      | complete   |       |
        | supplier            | incomplete |       |
        | commencement-date   | incomplete |       |
        | associated-services | incomplete |       |
        | service-recipients  | incomplete | 0     |
        | catalogue-solutions | incomplete |       |
        | additional-services | incomplete |       |
        | funding-source      | incomplete |       |

@4619
Scenario: 3. Get the order summary when the order has a primary supplier contact
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
        | Id                  | Status     | Count |
        | description         | complete   |       |
        | ordering-party      | incomplete |       |
        | supplier            | complete   |       |
        | commencement-date   | incomplete |       |
        | associated-services | incomplete |       |
        | service-recipients  | incomplete | 0     |
        | catalogue-solutions | incomplete |       |
        | additional-services | incomplete |       |
        | funding-source      | incomplete |       |

@4619
Scenario: 4. Get the order summary when the order has a commencement date
    Given Orders exist
        | OrderId    | Description   | OrganisationId                       | CommencementDate |
        | C000015-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | 31/05/2020       |
    When the user makes a request to retrieve the order summary with the ID C000015-01
    Then a response with status code 200 is returned
    And the order summary is returned with the following values
        | OrderId    | OrganisationId                       | Description   |
        | C000015-01 | 4af62b99-638c-4247-875e-965239cd0c48 | A Description |
    And the order Summary Sections have the following values
        | Id                  | Status     | Count |
        | description         | complete   |       |
        | ordering-party      | incomplete |       |
        | supplier            | incomplete |       |
        | commencement-date   | complete   |       |
        | associated-services | incomplete |       |
        | service-recipients  | incomplete | 0     |
        | catalogue-solutions | incomplete |       |
        | additional-services | incomplete |       |
        | funding-source      | incomplete |       |

@7412
Scenario: 5. Get the order summary after the user has viewed the service recipients section
    Given Orders exist
        | OrderId    | Description   | OrganisationId                       | ServiceRecipientsViewed |
        | C000015-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | true                    |
    When the user makes a request to retrieve the order summary with the ID C000015-01
    Then a response with status code 200 is returned
    And the order summary is returned with the following values
        | OrderId    | OrganisationId                       | Description   |
        | C000015-01 | 4af62b99-638c-4247-875e-965239cd0c48 | A Description |
    And the order Summary Sections have the following values
        | Id                  | Status     | Count |
        | description         | complete   |       |
        | ordering-party      | incomplete |       |
        | supplier            | incomplete |       |
        | commencement-date   | incomplete |       |
        | associated-services | incomplete |       |
        | service-recipients  | complete   | 0     |
        | catalogue-solutions | incomplete |       |
        | additional-services | incomplete |       |
        | funding-source      | incomplete |       |

@4629
Scenario: 6. Get the order summary that includes a list of service recipients
    Given Orders exist
        | OrderId    | Description   | OrganisationId                       | ServiceRecipientsViewed |
        | C000016-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | true                    |
    And Service Recipients exist
        | OdsCode | Name                      | OrderId    |
        | Ods1    | Updated Service Recipient | C000016-01 |
        | Ods2    | Another Name              | C000016-01 |
    When the user makes a request to retrieve the order summary with the ID C000016-01
    Then a response with status code 200 is returned
    And the order summary is returned with the following values
        | OrderId    | OrganisationId                       | Description   |
        | C000016-01 | 4af62b99-638c-4247-875e-965239cd0c48 | A Description |
    And the order Summary Sections have the following values
        | Id                  | Status     | Count |
        | description         | complete   |       |
        | ordering-party      | incomplete |       |
        | supplier            | incomplete |       |
        | commencement-date   | incomplete |       |
        | associated-services | incomplete |       |
        | service-recipients  | complete   | 2     |
        | catalogue-solutions | incomplete |       |
        | additional-services | incomplete |       |
        | funding-source      | incomplete |       |

@7412
Scenario: 7. Get the order summary after the user has viewed the Catalogue Solutions section 
    Given Orders exist
        | OrderId    | Description   | OrganisationId                       | CatalogueSolutionsViewed |
        | C000015-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | true                     |
    When the user makes a request to retrieve the order summary with the ID C000015-01
    Then a response with status code 200 is returned
    And the order summary is returned with the following values
        | OrderId    | OrganisationId                       | Description   |
        | C000015-01 | 4af62b99-638c-4247-875e-965239cd0c48 | A Description |
    And the order Summary Sections have the following values
        | Id                  | Status     | Count |
        | description         | complete   |       |
        | ordering-party      | incomplete |       |
        | supplier            | incomplete |       |
        | commencement-date   | incomplete |       |
        | associated-services | incomplete |       |
        | service-recipients  | incomplete | 0     |
        | catalogue-solutions | complete   |       |
        | additional-services | incomplete |       |
        | funding-source      | incomplete |       |

@5321
Scenario: 8. If the order ID does not exist, return not found
    When the user makes a request to retrieve the order summary with the ID INVALID
    Then a response with status code 404 is returned

@5321
Scenario: 9. If a user is not authorised then they cannot access the order summary
    Given no user is logged in
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 401 is returned

@5321
Scenario: 10. A non buyer user cannot access the order summary
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 403 is returned

@5321
Scenario: 11. A buyer user cannot access the order summary for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 403 is returned

@5321
Scenario: 12. Service Failure
    Given the call to the database will fail
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 500 is returned
