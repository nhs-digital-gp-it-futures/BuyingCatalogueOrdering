CREATE TABLE dbo.ProvisioningType
(
    Id int NOT NULL,
    [Name] nvarchar(35) NOT NULL,
    CONSTRAINT PK_ProvisioningType PRIMARY KEY (Id),
    CONSTRAINT AK_ProvisioningType_Name UNIQUE NONCLUSTERED ([Name]),
);
