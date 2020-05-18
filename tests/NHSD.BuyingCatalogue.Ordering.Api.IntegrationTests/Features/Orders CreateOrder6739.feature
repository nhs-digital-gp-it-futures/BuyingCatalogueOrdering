Feature: Create Order
    As a Buyer User
    I want to create an orders for a given organisation
    So that I can add information to the order


Scenario: 1. A user can create a order and a incremented order id is returned;
	Given Orders exist
		| OrderId    | Description          | OrderStatusId | Created    | LastUpdated | LastUpdatedBy                        | OrganisationId                       |
		| C000014-01 | Some Description     | 1             | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
	When a POST request is made to create a order 
		| OrganisationId                       | Description                         |
		| 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 2 |
	Then a response with status code 201 is returned
	And a create order response is returned with the OrderId C000015-01

Scenario: 2. A user creates mutiple orders and order id is incremented multiple times returned;
	Given Orders exist
		| OrderId    | Description          | OrderStatusId | Created    | LastUpdated | LastUpdatedBy                        | OrganisationId                       |
		| C000014-01 | Some Description     | 1             | 11/05/2020 | 11/05/2020  | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
	When a POST request is made to create a order 
		| OrganisationId                       | Description                         |
		| 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 2 |
	And a POST request is made to create a order 
		| OrganisationId                       | Description                         |
		| 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 2 |
	Then a response with status code 201 is returned
	And a create order response is returned with the OrderId C000016-01


Scenario: 3. A user can create a ordrder when no orders exist and a defualt OrderId is returned;
	When a POST request is made to create a order 
		| OrganisationId                       | Description                         |
		| 4af62b99-638c-4247-875e-965239cd0c48 | This is an order for organisation 2 |
	Then a response with status code 201 is returned
	And a create order response is returned with the OrderId C000000-01

Scenario: 4. A user create an order without specifing an Organisation Id a Status Code of 400 is returned
	When a POST request is made to create a order
		| Description                         |
		| This is an order for organisation 2 |
	Then a response with status code 400 is returned
     And the response contains the following errors
		| ErrorMessageId         | FieldName      |
		| OrganisationIdRequired | OrganisationId |

Scenario: 5. A user create an order without specifing a description a Status Code of 400 is returned
	When a POST request is made to create a order
		| OrganisationId                       |
		| 4af62b99-638c-4247-875e-965239cd0c48 |
	Then a response with status code 400 is returned
     And the response contains the following errors
		| ErrorMessageId           | FieldName   |
		| OrderDescriptionRequired | Description |

Scenario: 6. A user attempts to create a order with a description that is too long,  a Status Code of 400 is returned;
	When a POST request is made to create a order 
		| OrganisationId                       | Description                                                                                                                        |
		| 4af62b99-638c-4247-875e-965239cd0c48 | This description is too long 12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901 |
	Then a response with status code 400 is returned
     And the response contains the following errors
		| ErrorMessageId          | FieldName   |
		| OrderDescriptionTooLong | Description |

Scenario: 7. A user attempts to create a order with multiple errors, all errors are returned;
	When a POST request is made to create a order 
		| Description                                                                                                                        |
		| This description is too long 12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901 |
	Then a response with status code 400 is returned
     And the response contains the following errors
		| ErrorMessageId          | FieldName      |
		| OrganisationIdRequired  | OrganisationId |
		| OrderDescriptionTooLong | Description    |

