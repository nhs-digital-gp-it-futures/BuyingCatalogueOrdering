Feature: Delete the order items for an order
    As a Buyer User
    I want to update the order items for a given order
    So that I can keep my order up to date

Background:
	Given ordering parties exist
		| Id                                   |
		| 4af62b99-638c-4247-875e-965239cd0c48 |
	Given orders exist
		| OrderId | Description      | OrderingPartyId                      | LastUpdatedBy                        | LastUpdatedByName | OrganisationId                       | CommencementDate |
		| 10001   | Some Description | 4af62b99-638c-4247-875e-965239cd0c48 | 335392e4-4bb1-413b-9de5-36a85c9c0422 | Tom Smith         | 4af62b99-638c-4247-875e-965239cd0c48 | 01/01/2021       |
	And pricing units exist
        | Name    | Description |
        | patient | per patient |
    And service recipients exist
		| OdsCode | Name                |
		| ODS1    | Service Recipient 1 |
		| ODS2    | Service Recipient 2 |
		| ODS3    | Service Recipient 3 |
	And catalogue items exist
		| Id         | Name   | CatalogueItemType | ParentCatalogueItemId |
		| 100001-001 | Item 1 | Solution          | NULL                  |
		| 200002-002 | Item 2 | Solution          | NULL                  |
		| 300003-003 | Item 3 | AdditionalService | 100001-001            |
	And order items exist
		| OrderId | OdsCode | CatalogueItemId | CatalogueItemName | CatalogueItemType | ProvisioningType |
		| 10001   | ODS1    | 100001-001      | Order Item 1      | Solution          | OnDemand         |
		| 10001   | ODS2    | 200002-002      | Order Item 2      | Solution          | Declarative      |
		| 10001   | ODS3    | 300003-003      | Order Item 3      | Solution          | OnDemand         |
	And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@9038
Scenario: Delete a catalogue item with additional service
	Given the user creates a request to delete order item with catalogue item ID 100001-001 for order ID 10001
	When the user sends the delete order item request
	Then a response with status code 204 is returned
	And the expected order items and additional services are deleted

@9038
Scenario: Delete a catalogue item that does not exist
	Given the user creates a request to delete order item with catalogue item ID 234567-890 for order ID 10001
	When the user sends the delete order item request
	Then a response with status code 404 is returned
	And the order is not updated

@9038
Scenario: If a user is not authorized then they cannot delete a catalogue item
	Given no user is logged in
	And the user creates a request to delete order item with catalogue item ID 200002-002 for order ID 10001
	When the user sends the delete order item request
	Then a response with status code 401 is returned

@9038
Scenario: A non buyer user cannot delete a catalogue item
	Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
	And the user creates a request to delete order item with catalogue item ID 200002-002 for order ID 10001
	When the user sends the delete order item request
	Then a response with status code 403 is returned

@9038
Scenario: A buyer user cannot delete a catalogue item for an organisation they don't belong to
	Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
	And the user creates a request to delete order item with catalogue item ID 200002-002 for order ID 10001
	When the user sends the delete order item request
	Then a response with status code 403 is returned

@9038
Scenario: Service failure
	Given the call to the database will fail
	And the user creates a request to delete order item with catalogue item ID 200002-002 for order ID 10001
	When the user sends the delete order item request
	Then a response with status code 500 is returned
