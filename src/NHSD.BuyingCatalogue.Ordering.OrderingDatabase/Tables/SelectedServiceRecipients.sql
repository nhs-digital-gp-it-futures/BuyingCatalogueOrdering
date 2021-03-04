-- Exists solely for non-bulk ordering
-- TODO: remove once bulk ordering is complete
CREATE TABLE dbo.SelectedServiceRecipients
(
    OrderId int NOT NULL,
    OdsCode nvarchar(8) NOT NULL,
    CONSTRAINT PK_SelectedServiceRecipients PRIMARY KEY (OrderId, OdsCode),
    CONSTRAINT FK_SelectedServiceRecipients_Order FOREIGN KEY (OrderId) REFERENCES dbo.[Order] (Id) ON DELETE CASCADE,
    CONSTRAINT FK_SelectedServiceRecipients_OdsCode FOREIGN KEY (OdsCode) REFERENCES dbo.ServiceRecipient (OdsCode),
);
