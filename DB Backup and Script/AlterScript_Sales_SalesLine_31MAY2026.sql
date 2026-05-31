ALTER TABLE dbo.Sales
ADD
    SalesOrderStatus TINYINT NOT NULL CONSTRAINT DF_Sales_SalesOrderStatus DEFAULT (0),
    SentToProviderDate DATE NULL,
    SentToProviderUserId UNIQUEIDENTIFIER NULL,
    ProviderAcceptedDate DATE NULL,
    ProviderAcceptedUserId UNIQUEIDENTIFIER NULL,
    ProviderRejectedDate DATE NULL,
    ProviderRejectedUserId UNIQUEIDENTIFIER NULL,
    DeliveredDate DATE NULL,
    DeliveredDateUpdatedBy UNIQUEIDENTIFIER NULL,
    CancelledDate DATE NULL,
    CancelledReason NVARCHAR(300) NULL,
    CancelledByUserId UNIQUEIDENTIFIER NULL,
    OnHoldDate DATE NULL,
    OnHoldReason NVARCHAR(300) NULL,
    OnHoldByUserId UNIQUEIDENTIFIER NULL,
    SpecialNotes NVARCHAR(500) NULL,
    EmailSentToCustomer BIT NOT NULL CONSTRAINT DF_Sales_EmailSentToCustomer DEFAULT (0),
    EmailSentToProvider BIT NOT NULL CONSTRAINT DF_Sales_EmailSentToProvider DEFAULT (0),
    CompletedDate DATE NULL,
    ServiceStartDate DATE NULL,
    NextRenewDate DATE NULL,
    NoOfRenew INT NOT NULL CONSTRAINT DF_Sales_NoOfRenew DEFAULT (0),
    RenewNotes NVARCHAR(300) NULL,
    ManagerUserId UNIQUEIDENTIFIER NULL;
GO

ALTER TABLE dbo.SaleLines
ADD
    Completed BIT NOT NULL CONSTRAINT DF_SaleLines_Completed DEFAULT (0),
    CompletedDate DATE NULL,
    CancelledOrRejected BIT NOT NULL CONSTRAINT DF_SaleLines_CancelledOrRejected DEFAULT (0),
    CancelledOrRejectedDate DATE NULL;
GO

ALTER TABLE dbo.Sales
ADD CONSTRAINT FK_Sales_SentToProviderUser
FOREIGN KEY (SentToProviderUserId) REFERENCES dbo.AspNetUsers(Id);
GO

ALTER TABLE dbo.Sales
ADD CONSTRAINT FK_Sales_ProviderAcceptedUser
FOREIGN KEY (ProviderAcceptedUserId) REFERENCES dbo.AspNetUsers(Id);
GO

ALTER TABLE dbo.Sales
ADD CONSTRAINT FK_Sales_ProviderRejectedUser
FOREIGN KEY (ProviderRejectedUserId) REFERENCES dbo.AspNetUsers(Id);
GO

ALTER TABLE dbo.Sales
ADD CONSTRAINT FK_Sales_DeliveredDateUpdatedBy
FOREIGN KEY (DeliveredDateUpdatedBy) REFERENCES dbo.AspNetUsers(Id);
GO

ALTER TABLE dbo.Sales
ADD CONSTRAINT FK_Sales_CancelledByUser
FOREIGN KEY (CancelledByUserId) REFERENCES dbo.AspNetUsers(Id);
GO

ALTER TABLE dbo.Sales
ADD CONSTRAINT FK_Sales_OnHoldByUser
FOREIGN KEY (OnHoldByUserId) REFERENCES dbo.AspNetUsers(Id);
GO

ALTER TABLE dbo.Sales
ADD CONSTRAINT FK_Sales_ManagerUser
FOREIGN KEY (ManagerUserId) REFERENCES dbo.AspNetUsers(Id);
GO