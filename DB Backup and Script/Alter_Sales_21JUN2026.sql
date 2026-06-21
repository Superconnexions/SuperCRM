ALTER TABLE dbo.SaleLines
ADD
    SpecialNotes NVARCHAR(250) NULL,
    Remarks NVARCHAR(250) NULL;

ALTER TABLE dbo.SalesOrderDraftLines
ADD SpecialNotes NVARCHAR(250) NULL