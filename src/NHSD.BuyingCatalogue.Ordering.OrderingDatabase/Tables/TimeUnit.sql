CREATE TABLE dbo.TimeUnit
(
    TimeUnitId int NOT NULL,
    [Name] varchar(20) NOT NULL,
    [Description] varchar(32) NOT NULL,
    CONSTRAINT PK_TimeUnit PRIMARY KEY (TimeUnitId)
);