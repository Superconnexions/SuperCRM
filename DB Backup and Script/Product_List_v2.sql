SELECT 
   -- P.ProductId,
    ISNULL(PC.CategoryName, '') AS CategoryName,
    P.ProductCode,
    P.ProductName,
    P.ProductDisplayName,
	ISNULL(P.ProductDisplayNotes, '') AS ProductDisplayNotes,
	ISNULL(P.PaymentNotes, '') AS PaymentNotes,
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

    -- Base Price Type
    CASE P.BasePriceType
        WHEN 1 THEN 'SimplePrice'
        WHEN 2 THEN 'OpenPrice'
        WHEN 3 THEN 'VariantPrice'
        ELSE ''
    END AS BasePriceType,
	ISNULL(SU.UnitName, '') AS SalesUnitName,
	ISNULL(P.CurrencyCode, '') AS CurrencyCode,
    P.BasePrice as [Unit Price],
	CASE WHEN P.InstallmentApplicable = 1 THEN 'Yes' ELSE 'No' END AS IsInstallmentApplicable,
    ISNULL(P.DownPaymentAmount, 0) AS DownPaymentAmount,
	0 as [No of Installment],
	0 as 'Installment Amount',

    -- BIT ? Yes/No
    CASE WHEN P.IsThirdPartyProduct = 1 THEN 'Yes' ELSE 'No' END AS IsProviderProduct,
    
    CASE WHEN P.IsRequiredBankInformation = 1 THEN 'Yes' ELSE 'No' END AS IsRequiredBankInformation,
    CASE WHEN P.IsProviderDeliveryProduct = 1 THEN 'Yes' ELSE 'No' END AS IsProviderDeliveryProduct,
    CASE WHEN P.IsPriceEditable = 1 THEN 'Yes' ELSE 'No' END AS IsPriceEditableDuringOrder,
    --CASE WHEN P.IsPortalVisible = 1 THEN 'Yes' ELSE 'No' END AS IsPortalVisible,
    --CASE WHEN P.IsPortalOrderEnabled = 1 THEN 'Yes' ELSE 'No' END AS IsPortalOrderEnabled,
    CASE WHEN P.IsActive = 1 THEN 'Yes' ELSE 'No' END AS IsActive,

	
    
    ISNULL(P.DisplayOrder, 0) AS DisplayOrder,
    
   
    ISNULL(P.Remarks, '') AS Remarks
	--,

 --   P.CreatedAt,
 --   P.UpdatedAt

FROM Products P
LEFT JOIN ProductCategories PC 
    ON P.CategoryId = PC.CategoryId
LEFT JOIN SalesUnits SU 
    ON P.SalesUnitId = SU.SalesUnitId

	order by P.CustomerType, CategoryName, ISNULL(P.DisplayOrder, 0), P.ProductName