ALTER TABLE dbo.Providers
    ADD ProviderURL NVARCHAR(100) NULL;


 ALTER TABLE dbo.Providers
    ADD ProviderAddress NVARCHAR(200) NULL;


ALTER TABLE dbo.ProductVariants
    ADD DisplayOrder INT NULL;


ALTER TABLE dbo.ProductBaseCommission
ADD 
    CreatedAt DATETIME2 NOT NULL 
        CONSTRAINT DF_ProductBaseCommission_CreatedAt DEFAULT (SYSDATETIME()),

    CreatedByUserId UNIQUEIDENTIFIER NOT NULL;
GO


ALTER TABLE dbo.ProductBaseCommissionHistory
ADD 
    CreatedAt DATETIME2 NOT NULL 
        CONSTRAINT DF_ProductBaseCommissionHistory_CreatedAt DEFAULT (SYSDATETIME()),

    CreatedByUserId UNIQUEIDENTIFIER NOT NULL;
GO

ALTER TABLE dbo.ProductBaseCommission ADD CONSTRAINT FK_ProductBaseCommission_CreatedBy FOREIGN KEY (CreatedByUserId) REFERENCES dbo.AspNetUsers(Id);
GO

ALTER TABLE dbo.ProductBaseCommissionHistory ADD CONSTRAINT FK_ProductBaseCommission_CreatedBy FOREIGN KEY (CreatedByUserId) REFERENCES dbo.AspNetUsers(Id);
GO
