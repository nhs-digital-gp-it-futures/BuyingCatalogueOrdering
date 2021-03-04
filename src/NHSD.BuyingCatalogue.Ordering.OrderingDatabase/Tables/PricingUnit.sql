CREATE TABLE dbo.PricingUnit
(
    [Name] nvarchar(20) NOT NULL,
    [Description] nvarchar(40) NOT NULL,
    CONSTRAINT PK_PricingUnit PRIMARY KEY ([Name]),
);
