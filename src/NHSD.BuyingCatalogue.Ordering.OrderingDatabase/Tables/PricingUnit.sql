CREATE TABLE dbo.PricingUnit
(
    [Name] nvarchar(20) NOT NULL,
    [Description] nvarchar(100) NOT NULL,
    CONSTRAINT PK_PricingUnit PRIMARY KEY ([Name]),
);
