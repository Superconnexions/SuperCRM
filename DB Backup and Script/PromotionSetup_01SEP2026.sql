SET XACT_ABORT ON;
BEGIN TRANSACTION;

CREATE TABLE dbo.PromotionSetup (
    PromotionId uniqueidentifier NOT NULL,
    PromotionCode varchar(10) NOT NULL,
    PromotionName nvarchar(200) NOT NULL,
    PromotionSummary nvarchar(500) NULL,
    EmailNotificationToAgents bit NOT NULL CONSTRAINT DF_PromotionSetup_EmailNotification DEFAULT(0),
    NoOfEmailNotificationSend tinyint NOT NULL CONSTRAINT DF_PromotionSetup_EmailCount DEFAULT(0),
    NotificationMessage nvarchar(2000) NULL,
    PromotionStartDate date NOT NULL,
    PromotionEndDate date NOT NULL,
    Published bit NOT NULL CONSTRAINT DF_PromotionSetup_Published DEFAULT(0),
    Cancelled bit NOT NULL CONSTRAINT DF_PromotionSetup_Cancelled DEFAULT(0),
    IsActive bit NOT NULL CONSTRAINT DF_PromotionSetup_IsActive DEFAULT(1),
    Submitted bit NOT NULL CONSTRAINT DF_PromotionSetup_Submitted DEFAULT(1),
    SubmittedAt datetime2 NOT NULL,
    SubmittedByUserId uniqueidentifier NOT NULL,
    UpdatedAt datetime2 NULL,
    UpdatedByUserId uniqueidentifier NULL,
    LastEmailNotificationAt datetime2 NULL,
    LastEmailNotificationByUserId uniqueidentifier NULL,
    Remarks nvarchar(500) NULL,
    CONSTRAINT PK_PromotionSetup PRIMARY KEY (PromotionId),
    CONSTRAINT UQ_PromotionSetup_PromotionCode UNIQUE (PromotionCode),
    CONSTRAINT CK_PromotionSetup_Date CHECK (PromotionEndDate >= PromotionStartDate),
    CONSTRAINT FK_PromotionSetup_SubmittedBy FOREIGN KEY (SubmittedByUserId) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_PromotionSetup_UpdatedBy FOREIGN KEY (UpdatedByUserId) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_PromotionSetup_LastEmailBy FOREIGN KEY (LastEmailNotificationByUserId) REFERENCES dbo.AspNetUsers(Id)
);
CREATE INDEX IX_PromotionSetup_StatusDates ON dbo.PromotionSetup(IsActive,Cancelled,Published,PromotionStartDate,PromotionEndDate);

CREATE TABLE dbo.PromotionItem (
    PromotionItemId uniqueidentifier NOT NULL,
    PromotionId uniqueidentifier NOT NULL,
    PromotionCode varchar(10) NOT NULL,
    ProductBaseCommissionId uniqueidentifier NOT NULL,
    ProductId uniqueidentifier NOT NULL,
    StandardCommission decimal(18,2) NOT NULL,
    PromotionType tinyint NOT NULL,
    PromotionPercentage decimal(9,4) NULL,
    PromotionAmount decimal(18,2) NOT NULL,
    FinalCommissionAmount decimal(18,2) NOT NULL,
    IsActive bit NOT NULL CONSTRAINT DF_PromotionItem_IsActive DEFAULT(1),
    UpdatedAt datetime2 NULL,
    UpdatedByUserId uniqueidentifier NULL,
    CONSTRAINT PK_PromotionItem PRIMARY KEY(PromotionItemId),
    CONSTRAINT FK_PromotionItem_Promotion FOREIGN KEY(PromotionId) REFERENCES dbo.PromotionSetup(PromotionId),
    CONSTRAINT FK_PromotionItem_ProductBaseCommission FOREIGN KEY(ProductBaseCommissionId) REFERENCES dbo.ProductBaseCommission(ProductBaseCommissionId),
    CONSTRAINT FK_PromotionItem_Product FOREIGN KEY(ProductId) REFERENCES dbo.Products(ProductId),
    CONSTRAINT FK_PromotionItem_UpdatedBy FOREIGN KEY(UpdatedByUserId) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT CK_PromotionItem_Type CHECK(PromotionType IN (1,2)),
    CONSTRAINT CK_PromotionItem_Amounts CHECK(StandardCommission>=0 AND PromotionAmount>=0 AND FinalCommissionAmount>=0),
    CONSTRAINT CK_PromotionItem_Percentage CHECK(PromotionPercentage IS NULL OR PromotionPercentage>=0)
);
CREATE UNIQUE INDEX UQ_PromotionItem_Promotion_Product ON dbo.PromotionItem(PromotionId,ProductId);
CREATE INDEX IX_PromotionItem_ProductBaseCommission ON dbo.PromotionItem(ProductBaseCommissionId);
CREATE INDEX IX_PromotionItem_Product ON dbo.PromotionItem(ProductId);
COMMIT TRANSACTION;


----------Step-2

ALTER TABLE dbo.PromotionSetup
ADD CancelledAt DATETIME2 NULL,
    CancelledByUserId UNIQUEIDENTIFIER NULL;
GO

ALTER TABLE dbo.PromotionSetup
ADD CONSTRAINT FK_PromotionSetup_CancelledByUser
    FOREIGN KEY (CancelledByUserId)
    REFERENCES dbo.AspNetUsers(Id);
GO


CREATE INDEX IX_PromotionSetup_CancelledByUserId
ON dbo.PromotionSetup(CancelledByUserId);
GO

