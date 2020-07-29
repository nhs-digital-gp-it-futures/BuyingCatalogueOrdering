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
        | associated-services | incomplete | 0     |
        | service-recipients  | incomplete | 0     |
        | catalogue-solutions | incomplete | 0     |
        | additional-services | incomplete | 0     |
        | funding-source      | incomplete |       |
    And the order Section Status is incomplete

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
        | associated-services | incomplete | 0     |
        | service-recipients  | incomplete | 0     |
        | catalogue-solutions | incomplete | 0     |
        | additional-services | incomplete | 0     |
        | funding-source      | incomplete |       |
    And the order Section Status is incomplete

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
        | associated-services | incomplete | 0     |
        | service-recipients  | incomplete | 0     |
        | catalogue-solutions | incomplete | 0     |
        | additional-services | incomplete | 0     |
        | funding-source      | incomplete |       |
    And the order Section Status is incomplete

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
        | associated-services | incomplete | 0     |
        | service-recipients  | incomplete | 0     |
        | catalogue-solutions | incomplete | 0     |
        | additional-services | incomplete | 0     |
        | funding-source      | incomplete |       |
    And the order Section Status is incomplete

@5115
Scenario: 5. Get the order summary after a section has been viewed
    Given Orders exist
        | OrderId    | Description   | OrganisationId                       | ServiceRecipientsViewed     | AdditionalServicesViewed     | CatalogueSolutionsViewed     | AssociatedServicesViewed     | FundingSourceOnlyGMS      |
        | C000015-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | <service-recipients-viewed> | <additional-services-viewed> | <catalogue-solutions-viewed> | <associated-services-viewed> | <funding-source-only-gms> |
    When the user makes a request to retrieve the order summary with the ID C000015-01
    Then a response with status code 200 is returned
    And the order summary is returned with the following values
        | OrderId    | OrganisationId                       | Description   |
        | C000015-01 | 4af62b99-638c-4247-875e-965239cd0c48 | A Description |
    And the order Summary Sections have the following values
        | Id                  | Status                       | Count |
        | description         | complete                     |       |
        | ordering-party      | incomplete                   |       |
        | supplier            | incomplete                   |       |
        | commencement-date   | incomplete                   |       |
        | associated-services | <associated-services-status> | 0     |
        | service-recipients  | <service-recipients-status>  | 0     |
        | catalogue-solutions | <catalogue-solutions-status> | 0     |
        | additional-services | <additional-services-status> | 0     |
        | funding-source      | <funding-source-status>      |       |
    And the order Section Status is incomplete

    Examples: Sections
        | service-recipients-viewed | additional-services-viewed | catalogue-solutions-viewed | associated-services-viewed | funding-source-only-gms | service-recipients-status | additional-services-status | catalogue-solutions-status | associated-services-status | funding-source-status |
        | True                      | False                      | False                      | False                      | NULL                    | complete                  | incomplete                 | incomplete                 | incomplete                 | incomplete            |
        | False                     | True                       | False                      | False                      | NULL                    | incomplete                | complete                   | incomplete                 | incomplete                 | incomplete            |
        | False                     | False                      | True                       | False                      | NULL                    | incomplete                | incomplete                 | complete                   | incomplete                 | incomplete            |
        | False                     | False                      | False                      | True                       | False                   | incomplete                | incomplete                 | incomplete                 | complete                   | complete              |
        | False                     | False                      | False                      | False                      | True                    | incomplete                | incomplete                 | incomplete                 | incomplete                 | complete              |

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
        | associated-services | incomplete | 0     |
        | service-recipients  | complete   | 2     |
        | catalogue-solutions | incomplete | 0     |
        | additional-services | incomplete | 0     |
        | funding-source      | incomplete |       |
    And the order Section Status is incomplete

