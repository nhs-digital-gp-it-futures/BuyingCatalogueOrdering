Feature: order item PUT
    As a buyer user
    I want to create order items for a given order
    So that I can complete my order

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | Description      | OrderingPartyId                      | LastUpdatedByName | LastUpdatedBy                        | CommencementDate |
        | 10001   | Some Description | 4af62b99-638c-4247-875e-965239cd0c48 | Tom Smith         | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 01/01/2021       |
    And order progress exists
        | OrderId | AdditionalServicesViewd | AssociatedServicesViewed | CatalogueSolutionsViewed |
        | 10001   | false                   | false                    | false                    |
    And pricing units exist
        | Name    | Description |
        | patient | per patient |
    And catalogue items exist
        | Id       | Name       | CatalogueItemType | ParentCatalogueItemId |
        | 1000-001 | Sol 1      | Solution          | NULL                  |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@6036
Scenario: create order items
    Given the user creates a request to add the catalogue item with ID 1000-002 and the following details to the order with ID 10001
        | CatalogueItemName | CatalogueItemType   | CatalogueSolutionId   |
        | Item A            | <CatalogueItemType> | <CatalogueSolutionId> |
    And for the following recipients
        | OdsCode | Name        | Quantity | DeliveryDate |
        | ODS1    | Recipient 1 | 1        | 12/03/2021   |
        | ODS2    | Recipient 2 | 1        | 12/03/2021   |
    When the user sends the create order items request
    Then a response with status code 201 is returned
    And the expected order item is created
    And the order item has the expected recipients
    Examples: Item Types
        | CatalogueItemType | CatalogueSolutionId |
        | Solution          | NULL                |
        | AdditionalService | 1000-001            |
        | AssociatedService | NULL                |

@6036
Scenario: create an order item and the order audit information is updated
   Given the user creates a request to add the catalogue item with ID 1000-002 and the following details to the order with ID 10001
        | CatalogueItemName | CatalogueItemType |
        | Item A            | Solution          |
    And for the following recipients
        | OdsCode | Name        | Quantity | DeliveryDate |
        | ODS1    | Recipient 1 | 1        | 12/03/2021   |
    When the user sends the create order items request
    Then the order with ID 10001 is updated in the database with data
        | LastUpdatedBy                        | LastUpdatedByName |
        | 7b195137-6a59-4854-b118-62b39a3101ef | Bob Smith         |

@7840
Scenario: create catalogue solution order item and the catalogue solution order section should be marked as complete
    Given the user creates a request to add the catalogue item with ID 1000-002 and the following details to the order with ID 10001
        | CatalogueItemName | CatalogueItemType |
        | Item A            | Solution          |
    And for the following recipients
        | OdsCode | Name        | Quantity | DeliveryDate |
        | ODS1    | Recipient 1 | 1        | 12/03/2021   |
    When the user sends the create order items request
    Then a response with status code 201 is returned
    And the catalogue solution order section is marked as complete

@7840
Scenario: create additional service order item and the additional service order section should be marked as complete
    Given the user creates a request to add the catalogue item with ID 1000-002 and the following details to the order with ID 10001
        | CatalogueItemName | CatalogueItemType | CatalogueSolutionId |
        | Item A            | AdditionalService | 1000-001            |
    And for the following recipients
        | OdsCode | Name        | Quantity | DeliveryDate |
        | ODS1    | Recipient 1 | 1        | 12/03/2021   |
    When the user sends the create order items request
    Then a response with status code 201 is returned
    And the additional service order section is marked as complete

@7840
Scenario: create associated service order item and the associated service order section should be marked as complete
    Given the user creates a request to add the catalogue item with ID 1000-002 and the following details to the order with ID 10001
        | CatalogueItemName | CatalogueItemType |
        | Item A            | AssociatedService |
    And for the following recipients
        | OdsCode | Name        | Quantity | DeliveryDate |
        | ODS1    | Recipient 1 | 1        | 12/03/2021   |
    When the user sends the create order items request
    Then a response with status code 201 is returned
    And the associated service order section is marked as complete

