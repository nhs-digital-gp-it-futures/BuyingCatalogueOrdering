Feature: Create Order
    As a Buyer User
    I want to create an orders for a given organisation
    So that I can add information to the order

Background:
	Given Orders exist
		| OrderId    | Description          | OrderStatusId | Created    | LastUpdated | LastUpdatedBy                        | OrganisationId                       |
		| C000014-01 | Some Description     | 1             | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
		| C000014-02 | Another Description  | 2             | 05/05/2020 | 09/05/2020  | a11a46f9-ce6f-448a-95c2-fde6e61c804a | 4af62b99-638c-4247-875e-965239cd0c48 |
		| C000014-03 | One more Description | 2             | 15/05/2020 | 19/05/2020  | a11a46f9-ce6f-448a-95c2-fde6e61c804a | e6ea864e-ef1b-41aa-a4d5-04fc6fce0933 |


@3538
Scenario: 1. A user can create a order
	When a POST request is made to create a order
		| OrganisationId                       | Description                         |
		| 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 2 |
	Then a response with status code 200 is returned
	And a create order response is returned with the OrderId C000015-01

