CREATE TABLE dbo.ProvisioningType
(
    ProvisioningTypeId int NOT NULL,
    [Name] nvarchar(35) NOT NULL,
    CONSTRAINT PK_ProvisioningType PRIMARY KEY (ProvisioningTypeId),
    CONSTRAINT IX_ProvisioningTypeName UNIQUE NONCLUSTERED ([Name])
);
