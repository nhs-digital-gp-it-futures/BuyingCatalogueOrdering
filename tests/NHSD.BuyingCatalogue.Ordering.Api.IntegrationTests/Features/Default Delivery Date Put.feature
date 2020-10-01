Feature: Default Delivery Date Put
    As a buyer
    I want to be able to set the default delivery date of a catalogue item
    To reduce the amount of effort required to place an order

Background:
    Given Orders exist
        | OrderId    | Description            | SupplierId | SupplierName | OrganisationId                       | OrganisationOdsCode | OrganisationName       | CommencementDate | CatalogueSolutionsViewed | AssociatedServicesViewed | FundingSourceOnlyGMS | Completed |
        | C000014-01 | Some Description       | 10101      | Supplier 1   | 4af62b99-638c-4247-875e-965239cd0c48 | OrgOds              | NHS NORTHUMBERLAND CCG | 15/12/2020       | True                     | True                     | True                 | NULL      |
        | C000014-02 | Some Other Description | 10101      | Supplier 1   | 4af62b99-638c-4247-875e-965239cd0c48 | OrgOds              | NHS NORTHUMBERLAND CCG | 01/10/2020       | True                     | True                     | NULL                 | NULL      |
        | C000014-03 | Some Description       | 10101      | Supplier 1   | 4af62b99-638c-4247-875e-965239cd0c48 | OrgOds              | NHS NORTHUMBERLAND CCG |                  | True                     | True                     | True                 | NULL      |
    And Service Recipients exist
        | OrderId    | OdsCode | Name    |
        | C000014-01 | eu      | EU Test |
        | C000014-02 | eu      | EU Test |
    And Order items exist
        | OrderId    | CatalogueItemId | CatalogueItemName | CatalogueItemType | OdsCode | PriceTimeUnit | EstimationPeriod | DeliveryDate | Price  | ProvisioningType |
        | C000014-01 | 10001-001       | Item 1            | Solution          | eu      | Month         | Month            | 05/09/2021   | 599.99 | Patient          |
        | C000014-01 | 10001-002       | Item 2            | Solution          | eu      | Year          | Year             | 24/12/2021   | NULL   | Patient          |
        | C000014-02 | 10001-001       | Item 3            | Solution          | eu      | Month         | Month            | NULL         | NULL   | Declarative      |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@8952
Scenario: Set the default delivery date of a catalogue item
    Given the user sets the default delivery date using the following details
        | OrderId    | CatalogueItemId | PriceId | DeliveryDate |
        | C000014-02 | 10001-001       | 1       | 31/10/2020   |
    When the user confirms the default delivery date
    Then a response with status code 201 is returned
    And the default delivery date is set correctly

@8952
Scenario: Update the default delivery date of a catalogue item
    Given the following default delivery dates have already been set
        | OrderId    | CatalogueItemId | PriceId | DeliveryDate |
        | C000014-01 | 10001-001       | 1       | 31/10/2020   |
        | C000014-01 | 10001-002       | 2       | 31/10/2020   |
    And the user sets the default delivery date using the following details
        | OrderId    | CatalogueItemId | PriceId | DeliveryDate |
        | C000014-01 | 10001-002       | 2       | 01/01/2021   |
    When the user confirms the default delivery date
    Then a response with status code 200 is returned
    And the default delivery date is set correctly
    And the following default delivery dates remain unchanged
        | OrderId    | CatalogueItemId | PriceId | DeliveryDate |
        | C000014-01 | 10001-001       | 1       | 31/10/2020   |

@8952
Scenario: If a user is not authorized then they cannot set a default delivery date
    Given no user is logged in
    And the user sets the default delivery date using the following details
        | OrderId    | CatalogueItemId | PriceId | DeliveryDate |
        | C000014-02 | 10001-001       | 1       | 31/10/2020   |
    When the user confirms the default delivery date
    Then a response with status code 401 is returned

@8952
Scenario: A non buyer user cannot set a default delivery date
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    And the user sets the default delivery date using the following details
        | OrderId    | CatalogueItemId | PriceId | DeliveryDate |
        | C000014-02 | 10001-001       | 1       | 31/10/2020   |
    When the user confirms the default delivery date
    Then a response with status code 403 is returned

@8952
Scenario: A user with read only permissions cannot set a default delivery date
    Given the user is logged in with the Readonly-Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    And the user sets the default delivery date using the following details
        | OrderId    | CatalogueItemId | PriceId | DeliveryDate |
        | C000014-02 | 10001-001       | 1       | 31/10/2020   |
    When the user confirms the default delivery date
    Then a response with status code 403 is returned

@8952
Scenario: Set a default delivery date for an order that does not exist
    Given the user sets the default delivery date using the following details
        | OrderId    | CatalogueItemId | PriceId | DeliveryDate |
        | C000015-01 | 10001-001       | 1       | 31/10/2020   |
    When the user confirms the default delivery date
    Then a response with status code 404 is returned

@8952
Scenario: Set a default delivery date for an order that does not have a commencement date
    Given the user sets the default delivery date using the following details
        | OrderId    | CatalogueItemId | PriceId | DeliveryDate |
        | C000014-03 | 10001-001       | 1       | 31/10/2020   |
    When the user confirms the default delivery date
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id                       |
        | CommencementDateRequired |

@8952
Scenario: A default delivery date is not specified
    Given the user sets the default delivery date using the following details
        | OrderId    | CatalogueItemId | PriceId |
        | C000014-02 | 10001-001       | 1       |
    When the user confirms the default delivery date
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id                   | field         |
        | DeliveryDateRequired | DeliveryDate  |

@8952
Scenario: Set a default delivery date before the commencement date of an order
    Given the user sets the default delivery date using the following details
        | OrderId    | CatalogueItemId | PriceId | DeliveryDate |
        | C000014-02 | 10001-001       | 1       | 30/09/2020   |
    When the user confirms the default delivery date
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id                                | field         |
        | DeliveryDateOutsideDeliveryWindow | DeliveryDate  |

@8952
Scenario: Set a default delivery date five years after the commencement date of an order
    Given the user sets the default delivery date using the following details
        | OrderId    | CatalogueItemId | PriceId | DeliveryDate |
        | C000014-02 | 10001-001       | 1       | 31/12/2025   |
    When the user confirms the default delivery date
    Then a response with status code 400 is returned
    And the response contains the following errors
        | id                                | field         |
        | DeliveryDateOutsideDeliveryWindow | DeliveryDate  |

@8952
Scenario: A service failure causes the expected response to be returned when setting a default delivery date
    Given the call to the database will fail
    And the user sets the default delivery date using the following details
        | OrderId    | CatalogueItemId | PriceId | DeliveryDate |
        | C000014-02 | 10001-001       | 1       | 31/10/2020   |
    When the user confirms the default delivery date
    Then a response with status code 500 is returned
