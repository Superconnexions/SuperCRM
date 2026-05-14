SELECT 
   -- P.ProductId,
    ISNULL(PC.CategoryName, '') AS CategoryName,
    P.ProductCode,
    P.ProductName,
    P.ProductDisplayName,
	SU.UnitName as [Unit],
	ISNULL(Pro.ProviderName, '') AS ProviderName,
	ISNULL(P.ProductDisplayNotes, '') AS ProductDisplayNotes,
	
	ISNULL(P.PaymentNotes, '') AS PaymentNotes,
	ISNULL(provar.VariantName, '') AS VariantName,
	
    -- Product Type
    CASE P.ProductType
        WHEN 1 THEN 'SimpleProduct'
        WHEN 2 THEN 'VariantProduct'
        ELSE 'Unknown'
    END AS ProductType,

    -- Customer Type
    CASE P.CustomerType
        WHEN 1 THEN 'Business'
        WHEN 2 THEN 'Residential'
        WHEN 3 THEN 'Both'
        ELSE ''
    END AS CustomerType,

  

    ISNULL(P.ProductDescription, '') AS ProductDescription,

   
	ISNULL(SU.UnitName, '') AS SalesUnitName,
	ISNULL(P.CurrencyCode, '') AS CurrencyCode,
    P.BasePrice as [Unit Price as Sales Unit],
	 -- Base Price Type
    CASE P.BasePriceType
        WHEN 1 THEN 'SimplePrice'
        WHEN 2 THEN 'OpenPrice'
        WHEN 3 THEN 'VariantPrice'
        ELSE ''
    END AS BasePriceType,
	 ISNULL(provar.BasePrice,0) as 'Variant Price',
	CASE WHEN P.InstallmentApplicable = 1 THEN 'Yes' ELSE 'No' END AS [Is Installment Applicable],
    ISNULL(P.DownPaymentAmount, 0) AS DownPaymentAmount,
	[NoOfInstallment] as [No of Installment],
	 ISNULL(MonthlyInstallmentAmount,0) as 'Monthly Installment Amount',

    -- BIT → Yes/No
    CASE WHEN P.IsThirdPartyProduct = 1 THEN 'Yes' ELSE 'No' END AS IsProviderProduct,
    
    CASE WHEN P.IsRequiredBankInformation = 1 THEN 'Yes' ELSE 'No' END AS IsRequiredBankInformation,
    CASE WHEN P.IsProviderDeliveryProduct = 1 THEN 'Yes' ELSE 'No' END AS IsProviderDeliveryProduct,
    CASE WHEN P.IsPriceEditable = 1 THEN 'Yes' ELSE 'No' END AS IsPriceEditableDuringOrder,
    --CASE WHEN P.IsPortalVisible = 1 THEN 'Yes' ELSE 'No' END AS IsPortalVisible,
    --CASE WHEN P.IsPortalOrderEnabled = 1 THEN 'Yes' ELSE 'No' END AS IsPortalOrderEnabled,
    CASE WHEN P.IsActive = 1 THEN 'Yes' ELSE 'No' END AS IsActive,

	
    
    ISNULL(P.DisplayOrder, 0) AS DisplayOrder,
    
   ISNULL(PC.CategoryImageUrl, '') AS CategoryImageUrl,
   
    ISNULL(P.Remarks, '') AS Remarks
	--,

 --   P.CreatedAt,
 --   P.UpdatedAt

FROM Products P
LEFT JOIN ProductCategories PC 
    ON P.CategoryId = PC.CategoryId
LEFT JOIN SalesUnits SU 
    ON P.SalesUnitId = SU.SalesUnitId

LEFT JOIN [dbo].[ProviderProducts] pp
on P.ProductId = pp.ProductId

LEFT JOIN  [dbo].[Providers] pro
on pro.ProviderId = pp.ProviderId


LEFT JOIN  [dbo].[ProductVariants] provar
on provar.ProductId = P.ProductId


	where 
	 P.IsActive = 1
	--AND P.IsThirdPartyProduct = 1
	--AND P.ProductCode IN ('CARD-MACH-WORLDPAY-PKG2','CARD-MACH-ANYOTHER-PKG1')
	--AND P.IsActive = 0
	

	--UPDATE Products
	--SET IsActive = 0
	--WHERE ProductCode IN ('CARD-MACH-WORLDPAY-PKG2','CARD-MACH-ANYOTHER-PKG1')

	order by P.CustomerType, CategoryName, ISNULL(P.DisplayOrder, 0), P.ProductName