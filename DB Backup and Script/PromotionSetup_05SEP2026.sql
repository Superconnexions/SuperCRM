
---------------Phase-2-----------

/* =========================================================
   SaleLines - Promotion Audit Enhancement
   ========================================================= */

ALTER TABLE dbo.SaleLines
ADD
    PromotionId UNIQUEIDENTIFIER NULL,
    PromotionItemId UNIQUEIDENTIFIER NULL,
    IsPromotionApplied BIT NOT NULL
        CONSTRAINT DF_SaleLines_IsPromotionApplied DEFAULT (0);
GO

ALTER TABLE dbo.SaleLines
ADD CONSTRAINT FK_SaleLines_PromotionSetup
FOREIGN KEY (PromotionId)
REFERENCES dbo.PromotionSetup (PromotionId);
GO


ALTER TABLE dbo.SaleLines
ADD CONSTRAINT FK_SaleLines_PromotionItem
FOREIGN KEY (PromotionItemId)
REFERENCES dbo.PromotionItem (PromotionItemId);
GO

CREATE INDEX IX_SaleLines_PromotionId
ON dbo.SaleLines (PromotionId)
WHERE PromotionId IS NOT NULL;
GO


CREATE INDEX IX_SaleLines_PromotionItemId
ON dbo.SaleLines (PromotionItemId)
WHERE PromotionItemId IS NOT NULL;
GO


CREATE INDEX IX_SaleLines_IsPromotionApplied
ON dbo.SaleLines (IsPromotionApplied);
GO