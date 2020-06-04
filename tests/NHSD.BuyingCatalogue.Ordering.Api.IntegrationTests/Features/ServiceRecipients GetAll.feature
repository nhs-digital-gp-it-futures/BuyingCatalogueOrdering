Feature: Display all of the Service Recipients for an order
    As a Buyer
    I want to view all of the service recipients for an order
    So that I can ensure the information is correct

Background:
    Given Orders exist
        | OrderId    | Description         | OrderStatusId | LastUpdatedByName | LastUpdatedBy                        | OrganisationId                       |
        | C000014-01 | Some Description    | 1             | Bob Smith         | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
        | C000014-02 | Another Description | 2             | Alice Smith       | a11a46f9-ce6f-448a-95c2-fde6e61c804a | 4af62b99-638c-4247-875e-965239cd0c48 |
    Given Service Recipients exist
        | OdsCode | Name                      | OrderId    |
        | Ods1    | Updated Service Recipient | C000014-01 |
        | Ods2    | Another Name              | C000014-02 |
        | Ods3    | Service Recipients        | C000014-01 |

#@7412
#Scenario: 1. Get the service recipients from an exisiting ordering ID
#    When the user makes a request to retrieve the service-recipients section
