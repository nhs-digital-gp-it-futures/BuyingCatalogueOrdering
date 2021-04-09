Feature: default delivery date GET
    As a buyer
    I want to be able to get the default delivery date of a catalogue item
    So that I can edit it

Background:
    Given ordering parties exist
        | Id                                   |
        | 4af62b99-638c-4247-875e-965239cd0c48 |
    And orders exist
        | OrderId | Description      | OrderingPartyId                      | CommencementDate |
        | 10001   | Some Description | 4af62b99-638c-4247-875e-965239cd0c48 | 15/12/2020       |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@8952
Scenario: get an existing default delivery date of a catalogue item
    Given the following default delivery dates have already been set
        | OrderId | CatalogueItemId | DeliveryDate |
        | 10001   | 10001-001       | 31/12/2020   |
    When the user gets the default delivery date for the catalogue item with the following details
        | OrderId | CatalogueItemId |
        | 10001   | 10001-001       |
    Then a response with status code 200 is returned
    And the default delivery date returned is 31/12/2020

@8952
Scenario: get a default delivery date that does not exist
    When the user gets the default delivery date for the catalogue item with the following details
        | OrderId | CatalogueItemId |
        | 10001   | 10001-001       |
    Then the default delivery date does not exists
    
@8952
Scenario: if a user is not authorized then they cannot get a default delivery date
    Given no user is logged in
    When the user gets the default delivery date for the catalogue item with the following details
        | OrderId | CatalogueItemId |
        | 10001   | 10001-001       |
    Then a response with status code 401 is returned

@8952
Scenario: a non-buyer user cannot get a default delivery date
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user gets the default delivery date for the catalogue item with the following details
        | OrderId | CatalogueItemId |
        | 10001   | 10001-001       |
    Then a response with status code 403 is returned

@8952
Scenario: a buyer user cannot get the default delivery of an order for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user gets the default delivery date for the catalogue item with the following details
        | OrderId | CatalogueItemId |
        | 10001   | 10001-001       |
    Then a response with status code 403 is returned

@8952
Scenario: a service failure causes the expected response to be returned when getting a default delivery date
    Given the call to the database will fail
    When the user gets the default delivery date for the catalogue item with the following details
        | OrderId | CatalogueItemId |
        | 10001   | 10001-001       |
    Then a response with status code 500 is returned
