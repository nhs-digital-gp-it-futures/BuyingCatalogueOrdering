Feature: Default Delivery Date Get
    As a buyer
    I want to be able to get the default delivery date of a catalogue item
    So that I can edit it

Background:
    Given Orders exist
        | OrderId    | Description            | SupplierId | SupplierName | OrganisationId                       | OrganisationOdsCode | OrganisationName       | CommencementDate | CatalogueSolutionsViewed | AssociatedServicesViewed | FundingSourceOnlyGMS | Completed |
        | C000014-01 | Some Description       | 10101      | Supplier 1   | 4af62b99-638c-4247-875e-965239cd0c48 | OrgOds              | NHS NORTHUMBERLAND CCG | 15/12/2020       | True                     | True                     | True                 | NULL      |
    And Service Recipients exist
        | OrderId    | OdsCode | Name    |
        | C000014-01 | eu      | EU Test |
    And Order items exist
        | OrderId    | CatalogueItemId | CatalogueItemName | CatalogueItemType | OdsCode | PriceTimeUnit | EstimationPeriod | DeliveryDate | Price  | ProvisioningType |
        | C000014-01 | 10001-001       | Item 1            | Solution          | eu      | Month         | Month            | 05/09/2021   | 599.99 | Patient          |
        | C000014-01 | 10001-002       | Item 2            | Solution          | eu      | Year          | Year             | 24/12/2021   | NULL   | Patient          |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@8952
Scenario: Get an existing default delivery date of a catalogue item
    Given the following default delivery dates have already been set
        | OrderId    | CatalogueItemId | PriceId | DeliveryDate |
        | C000014-01 | 10001-001       | 1       | 31/12/2020   |
    When the user gets the default delivery date for the catalogue item with the following details
        | OrderId    | CatalogueItemId | PriceId |
        | C000014-01 | 10001-001       | 1       |
    Then a response with status code 200 is returned
    And the default delivery date returned is 31/12/2020

@8952
Scenario: Get a default delivery date that does not exist
    When the user gets the default delivery date for the catalogue item with the following details
        | OrderId    | CatalogueItemId | PriceId |
        | C000014-01 | 10001-001       | 1       |
    Then a response with status code 404 is returned

@8952
Scenario: If a user is not authorized then they cannot get a default delivery date
    Given no user is logged in
    When the user gets the default delivery date for the catalogue item with the following details
        | OrderId    | CatalogueItemId | PriceId |
        | C000014-01 | 10001-001       | 1       |
    Then a response with status code 401 is returned

@8952
Scenario: A non buyer user cannot get a default delivery date
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user gets the default delivery date for the catalogue item with the following details
        | OrderId    | CatalogueItemId | PriceId |
        | C000014-01 | 10001-001       | 1       |
    Then a response with status code 403 is returned

@8952
Scenario: A service failure causes the expected response to be returned when getting a default delivery date
    Given the call to the database will fail
    When the user gets the default delivery date for the catalogue item with the following details
        | OrderId    | CatalogueItemId | PriceId |
        | C000014-01 | 10001-001       | 1       |
    Then a response with status code 500 is returned
