Feature: Order Summary Section Status
    As an authority user
    I want to be able to view the summary for a given order
    So that I can ensure that the information is complete

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
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

@5291
Scenario: the order section status is set for an order with one of each catalogue item type
    Given orders exist
        | OrderId | Description   | OrderingPartyId                      | FundingSourceOnlyGms | OrderStatus | Created    |
        | 10001   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | true                 | Incomplete  | 10/03/2021 |
    And order progress exists
        | OrderId | AdditionalServicesViewed | AssociatedServicesViewed | CatalogueSolutionsViewed | ServiceRecipientsViewed |
        | 10001   | true                     | true                     | true                     | true                    |
    And order items exist
        | OrderId | CatalogueItemId |
        | 10001   | 1000-001        |
        | 10001   | 1000-001-A01    |
        | 10001   | 1000-S-01       |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 1000-001        | ODS1    |
        | 10001   | 1000-001-A01    | ODS2    |
        | 10001   | 1000-S-01       | ODS3    |
    When the user makes a request to retrieve the order summary with the ID 10001
    Then a response with status code 200 is returned
    And the order Section Status is complete

@5291
Scenario: the order section status is set for an order with one associated service
    Given orders exist
        | OrderId | Description   | OrderingPartyId                      | FundingSourceOnlyGms | OrderStatus | Created    |
        | 10001   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | true                 | Incomplete  | 10/03/2021 |
    And order progress exists
        | OrderId | AdditionalServicesViewed | AssociatedServicesViewed | CatalogueSolutionsViewed | ServiceRecipientsViewed |
        | 10001   | true                     | true                     | true                     | true                    |
    And order items exist
        | OrderId | CatalogueItemId |
        | 10001   | 1000-S-01       |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 1000-S-01       | ODS1    |
    When the user makes a request to retrieve the order summary with the ID 10001
    Then a response with status code 200 is returned
    And the order Section Status is complete

@5291
Scenario: the order section status is set for an order with one associated service without recipients
    Given orders exist
        | OrderId | Description   | OrderingPartyId                      | FundingSourceOnlyGms | OrderStatus | Created    |
        | 10001   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | true                 | Incomplete  | 10/03/2021 |
    And order progress exists
        | OrderId | AdditionalServicesViewed | AssociatedServicesViewed | CatalogueSolutionsViewed | ServiceRecipientsViewed |
        | 10001   | true                     | true                     | true                     | true                    |
    And order items exist
        | OrderId | CatalogueItemId |
        | 10001   | 1000-S-01       |
    When the user makes a request to retrieve the order summary with the ID 10001
    Then a response with status code 200 is returned
    And the order Section Status is complete

@5291
Scenario: the order section status is set for an order with one solution and one associated service
    Given orders exist
        | OrderId | Description   | OrderingPartyId                      | FundingSourceOnlyGms | OrderStatus | Created    |
        | 10001   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | true                 | Incomplete  | 10/03/2021 |
    And order progress exists
        | OrderId | AdditionalServicesViewed | AssociatedServicesViewed | CatalogueSolutionsViewed | ServiceRecipientsViewed |
        | 10001   | true                     | true                     | true                     | true                    |
    And order items exist
        | OrderId | CatalogueItemId |
        | 10001   | 1000-001        |
        | 10001   | 1000-S-01       |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 1000-001        | ODS1    |
        | 10001   | 1000-S-01       | ODS2    |
    When the user makes a request to retrieve the order summary with the ID 10001
    Then a response with status code 200 is returned
    And the order Section Status is complete

@5291
Scenario: the order section status is set for an order with one solution only
    Given orders exist
        | OrderId | Description   | OrderingPartyId                      | FundingSourceOnlyGms | OrderStatus | Created    |
        | 10001   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | true                 | Incomplete  | 10/03/2021 |
    And order progress exists
        | OrderId | AdditionalServicesViewed | AssociatedServicesViewed | CatalogueSolutionsViewed | ServiceRecipientsViewed |
        | 10001   | true                     | true                     | true                     | true                    |
    And order items exist
        | OrderId | CatalogueItemId |
        | 10001   | 1000-001        |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 1000-001        | ODS1    |
    When the user makes a request to retrieve the order summary with the ID 10001
    Then a response with status code 200 is returned
    And the order Section Status is complete

