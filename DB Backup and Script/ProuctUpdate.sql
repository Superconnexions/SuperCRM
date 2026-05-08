/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [ProductId]
      ,[CategoryId]
      ,[ProductCode]
      ,[ProductName]
      ,[ProductDisplayName]
      ,[ProductType]
      ,[CustomerType]
      ,[ProductDescription]
      ,[IsThirdPartyProduct]
      ,[InstallmentApplicable]
      ,[DownPaymentAmount]
      ,[CurrencyCode]
      ,[IsRequiredBankInformation]
      ,[IsProviderDeliveryProduct]
      ,[BasePriceType]
      ,[BasePrice]
      ,[IsPriceEditable]
      ,[IsPortalVisible]
      ,[IsPortalOrderEnabled]
      ,[DisplayOrder]
      ,[ProductDisplayNotes]
      ,[PaymentNotes]
      ,[Remarks]
      ,[IsActive]
      ,[CreatedAt]
      ,[UpdatedAt]
      ,[UpdatedByUserId]
      ,[SalesUnitId]
      ,[SalesUnitCode]
      ,[MonthlyInstallmentAmount]
      ,[NoOfInstallment]
  FROM [SuperCRM_Dev].[dbo].[Products]
  where IsActive = 0
  AND ProductCode IN ('CARD-MACH-WORLDPAY-PKG2','CARD-MACH-ANYOTHER-PKG1')


 UPDATE [SuperCRM_Dev].[dbo].[Products]
 SET  IsActive = 0
  where IsActive = 1
   AND ProductCode IN ('CARD-MACH-WORLDPAY-PKG2','CARD-MACH-ANYOTHER-PKG1')