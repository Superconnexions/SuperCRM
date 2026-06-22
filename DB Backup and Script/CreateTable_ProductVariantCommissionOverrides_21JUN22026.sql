CREATE TABLE dbo.ProductVariantCommissionOverrides
(
    ProductVariantCommissionOverrideId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_ProductVariantCommissionOverrides PRIMARY KEY,

    ProductId UNIQUEIDENTIFIER NOT NULL,
    ProductCode NVARCHAR(50) NOT NULL,

    ProductVariantId UNIQUEIDENTIFIER NULL,
    VariantCode NVARCHAR(100) NOT NULL,

    ExtraCommissionAmount DECIMAL(18, 2) NOT NULL,

    EffectiveFrom DATETIME2 NULL,
    EffectiveTo DATETIME2 NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_ProductVariantCommissionOverrides_IsActive DEFAULT (1),

    Note NVARCHAR(500) NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_ProductVariantCommissionOverrides_CreatedAt DEFAULT (SYSUTCDATETIME()),

    CreatedByUserId UNIQUEIDENTIFIER NULL,

    UpdatedAt DATETIME2 NULL,
    UpdatedByUserId UNIQUEIDENTIFIER NULL
);
GO

CREATE INDEX IX_ProductVariantCommissionOverrides_ProductVariant
ON dbo.ProductVariantCommissionOverrides
(
    ProductCode,
    VariantCode,
    IsActive,
    EffectiveFrom,
    EffectiveTo
);
GO



INSERT INTO dbo.ProductVariantCommissionOverrides
(
    ProductVariantCommissionOverrideId,
    ProductId,
    ProductCode,
    ProductVariantId,
    VariantCode,
    ExtraCommissionAmount,
    EffectiveFrom,
    EffectiveTo,
    IsActive,
    Note
)
SELECT
    NEWID(),
    p.ProductId,
    p.ProductCode,
    pv.ProductVariantId,
    pv.VariantCode,
    200.00,
    GETDATE(),
    NULL,
    1,
    'Extra commission for ROLL 57X40(BOX100)'
FROM dbo.Products p
INNER JOIN dbo.ProductVariants pv
    ON pv.ProductId = p.ProductId
WHERE p.ProductCode = 'ROLLS'
  AND pv.VariantCode = '57X40(BOX100)';