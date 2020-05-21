CREATE TABLE dbo.[Order]
(
    OrderId NVARCHAR(10) NOT NULL PRIMARY KEY,
    [Description] NVARCHAR(100) NOT NULL,
    OrganisationId UNIQUEIDENTIFIER NOT NULL,    
    OrganisationName NVARCHAR(256) NULL, 
    OrganisationOdsCode NVARCHAR(8) NULL, 
    OrganisationAddressId INT NULL,
    OrganisationBillingAddressId INT NULL,    
    OrganisationContactId INT NULL,
    OrderStatusId INT NOT NULL,    
    Created DATETIME2 NOT NULL,
    LastUpdated DATETIME2 NOT NULL,
    LastUpdatedBy UNIQUEIDENTIFIER NOT NULL,
    LastUpdatedByName NVARCHAR(256) NULL,     

    CONSTRAINT FK_Order_OrderStatusId_OrderStatus_OrderStatusId FOREIGN KEY (OrderStatusId) REFERENCES OrderStatus (OrderStatusId), 
    CONSTRAINT FK_Order_OrganisationAddressId_Address_AddressId FOREIGN KEY (OrganisationAddressId) REFERENCES Address (AddressId),
    CONSTRAINT FK_Order_OrganisationBillingAddressId_Address_AddressId FOREIGN KEY (OrganisationBillingAddressId) REFERENCES Address (AddressId),
    CONSTRAINT FK_Order_OrganisationContactId_Contact_ContactId FOREIGN KEY (OrganisationContactId) REFERENCES Contact (ContactId)
);