@5291
Scenario: the order section status is set for an order with one solution and one associated service but incomplete funding source
    Given orders exist
        | OrderId | Description   | OrderingPartyId                      | FundingSourceOnlyGms | OrderStatus | Created    |
        | 10001   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | NULL                 | Incomplete  | 10/03/2021 |
    And order progress exists
        | OrderId | AdditionalServicesViewed | AssociatedServicesViewed | CatalogueSolutionsViewed | ServiceRecipientsViewed |
        | 10001   | true                     | true                     | true                     | true                    |
    And order items exist
        | OrderId | CatalogueItemId |
        | 10001   | 1000-001        |
        | 10001   | 1000-S-01       |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 1000-001        | ODS1    |
        | 10001   | 1000-S-01       | ODS2    |
    When the user makes a request to retrieve the order summary with the ID 10001
    Then a response with status code 200 is returned
    And the order Section Status is incomplete

@5291
Scenario: the order section status is set for a totally incomplete order
    Given orders exist
        | OrderId | Description   | OrderingPartyId                      | FundingSourceOnlyGms | OrderStatus | Created    |
        | 10001   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | NULL                 | Incomplete  | 10/03/2021 |
    And order progress exists
        | OrderId | AdditionalServicesViewed | AssociatedServicesViewed | CatalogueSolutionsViewed | ServiceRecipientsViewed |
        | 10001   | false                    | false                    | false                    | false                   |
    When the user makes a request to retrieve the order summary with the ID 10001
    Then a response with status code 200 is returned
    And the order Section Status is incomplete

@5291
Scenario: the order section status is set for an order with a selected service recipient
    Given orders exist
        | OrderId | Description   | OrderingPartyId                      | FundingSourceOnlyGms | OrderStatus | Created    |
        | 10001   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | true                 | Incomplete  | 10/03/2021 |
    And order progress exists
        | OrderId | AdditionalServicesViewed | AssociatedServicesViewed | CatalogueSolutionsViewed | ServiceRecipientsViewed |
        | 10001   | false                    | false                    | false                    | true                    |
    And order items exist
        | OrderId | CatalogueItemId | PriceTimeUnit | EstimationPeriod |
        | 10001   | 1000-001        | Month         | Month            |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 1000-001        | ODS1    |
    When the user makes a request to retrieve the order summary with the ID 10001
    Then a response with status code 200 is returned
    And the order Section Status is incomplete

@5291
Scenario: the order section status is set for an imcomplete order with one solution only
    Given orders exist
        | OrderId | Description   | OrderingPartyId                      | FundingSourceOnlyGms | OrderStatus | Created    |
        | 10001   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | true                 | Incomplete  | 10/03/2021 |
    And order progress exists
        | OrderId | AdditionalServicesViewed | AssociatedServicesViewed | CatalogueSolutionsViewed | ServiceRecipientsViewed |
        | 10001   | false                    | false                    | true                     | true                    |
    And order items exist
        | OrderId | CatalogueItemId |
        | 10001   | 1000-001        |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 1000-001        | ODS1    |
    When the user makes a request to retrieve the order summary with the ID 10001
    Then a response with status code 200 is returned
    And the order Section Status is incomplete

@5291
Scenario: the order section status is set for an incomplete order with one solution and one additional service
    Given orders exist
        | OrderId | Description   | OrderingPartyId                      | FundingSourceOnlyGms | OrderStatus | Created    |
        | 10001   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | true                 | Incomplete  | 10/03/2021 |
    And order progress exists
        | OrderId | AdditionalServicesViewed | AssociatedServicesViewed | CatalogueSolutionsViewed | ServiceRecipientsViewed |
        | 10001   | true                     | false                    | true                     | true                    |
    And order items exist
        | OrderId | CatalogueItemId |
        | 10001   | 1000-001        |
        | 10001   | 1000-001-A01    |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 1000-001        | ODS1    |
        | 10001   | 1000-001-A01    | ODS2    |
    When the user makes a request to retrieve the order summary with the ID 10001
    Then a response with status code 200 is returned
    And the order Section Status is incomplete

@5291
Scenario: the order section status is set for an incomplete order with one associated service only
    Given orders exist
        | OrderId | Description   | OrderingPartyId                      | FundingSourceOnlyGms | OrderStatus | Created    |
        | 10001   | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | NULL                 | Incomplete  | 10/03/2021 |
    And order progress exists
        | OrderId | AdditionalServicesViewed | AssociatedServicesViewed | CatalogueSolutionsViewed | ServiceRecipientsViewed |
        | 10001   | false                    | true                     | false                    | false                   |
    And order items exist
        | OrderId | CatalogueItemId |
        | 10001   | 1000-S-01       |
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode |
        | 10001   | 1000-S-01       | ODS1    |
    When the user makes a request to retrieve the order summary with the ID 10001
    Then a response with status code 200 is returned
    And the order Section Status is incomplete
