CREATE TABLE dbo.Contact
(
    ContactId INT IDENTITY(1, 1) NOT NULL,
    FirstName NVARCHAR(100) NULL, 
    LastName NVARCHAR(100) NULL,
    Email NVARCHAR(256) NULL, 
    Phone NVARCHAR(35) NULL,
    CONSTRAINT PK_Contact PRIMARY KEY (ContactId)
);
