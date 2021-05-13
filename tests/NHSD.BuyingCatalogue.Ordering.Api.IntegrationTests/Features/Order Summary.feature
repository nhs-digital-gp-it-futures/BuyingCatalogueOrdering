Feature: display the order summary in an authority section
    As an authority user
    I want to be able to view the summary for a given order
    So that I can ensure that the information is complete

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | Description   | OrderingPartyId                      | OrderStatus | Created    |
        | 10001   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | Complete    | 10/03/2021 |
    And pricing units exist
        | Name    | Description |
        | patient | per patient |
    And catalogue items exist
        | Id           | Name                 | CatalogueItemType | ParentCatalogueItemId |
        | 1000-001     | Solution 1           | Solution          | NULL                  |
        | 1000-001-A01 | Additional Service 1 | AdditionalService | 1000-001              |
        | 1000-S-01    | Associated Service 1 | AssociatedService | NULL                  |
    And service recipients exist
         | OdsCode | Name        |
         | ODS1    | Recipient 1 |
         | ODS2    | Recipient 2 |
         | ODS3    | Recipient 2 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5321
Scenario: get the order summary
    When the user makes a request to retrieve the order summary with the ID 10001
    Then a response with status code 200 is returned
    And the order summary is returned with the following values
        | OrderId    | OrganisationId                       | Description   | OrderStatus | Created    |
        | C010001-01 | 4af62b99-638c-4247-875e-965239cd0c48 | A Description | Complete    | 10/03/2021 |
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

@4619
Scenario: get the order summary when the order has a primary ordering party contact
    Given contacts exist
        | Id | FirstName | LastName | Email | Phone |
        | 1  | Fred      | Robinson | Fred.robinson@email.com | 12312543212 |
    And orders exist
        | OrderId | Description   | OrderingPartyId                      | OrderingPartyContactId | Created    |
        | 10002   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | 1                      | 10/03/2021 |
    When the user makes a request to retrieve the order summary with the ID 10002
    Then a response with status code 200 is returned
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

@4619
Scenario: get the order summary when the order has a primary supplier contact
    Given contacts exist
        | Id | FirstName | LastName | Email | Phone |
        | 1  | Fred      | Robinson | Fred.robinson@email.com | 12312543212 |
    And orders exist
        | OrderId | Description   | OrderingPartyId                      | SupplierContactId | Created    |
        | 10002   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | 1                 | 10/03/2021 |
    When the user makes a request to retrieve the order summary with the ID 10002
    Then a response with status code 200 is returned
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

@4619
Scenario: get the order summary when the order has a commencement date
    Given orders exist
        | OrderId | Description   | OrderingPartyId                      | CommencementDate | Created    |
        | 10002   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | 31/05/2020       | 10/03/2021 |
    When the user makes a request to retrieve the order summary with the ID 10002
    Then a response with status code 200 is returned
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

@5115
Scenario: get the order summary after a section has been viewed
    Given orders exist
        | OrderId | Description   | OrderingPartyId                      | FundingSourceOnlyGMS      | Created    |
        | 10002   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | <funding-source-only-gms> | 10/03/2021 |
    And order progress exists
        | OrderId | ServiceRecipientsViewed     | AdditionalServicesViewed     | CatalogueSolutionsViewed     | AssociatedServicesViewed     |
        | 10002   | <service-recipients-viewed> | <additional-services-viewed> | <catalogue-solutions-viewed> | <associated-services-viewed> |
    When the user makes a request to retrieve the order summary with the ID 10002
    Then a response with status code 200 is returned
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

    Examples: Sections
        | service-recipients-viewed | additional-services-viewed | catalogue-solutions-viewed | associated-services-viewed | funding-source-only-gms | service-recipients-status | additional-services-status | catalogue-solutions-status | associated-services-status | funding-source-status   |
        | True                      | False                      | False                      | False                      | NULL                    | complete                  | incomplete                 | incomplete                 | incomplete                 | incomplete              |
        | False                     | True                       | False                      | False                      | NULL                    | incomplete                | incomplete                 | incomplete                 | incomplete                 | incomplete              |
        | False                     | False                      | True                       | False                      | NULL                    | incomplete                | incomplete                 | complete                   | incomplete                 | incomplete              |
        | False                     | False                      | False                      | True                       | False                   | incomplete                | incomplete                 | incomplete                 | complete                   | incomplete              |
        | False                     | False                      | False                      | False                      | True                    | incomplete                | incomplete                 | incomplete                 | incomplete                 | incomplete              |

@4629
Scenario: get the order summary that includes a list of service recipients
    Given orders exist
        | OrderId | Description   | OrderingPartyId                      | Created    |
        | 10003   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | 10/03/2021 |
    And order progress exists
        | OrderId | ServiceRecipientsViewed |
        | 10003   | true                    |
    And catalogue items exist
        | Id        | Name         | CatalogueItemType |
        | 10003-001 | Order Item 1 | Solution          |
        | 10003-002 | Order Item 2 | Solution          |
    And order items exist
        | OrderId | CatalogueItemId |
        | 10003   | 10003-001       |
        | 10003   | 10003-002       |
     And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode | Quantity | DeliveryDate |
        | 10003   | 10003-001       | ODS1    | 5        | 21/03/2021   |
        | 10003   | 10003-002       | ODS2    | 5        | 21/03/2021   |
    When the user makes a request to retrieve the order summary with the ID 10003
    Then a response with status code 200 is returned
    And the order Summary Sections have the following values
        | Id                  | Status     | Count |
        | description         | complete   |       |
        | ordering-party      | incomplete |       |
        | supplier            | incomplete |       |
        | commencement-date   | incomplete |       |
        | associated-services | incomplete | 0     |
        | service-recipients  | complete   | 2     |
        | catalogue-solutions | incomplete | 2     |
        | additional-services | incomplete | 0     |
        | funding-source      | incomplete |       |

