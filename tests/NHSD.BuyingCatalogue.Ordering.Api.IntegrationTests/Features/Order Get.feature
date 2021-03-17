Feature: get an order
    As a buyer user
    I want to be able to view a preview of a given order
    So that I can ensure that the information is complete

Background:
    Given addresses exist
        | Id | Line1 | Line2      | Line3      | Line4          | Line5           | Town      | County  | Postcode | Country        |
        | 1  | 4     | Upper Flat | Rocks Lane | Little Village | Bigger Village  | Some Town | W Yorks | LS1 3AP  | United Kingdom |
        | 2  | 8     | Lower Flat | Water Lane | Big Village    | Smaller Village | The Town  | E Yorks | LS2 6ZP  | United Kingdom |
    And contacts exist
        | Id | FirstName | LastName | Email                   | Phone       |
        | 1  | Fred      | Robinson | Fred.robinson@email.com | 12312543212 |
    And ordering parties exist
        | Id                                   | OdsCode | Name         | AddressId |
        | 4af62b99-638c-4247-875e-965239cd0c48 | 432432  | Hampshire CC | 2         |
    And suppliers exist
        | Id   | Name       | AddressId |
        | S123 | Supplier A | 1         |
    And pricing units exist
        | Name    | Description |
        | patient | per patient |
    And orders exist
        | OrderId | IsDeleted | Description   | OrderingPartyId                      | OrderingPartyContactId | SupplierId | SupplierContactId | Completed  | Created    | LastUpdated |
        | 10001   | false     | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | 1                      | S123       | 1                 | 15/05/2020 | 15/05/2020 | 15/05/2020  |
        | 10002   | true      | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | 1                      | S123       | 1                 | NULL       | 15/05/2020 | NULL        |
    And service recipients exist
        | OdsCode | Name    |
        | eu      | EU Test |
        | au      | UA Test |
    And catalogue items exist
        | Id        | Name | CatalogueItemType | ParentCatalogueItemId |
        | 10001-001 | Sol1 | Solution          | NULL                  |
        | 10001-002 | Sol1 | AssociatedService | NULL                  |
        | 10001-003 | Sol1 | AdditionalService | 10001-001             |
    And order items exist
        | OrderId | CatalogueItemId | Price   | ProvisioningType | PriceTimeUnit | EstimationPeriod | 
        | 10001   | 10001-001       | 461.34  | OnDemand         | Null          | Month            | 
        | 10001   | 10001-002       | 721.34  | Declarative      | Month         | Null             | 
        | 10001   | 10001-003       | 3532.12 | Patient          | Year          | Year             | 
    And order item recipients exist
        | OrderId | CatalogueItemId | OdsCode | Quantity | DeliveryDate |
        | 10001   | 10001-001       | eu      | 5        | 21/03/2021   |
        | 10001   | 10001-002       | au      | 2        | 15/07/2020   |
        | 10001   | 10001-003       | eu      | 1        | 15/07/2020   |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@8122
Scenario: get an order
    Given the user creates a request to retrieve the details of an order by ID 10001
    When the user sends the get order request
    Then a response with status code 200 is returned
    And the get order response displays the expected order

@10035
@10684
Scenario: the service instance ID is included for each order item
    Given the user creates a request to retrieve the details of an order by ID 10001
    When the user sends the get order request
    Then a response with status code 200 is returned
    And the get order response displays the expected order
    And the expected service instance ID is included for each order item as follows
        | ItemId          | ServiceInstanceId |
        | C010001-01-eu-1 | SI1-eu            |
        | C010001-01-au-2 | NULL              |
        | C010001-01-eu-3 | SI1-eu            |

@8122
Scenario: get a single deleted order returns not found
    Given the user creates a request to retrieve the details of an order by ID 10002
    When the user sends the get order request
    Then a response with status code 404 is returned

@8122
Scenario: a non-existent order ID returns not found
    Given the user creates a request to retrieve the details of an order by ID 10000
    When the user sends the get order request
    Then a response with status code 404 is returned

@8122
Scenario: if a user is not authorised then they cannot update an order
    Given no user is logged in
    Given the user creates a request to retrieve the details of an order by ID 10001
    When the user sends the get order request
    Then a response with status code 401 is returned

@8122
Scenario: a non-buyer user cannot update an order
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    Given the user creates a request to retrieve the details of an order by ID 10001
    When the user sends the get order request
    Then a response with status code 403 is returned

@8122
Scenario: a buyer user cannot update an order for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    Given the user creates a request to retrieve the details of an order by ID 10001
    When the user sends the get order request
    Then a response with status code 403 is returned

@8122
Scenario: service failure
    Given the call to the database will fail
    Given the user creates a request to retrieve the details of an order by ID 10001
    When the user sends the get order request
    Then a response with status code 500 is returned