@7840
Scenario: create order item should set the expected estimation period
    Given the user creates a request to add the catalogue item with ID 1000-002 and the following details to the order with ID 10001
        | CatalogueItemName | CatalogueItemType   | ProvisioningType   | EstimationPeriod   | CatalogueSolutionId |
        | Item A            | <CatalogueItemType> | <ProvisioningType> | <EstimationPeriod> | 1000-001            |
    And for the following recipients
        | OdsCode | Name        | Quantity | DeliveryDate |
        | ODS1    | Recipient 1 | 1        | 12/03/2021   |
    When the user sends the create order items request
    Then a response with status code 201 is returned
    And the order item estimation period is set to '<ExpectedEstimationPeriod>'

    Examples: Request payloads
        | CatalogueItemType | ProvisioningType | EstimationPeriod | ExpectedEstimationPeriod |
        | Solution          | OnDemand         | Month            | Month                    |
        | Solution          | OnDemand         | Year             | Year                     |
        | Solution          | Patient          | NULL             | Month                    |
        | Solution          | Declarative      | NULL             | Year                     |
        | AdditionalService | OnDemand         | Month            | Month                    |
        | AdditionalService | OnDemand         | Year             | Year                     |
        | AdditionalService | Patient          | NULL             | Month                    |
        | AdditionalService | Declarative      | NULL             | Year                     |
        | AssociatedService | OnDemand         | Month            | Month                    |
        | AssociatedService | OnDemand         | Year             | Year                     |
        | AssociatedService | Declarative      | NULL             | NULL                     |

@6036
Scenario: create an order item with an invalid order ID
    Given the user creates a request to add the catalogue item with ID 1000-002 and the following details to the order with ID 10000
        | CatalogueItemName | CatalogueItemType |
        | Item A            | Solution          |
    And for the following recipients
        | OdsCode | Name        | Quantity | DeliveryDate |
        | ODS1    | Recipient 1 | 1        | 12/03/2021   |
    When the user sends the create order items request
    Then a response with status code 404 is returned

@6036
Scenario: if a user is not authorized then they cannot create an order item
    Given no user is logged in
    And the user creates a request to add the catalogue item with ID 1000-002 and the following details to the order with ID 10001
        | CatalogueItemName | CatalogueItemType |
        | Item A            | Solution          |
    And for the following recipients
        | OdsCode | Name        | Quantity | DeliveryDate |
        | ODS1    | Recipient 1 | 1        | 12/03/2021   |
    When the user sends the create order items request
    Then a response with status code 401 is returned

@6036
Scenario: a non-buyer user cannot create an order item
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    Given the user creates a request to add the catalogue item with ID 1000-002 and the following details to the order with ID 10001
        | CatalogueItemName | CatalogueItemType |
        | Item A            | Solution          |
    And for the following recipients
        | OdsCode | Name        | Quantity | DeliveryDate |
        | ODS1    | Recipient 1 | 1        | 12/03/2021   |
    When the user sends the create order items request
    Then a response with status code 403 is returned

@6036
Scenario: a buyer user cannot create an order item for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    Given the user creates a request to add the catalogue item with ID 1000-002 and the following details to the order with ID 10001
        | CatalogueItemName | CatalogueItemType |
        | Item A            | Solution          |
    And for the following recipients
        | OdsCode | Name        | Quantity | DeliveryDate |
        | ODS1    | Recipient 1 | 1        | 12/03/2021   |
    When the user sends the create order items request
    Then a response with status code 403 is returned

@6036
Scenario: service failure
    Given the call to the database will fail
    Given the user creates a request to add the catalogue item with ID 1000-002 and the following details to the order with ID 10001
        | CatalogueItemName | CatalogueItemType |
        | Item A            | Solution          |
    And for the following recipients
        | OdsCode | Name        | Quantity | DeliveryDate |
        | ODS1    | Recipient 1 | 1        | 12/03/2021   |
    When the user sends the create order items request
    Then a response with status code 500 is returned