@5123
Scenario: 7. Get the order summary that includes a list of Catalogue Solutions
    Given Orders exist
        | OrderId    | Description   | OrganisationId                       | CatalogueSolutionsViewed |
        | C000016-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | true                     |
    And Service Recipients exist
        | OdsCode | Name                      | OrderId    |
        | Ods1    | Updated Service Recipient | C000016-01 |
        | Ods2    | Another Name              | C000016-01 |
    And Order items exist
        | OrderId    | OdsCode | CatalogueItemName | CatalogueItemType |
        | C000016-01 | Ods1    | Order Item 1      | Solution          |
        | C000016-01 | Ods2    | Order Item 2      | Solution          |
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
        | associated-services | incomplete | 0     |
        | service-recipients  | incomplete | 2     |
        | catalogue-solutions | complete   | 2     |
        | additional-services | incomplete | 0     |
        | funding-source      | incomplete |       |
    And the order Section Status is incomplete

@5115
Scenario: 8. Get the order summary that includes a list of associated services
    Given Orders exist
        | OrderId    | Description   | OrganisationId                       | AssociatedServicesViewed |
        | C000016-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | true                     |
    And Service Recipients exist
        | OdsCode | Name                      | OrderId    |
        | Ods1    | Updated Service Recipient | C000016-01 |
    And Order items exist
        | OrderId    | OdsCode | CatalogueItemName | CatalogueItemType |
        | C000016-01 | Ods1    | Associated Item 1 | AssociatedService |
        | C000016-01 | Ods1    | Associated Item 2 | AssociatedService |
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
        | associated-services | complete   | 2     |
        | service-recipients  | incomplete | 1     |
        | catalogue-solutions | incomplete | 0     |
        | additional-services | incomplete | 0     |
        | funding-source      | incomplete |       |
    And the order Section Status is incomplete

@5115
Scenario: 9. Get the order summary that includes a list of additional services
    Given Orders exist
        | OrderId    | Description   | OrganisationId                       | AdditionalServicesViewed |
        | C000016-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | true                     |
    And Service Recipients exist
        | OdsCode | Name                      | OrderId    |
        | Ods1    | Updated Service Recipient | C000016-01 |
    And Order items exist
        | OrderId    | OdsCode | CatalogueItemName | CatalogueItemType |
        | C000016-01 | Ods1    | Additional Item 1 | AdditionalService |
        | C000016-01 | Ods1    | Additional Item 2 | AdditionalService |
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
        | associated-services | incomplete | 0     |
        | service-recipients  | incomplete | 1     |
        | catalogue-solutions | incomplete | 0     |
        | additional-services | complete   | 2     |
        | funding-source      | incomplete |       |
    And the order Section Status is incomplete

@5291
Scenario:10. Get the order section status is set when conditions are met 
    Given the user creates a new "<order-data-key>" order with id <order-id>
    When the user makes a request to retrieve the order summary with the ID <order-id>
    Then a response with status code 200 is returned
    And the order Section Status is <section-status>
    Examples: OrderData
        | order-id   | order-data-key                                               | section-status |
        | C000016-01 | complete                                                     | complete       |
        | C000016-01 | complete-with-1recipient-1associatedservice-fundingcomplete  | complete       |
        | C000017-01 | complete-with-0recipient-1associatedservice-fundingcomplete  | complete       |
        | C000018-01 | complete-with-1solution-1associatedservice-fundingcomplete   | complete       |
        | C000019-01 | complete-with-1solution-0associatedservice-fundingcomplete   | complete       |
        | C000020-01 | complete-with-1solution-1associatedservice-fundingincomplete | incomplete     |

@5321
Scenario: 11. If the order ID does not exist, return not found
    When the user makes a request to retrieve the order summary with the ID INVALID
    Then a response with status code 404 is returned

@5321
Scenario: 12. If a user is not authorised then they cannot access the order summary
    Given no user is logged in
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 401 is returned

@5321
Scenario: 13. A non buyer user cannot access the order summary
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 403 is returned

@5321
Scenario: 14. A buyer user cannot access the order summary for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 403 is returned

@5321
Scenario: 15. Service Failure
    Given the call to the database will fail
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 500 is returned