@5123
Scenario: get the order summary that includes a list of Catalogue Solutions
    Given orders exist
        | OrderId | Description   | OrderingPartyId                      | Created    |
        | 10003   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | 10/03/2021 |
    And order progress exists
        | OrderId | CatalogueSolutionsViewed |
        | 10003   | true                     |
    And catalogue items exist
        | Id        | Name         | CatalogueItemType |
        | 10003-001 | Order Item 1 | Solution          |
        | 10003-002 | Order Item 2 | Solution          |
    And order items exist
        | OrderId | CatalogueItemId |
        | 10003   | 10003-001       |
        | 10003   | 10003-002       |
     And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode | Quantity | DeliveryDate |
        | 10003   | 10003-001       | ODS1    | 5        | 21/03/2021   |
        | 10003   | 10003-002       | ODS2    | 5        | 21/03/2021   |
    When the user makes a request to retrieve the order summary with the ID 10003
    Then a response with status code 200 is returned
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

@5115
Scenario: get the order summary that includes a list of associated services
    Given orders exist
        | OrderId | Description   | OrderingPartyId                      | Created    |
        | 10003   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | 10/03/2021 |
    And order progress exists
        | OrderId | AssociatedServicesViewed |
        | 10003   | true                     |
    And catalogue items exist
        | Id        | Name              | CatalogueItemType |
        | 10003-001 | Associated Item 1 | AssociatedService |
        | 10003-002 | Associated Item 2 | AssociatedService |
    And order items exist
        | OrderId | CatalogueItemId |
        | 10003   | 10003-001       |
        | 10003   | 10003-002       |
    When the user makes a request to retrieve the order summary with the ID 10003
    Then a response with status code 200 is returned
    And the order Summary Sections have the following values
        | Id                  | Status     | Count |
        | description         | complete   |       |
        | ordering-party      | incomplete |       |
        | supplier            | incomplete |       |
        | commencement-date   | incomplete |       |
        | associated-services | complete   | 2     |
        | service-recipients  | incomplete | 0     |
        | catalogue-solutions | incomplete | 0     |
        | additional-services | incomplete | 0     |
        | funding-source      | incomplete |       |

@5115
Scenario: get the order summary that includes a list of additional services
    Given orders exist
        | OrderId | Description   | OrderingPartyId                      | Created    |
        | 10003   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | 10/03/2021 |
    And order progress exists
        | OrderId | AdditionalServicesViewed |
        | 10003   | true                     |   
    And catalogue items exist
        | Id        | Name              | CatalogueItemType |
        | 10003-001 | Additional Item 1 | AdditionalService |
        | 10003-002 | Additional Item 2 | AdditionalService |
        | 10003-003 | Order Item 2      | Solution          |
    And order items exist
        | OrderId | CatalogueItemId |
        | 10003   | 10003-001       |
        | 10003   | 10003-002       |
        | 10003   | 10003-003       |
    And service recipients exist
        | OdsCode | Name    |
        | eu      | EU Test |
     And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode | Quantity | DeliveryDate |
        | 10003   | 10003-003       | eu      | 5        | 21/03/2021   |
    When the user makes a request to retrieve the order summary with the ID 10003
    Then a response with status code 200 is returned
    And the order summary is returned with the following values
        | OrderId    | OrganisationId                       | Description   |
        | C010003-01 | 4af62b99-638c-4247-875e-965239cd0c48 | A Description |
    And the order Summary Sections have the following values
        | Id                  | Status     | Count |
        | description         | complete   |       |
        | ordering-party      | incomplete |       |
        | supplier            | incomplete |       |
        | commencement-date   | incomplete |       |
        | associated-services | incomplete | 0     |
        | service-recipients  | incomplete | 1     |
        | catalogue-solutions | incomplete | 1     |
        | additional-services | complete   | 2     |
        | funding-source      | incomplete |       |

@5321
Scenario: if the order ID does not exist, return not found
    When the user makes a request to retrieve the order summary with the ID 10000
    Then a response with status code 404 is returned

@5321
Scenario: if a user is not authorised then they cannot access the order summary
    Given no user is logged in
    When the user makes a request to retrieve the order summary with the ID 10001
    Then a response with status code 401 is returned

@5321
Scenario: a non-buyer user cannot access the order summary
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve the order summary with the ID 10001
    Then a response with status code 403 is returned

@5321
Scenario: a buyer user cannot access the order summary for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve the order summary with the ID 10001
    Then a response with status code 403 is returned

@5321
Scenario: service failure
    Given the call to the database will fail
    When the user makes a request to retrieve the order summary with the ID 10001
    Then a response with status code 500 is returned
