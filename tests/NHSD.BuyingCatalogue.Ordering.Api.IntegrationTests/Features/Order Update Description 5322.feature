Feature: Update an Orders Description in an Authority Section
	As an Authority User
	I want to update an orders description
	So that I can keep an order up to date

Background:
	Given Orders exist
		| OrderId    | Description      | OrderStatusId | Created    | LastUpdated | LastUpdatedBy                        | OrganisationId                       |
		| C000014-01 | Some Description | 1             | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |

@5322
Scenario: 1. Updating an orders description
	When the user makes a request to update the description with the ID C000014-01
		| Description         |
		| Another Description |
	Then a response with status code 204 is returned
	When the user makes a request to retrieve the order description section with the ID C000014-01
	Then a response with status code 200 is returned
	And the order description is returned
		| Description         |
		| Another Description |

@5322
Scenario: 2. Updating an order, with a non existent orderId returns not found
	When the user makes a request to update the description with the ID INVALID
		| Description         |
		| Another Description |
	Then a response with status code 404 is returned

@5322
Scenario: 3. Updating an order, with no description, returns a relevant error message
	When the user makes a request to update the description with the ID C000014-01
		| Description |
		| NULL        |
	Then a response with status code 400 is returned

@5322
Scenario: 4. Updating an order, with a description, exceeding it's maximum limit, returns a relevant error message
	When the user makes a request to update the description with the ID C000014-01
		| Description                     |
		| A string with the length of 101 |
	Then a response with status code 400 is returned

@ignore
@5322
Scenario: 5. If a user is not authorised, then they cannot update the orders description
When the user makes a request to update the description with the ID C000014-01
		| Description         |
		| Another Description |

@ignore
@5322
Scenario: 6. A non authority user cannot update an orders description
When the user makes a request to update the description with the ID C000014-01
		| Description         |
		| Another Description |

@5322
Scenario: 7. Service Failure
	Given the call to the database will fail
	When the user makes a request to update the description with the ID C000014-01
		| Description         |
		| Another Description |
	Then a response with status code 500 is returned