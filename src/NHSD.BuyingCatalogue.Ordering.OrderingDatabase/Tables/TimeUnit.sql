CREATE TABLE dbo.TimeUnit
(
    Id int NOT NULL,
    [Name] nvarchar(20) NOT NULL,
    [Description] nvarchar(32) NOT NULL,
    CONSTRAINT PK_TimeUnit PRIMARY KEY (Id),
    CONSTRAINT AK_TimeUnit_Name UNIQUE NONCLUSTERED ([Name]),
    CONSTRAINT AK_TimeUnit_Description UNIQUE NONCLUSTERED ([Description]),
);
