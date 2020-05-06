CREATE TABLE dbo.[Order]
(
	OrderId NVARCHAR(10) NOT NULL PRIMARY KEY,
	[Description] NVARCHAR(50) NOT NULL,
	OrganisationId UNIQUEIDENTIFIER NOT NULL,
	StatusId INT NOT NULL,
	Created DATETIME2 NOT NULL,
	LastUpdated DATETIME2 NOT NULL,
	LastUpdatedBy UNIQUEIDENTIFIER NOT NULL

	CONSTRAINT FK_Order_StatusId_OrderStatus_OrderStatusId FOREIGN KEY (StatusId) REFERENCES OrderStatus (OrderStatusId)
)
