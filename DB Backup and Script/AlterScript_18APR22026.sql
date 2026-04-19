CREATE TABLE dbo.SalesUnits
(
    SalesUnitId       INT IDENTITY(1,1) NOT NULL,
    UnitCode          NVARCHAR(50) NOT NULL,
    UnitName          NVARCHAR(100) NOT NULL,
    IsActive          BIT NOT NULL CONSTRAINT DF_SalesUnits_IsActive DEFAULT (1),
    CreatedAt         DATETIME2 NOT NULL CONSTRAINT DF_SalesUnits_CreatedAt DEFAULT (SYSDATETIME()),
    UpdatedAt         DATETIME2 NULL,
    UpdatedByUserId   UNIQUEIDENTIFIER NULL,
    CONSTRAINT PK_SalesUnits PRIMARY KEY (SalesUnitId)
);
GO

ALTER TABLE dbo.SalesUnits
ADD CONSTRAINT UQ_SalesUnits_UnitCode UNIQUE (UnitCode);
GO

ALTER TABLE dbo.SalesUnits
ADD CONSTRAINT FK_SalesUnits_UpdatedBy
FOREIGN KEY (UpdatedByUserId) REFERENCES dbo.AspNetUsers(Id);
GO

INSERT INTO dbo.SalesUnits
(
    UnitCode,
    UnitName,
    IsActive,
    CreatedAt,
    UpdatedAt,
    UpdatedByUserId
)
VALUES
(N'EACH',       N'Each',       1, SYSDATETIME(), NULL, NULL),
(N'PER-MONTH',  N'Per Month',  1, SYSDATETIME(), NULL, NULL),
(N'PER-UNIT',   N'Per Unit',   1, SYSDATETIME(), NULL, NULL);
GO

ALTER TABLE dbo.Products
ADD SalesUnitId   INT NOT NULL CONSTRAINT DF_Products_SalesUnitId DEFAULT (1),
    SalesUnitCode NVARCHAR(50) NOT NULL CONSTRAINT DF_Products_SalesUnitCode DEFAULT (N'EACH');
GO

ALTER TABLE dbo.Products
ADD CONSTRAINT FK_Products_SalesUnit
FOREIGN KEY (SalesUnitId) REFERENCES dbo.SalesUnits(SalesUnitId);
GO

CREATE INDEX IX_Products_SalesUnitId
ON dbo.Products(SalesUnitId);
GO

ALTER TABLE dbo.SaleLines
ADD SalesUnitId      INT NOT NULL CONSTRAINT DF_SaleLines_SalesUnitId DEFAULT (1),
    SalesUnitCode    NVARCHAR(50) NOT NULL CONSTRAINT DF_SaleLines_SalesUnitCode DEFAULT (N'EACH'),
    LineTotalAmount  DECIMAL(18,2) NOT NULL CONSTRAINT DF_SaleLines_LineTotalAmount DEFAULT (0);
GO

ALTER TABLE dbo.SaleLines
ADD CONSTRAINT FK_SaleLines_SalesUnit
FOREIGN KEY (SalesUnitId) REFERENCES dbo.SalesUnits(SalesUnitId);
GO

CREATE INDEX IX_SaleLines_SalesUnitId
ON dbo.SaleLines(SalesUnitId);
GO