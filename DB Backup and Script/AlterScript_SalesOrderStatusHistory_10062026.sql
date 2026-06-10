CREATE TABLE dbo.SalesOrderStatusHistory
(
    SalesOrderStatusHistoryId UNIQUEIDENTIFIER NOT NULL,
    SaleId UNIQUEIDENTIFIER NOT NULL,
    OldStatus TINYINT NULL,
    NewStatus TINYINT NOT NULL,
    Remarks NVARCHAR(500) NULL,
    ChangedByUserId UNIQUEIDENTIFIER NOT NULL,
    ChangedAt DATETIME2(7) NOT NULL CONSTRAINT DF_SalesOrderStatusHistory_ChangedAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_SalesOrderStatusHistory PRIMARY KEY (SalesOrderStatusHistoryId),
    CONSTRAINT FK_SalesOrderStatusHistory_Sales FOREIGN KEY (SaleId) REFERENCES dbo.Sales(SaleId),
    CONSTRAINT FK_SalesOrderStatusHistory_User FOREIGN KEY (ChangedByUserId) REFERENCES dbo.AspNetUsers(Id)
);
GO