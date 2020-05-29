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
    SupplierId NVARCHAR(6) NULL,
    SupplierName NVARCHAR(255) NULL,
    SupplierAddressId INT NULL,
    SupplierContactId INT NULL,
    CommencementDate DATETIME2 NULL,
    Created DATETIME2 NOT NULL,
    LastUpdated DATETIME2 NOT NULL,
    LastUpdatedBy UNIQUEIDENTIFIER NOT NULL,
    LastUpdatedByName NVARCHAR(256) NULL,
    
    CONSTRAINT FK_Order_OrderStatus FOREIGN KEY (OrderStatusId) REFERENCES OrderStatus (OrderStatusId),
    CONSTRAINT FK_Order_OrganisationAddress FOREIGN KEY (OrganisationAddressId) REFERENCES [Address] (AddressId),
    CONSTRAINT FK_Order_OrganisationBillingAddress FOREIGN KEY (OrganisationBillingAddressId) REFERENCES [Address] (AddressId),
    CONSTRAINT FK_Order_OrganisationContact FOREIGN KEY (OrganisationContactId) REFERENCES Contact (ContactId),
    CONSTRAINT FK_Order_SupplierAddress FOREIGN KEY (SupplierAddressId) REFERENCES [Address] (AddressId),
    CONSTRAINT FK_Order_SupplierContact FOREIGN KEY (SupplierContactId) REFERENCES Contact (ContactId)
);
