USE [SuperCRM_Dev]
GO

ALTER TABLE [dbo].[SalesOrderDrafts] DROP CONSTRAINT [FK_SalesOrderDrafts_UpdatedBy]
GO

ALTER TABLE [dbo].[SalesOrderDrafts] DROP CONSTRAINT [FK_SalesOrderDrafts_CustomerBusiness]
GO

ALTER TABLE [dbo].[SalesOrderDrafts] DROP CONSTRAINT [FK_SalesOrderDrafts_CustomerBankAccount]
GO

ALTER TABLE [dbo].[SalesOrderDrafts] DROP CONSTRAINT [FK_SalesOrderDrafts_CustomerAddress]
GO

ALTER TABLE [dbo].[SalesOrderDrafts] DROP CONSTRAINT [FK_SalesOrderDrafts_Customer]
GO

ALTER TABLE [dbo].[SalesOrderDrafts] DROP CONSTRAINT [FK_SalesOrderDrafts_CreatedBy]
GO

ALTER TABLE [dbo].[SalesOrderDrafts] DROP CONSTRAINT [DF_SalesOrderDrafts_CreatedAt]
GO

ALTER TABLE [dbo].[SalesOrderDrafts] DROP CONSTRAINT [DF_SalesOrderDrafts_DraftStatus]
GO

/****** Object:  Index [IX_SalesOrderDrafts_DraftNo]    Script Date: 5/14/2026 7:24:35 AM ******/
DROP INDEX [IX_SalesOrderDrafts_DraftNo] ON [dbo].[SalesOrderDrafts]
GO

/****** Object:  Index [IX_SalesOrderDrafts_CreatedBy_Status]    Script Date: 5/14/2026 7:24:35 AM ******/
DROP INDEX [IX_SalesOrderDrafts_CreatedBy_Status] ON [dbo].[SalesOrderDrafts]
GO

/****** Object:  Table [dbo].[SalesOrderDrafts]    Script Date: 5/14/2026 7:24:35 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SalesOrderDrafts]') AND type in (N'U'))
DROP TABLE [dbo].[SalesOrderDrafts]
GO

/****** Object:  Table [dbo].[SalesOrderDrafts]    Script Date: 5/14/2026 7:24:35 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SalesOrderDrafts](
	[SalesOrderDraftId] [uniqueidentifier] NOT NULL,
	[DraftNo] [nvarchar](50) NOT NULL,
	[CustomerId] [uniqueidentifier] NULL,
	[CustomerBusinessId] [uniqueidentifier] NULL,
	[CustomerAddressId] [uniqueidentifier] NULL,
	[CustomerBankAccountId] [uniqueidentifier] NULL,
	[DraftStatus] [tinyint] NOT NULL,
	[CreatedByUserId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedByUserId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_SalesOrderDrafts] PRIMARY KEY CLUSTERED 
(
	[SalesOrderDraftId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [SuperCRMDev_FileGroup],
 CONSTRAINT [UQ_SalesOrderDrafts_DraftNo] UNIQUE NONCLUSTERED 
(
	[DraftNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [SuperCRMDev_FileGroup]
) ON [SuperCRMDev_FileGroup]
GO

/****** Object:  Index [IX_SalesOrderDrafts_CreatedBy_Status]    Script Date: 5/14/2026 7:24:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_SalesOrderDrafts_CreatedBy_Status] ON [dbo].[SalesOrderDrafts]
(
	[CreatedByUserId] ASC,
	[DraftStatus] ASC,
	[UpdatedAt] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [SuperCRMDev_FileGroup]
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [IX_SalesOrderDrafts_DraftNo]    Script Date: 5/14/2026 7:24:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_SalesOrderDrafts_DraftNo] ON [dbo].[SalesOrderDrafts]
(
	[DraftNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [SuperCRMDev_FileGroup]
GO

ALTER TABLE [dbo].[SalesOrderDrafts] ADD  CONSTRAINT [DF_SalesOrderDrafts_DraftStatus]  DEFAULT ((1)) FOR [DraftStatus]
GO

ALTER TABLE [dbo].[SalesOrderDrafts] ADD  CONSTRAINT [DF_SalesOrderDrafts_CreatedAt]  DEFAULT (sysdatetime()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[SalesOrderDrafts]  WITH CHECK ADD  CONSTRAINT [FK_SalesOrderDrafts_CreatedBy] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO

ALTER TABLE [dbo].[SalesOrderDrafts] CHECK CONSTRAINT [FK_SalesOrderDrafts_CreatedBy]
GO

ALTER TABLE [dbo].[SalesOrderDrafts]  WITH CHECK ADD  CONSTRAINT [FK_SalesOrderDrafts_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerId])
GO

ALTER TABLE [dbo].[SalesOrderDrafts] CHECK CONSTRAINT [FK_SalesOrderDrafts_Customer]
GO

ALTER TABLE [dbo].[SalesOrderDrafts]  WITH CHECK ADD  CONSTRAINT [FK_SalesOrderDrafts_CustomerAddress] FOREIGN KEY([CustomerAddressId])
REFERENCES [dbo].[CustomerAddresses] ([CustomerAddressId])
GO

ALTER TABLE [dbo].[SalesOrderDrafts] CHECK CONSTRAINT [FK_SalesOrderDrafts_CustomerAddress]
GO

ALTER TABLE [dbo].[SalesOrderDrafts]  WITH CHECK ADD  CONSTRAINT [FK_SalesOrderDrafts_CustomerBankAccount] FOREIGN KEY([CustomerBankAccountId])
REFERENCES [dbo].[CustomerBankAccounts] ([CustomerBankAccountId])
GO

ALTER TABLE [dbo].[SalesOrderDrafts] CHECK CONSTRAINT [FK_SalesOrderDrafts_CustomerBankAccount]
GO

ALTER TABLE [dbo].[SalesOrderDrafts]  WITH CHECK ADD  CONSTRAINT [FK_SalesOrderDrafts_CustomerBusiness] FOREIGN KEY([CustomerBusinessId])
REFERENCES [dbo].[CustomerBusinesses] ([CustomerBusinessId])
GO

ALTER TABLE [dbo].[SalesOrderDrafts] CHECK CONSTRAINT [FK_SalesOrderDrafts_CustomerBusiness]
GO

ALTER TABLE [dbo].[SalesOrderDrafts]  WITH CHECK ADD  CONSTRAINT [FK_SalesOrderDrafts_UpdatedBy] FOREIGN KEY([UpdatedByUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO

ALTER TABLE [dbo].[SalesOrderDrafts] CHECK CONSTRAINT [FK_SalesOrderDrafts_UpdatedBy]
GO




ALTER TABLE [dbo].[SalesOrderDraftLines] DROP CONSTRAINT [FK_SalesOrderDraftLines_ProviderProduct]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] DROP CONSTRAINT [FK_SalesOrderDraftLines_Provider]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] DROP CONSTRAINT [FK_SalesOrderDraftLines_ProductVariant]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] DROP CONSTRAINT [FK_SalesOrderDraftLines_Product]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] DROP CONSTRAINT [FK_SalesOrderDraftLines_Draft]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] DROP CONSTRAINT [DF_SalesOrderDraftLines_IsInstallmentSelected]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] DROP CONSTRAINT [DF_SalesOrderDraftLines_CreatedAt]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] DROP CONSTRAINT [DF_SalesOrderDraftLines_InstallmentApplicable]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] DROP CONSTRAINT [DF_SalesOrderDraftLines_IsPriceEditable]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] DROP CONSTRAINT [DF_SalesOrderDraftLines_Quantity]
GO

/****** Object:  Index [IX_SalesOrderDraftLines_ProviderId]    Script Date: 5/14/2026 7:25:30 AM ******/
DROP INDEX [IX_SalesOrderDraftLines_ProviderId] ON [dbo].[SalesOrderDraftLines]
GO

/****** Object:  Index [IX_SalesOrderDraftLines_ProductId]    Script Date: 5/14/2026 7:25:30 AM ******/
DROP INDEX [IX_SalesOrderDraftLines_ProductId] ON [dbo].[SalesOrderDraftLines]
GO

/****** Object:  Index [IX_SalesOrderDraftLines_DraftId]    Script Date: 5/14/2026 7:25:30 AM ******/
DROP INDEX [IX_SalesOrderDraftLines_DraftId] ON [dbo].[SalesOrderDraftLines]
GO

/****** Object:  Table [dbo].[SalesOrderDraftLines]    Script Date: 5/14/2026 7:25:30 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SalesOrderDraftLines]') AND type in (N'U'))
DROP TABLE [dbo].[SalesOrderDraftLines]
GO

/****** Object:  Table [dbo].[SalesOrderDraftLines]    Script Date: 5/14/2026 7:25:30 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SalesOrderDraftLines](
	[SalesOrderDraftLineId] [uniqueidentifier] NOT NULL,
	[SalesOrderDraftId] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[ProductCode] [nvarchar](50) NULL,
	[ProductName] [nvarchar](200) NULL,
	[ProductVariantId] [uniqueidentifier] NULL,
	[VariantCode] [nvarchar](50) NULL,
	[VariantName] [nvarchar](200) NULL,
	[ProviderProductId] [uniqueidentifier] NULL,
	[ProviderId] [uniqueidentifier] NULL,
	[ProviderName] [nvarchar](200) NULL,
	[Quantity] [int] NOT NULL,
	[BasePriceType] [tinyint] NOT NULL,
	[BasePrice] [decimal](18, 2) NOT NULL,
	[SalePrice] [decimal](18, 2) NOT NULL,
	[LineTotalAmount] [decimal](18, 2) NOT NULL,
	[CurrencyCode] [nvarchar](10) NULL,
	[IsPriceEditable] [bit] NOT NULL,
	[InstallmentApplicable] [bit] NOT NULL,
	[DownPaymentAmount] [decimal](18, 2) NULL,
	[NoOfInstallment] [int] NULL,
	[MonthlyInstallmentAmount] [decimal](18, 2) NULL,
	[FirstInstallmentDate] [date] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[IsInstallmentSelected] [bit] NOT NULL,
 CONSTRAINT [PK_SalesOrderDraftLines] PRIMARY KEY CLUSTERED 
(
	[SalesOrderDraftLineId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [SuperCRMDev_FileGroup]
) ON [SuperCRMDev_FileGroup]
GO

/****** Object:  Index [IX_SalesOrderDraftLines_DraftId]    Script Date: 5/14/2026 7:25:30 AM ******/
CREATE NONCLUSTERED INDEX [IX_SalesOrderDraftLines_DraftId] ON [dbo].[SalesOrderDraftLines]
(
	[SalesOrderDraftId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [SuperCRMDev_FileGroup]
GO

/****** Object:  Index [IX_SalesOrderDraftLines_ProductId]    Script Date: 5/14/2026 7:25:30 AM ******/
CREATE NONCLUSTERED INDEX [IX_SalesOrderDraftLines_ProductId] ON [dbo].[SalesOrderDraftLines]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [SuperCRMDev_FileGroup]
GO

/****** Object:  Index [IX_SalesOrderDraftLines_ProviderId]    Script Date: 5/14/2026 7:25:30 AM ******/
CREATE NONCLUSTERED INDEX [IX_SalesOrderDraftLines_ProviderId] ON [dbo].[SalesOrderDraftLines]
(
	[ProviderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [SuperCRMDev_FileGroup]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] ADD  CONSTRAINT [DF_SalesOrderDraftLines_Quantity]  DEFAULT ((1)) FOR [Quantity]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] ADD  CONSTRAINT [DF_SalesOrderDraftLines_IsPriceEditable]  DEFAULT ((0)) FOR [IsPriceEditable]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] ADD  CONSTRAINT [DF_SalesOrderDraftLines_InstallmentApplicable]  DEFAULT ((0)) FOR [InstallmentApplicable]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] ADD  CONSTRAINT [DF_SalesOrderDraftLines_CreatedAt]  DEFAULT (sysdatetime()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] ADD  CONSTRAINT [DF_SalesOrderDraftLines_IsInstallmentSelected]  DEFAULT ((0)) FOR [IsInstallmentSelected]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines]  WITH CHECK ADD  CONSTRAINT [FK_SalesOrderDraftLines_Draft] FOREIGN KEY([SalesOrderDraftId])
REFERENCES [dbo].[SalesOrderDrafts] ([SalesOrderDraftId])
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] CHECK CONSTRAINT [FK_SalesOrderDraftLines_Draft]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines]  WITH CHECK ADD  CONSTRAINT [FK_SalesOrderDraftLines_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] CHECK CONSTRAINT [FK_SalesOrderDraftLines_Product]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines]  WITH CHECK ADD  CONSTRAINT [FK_SalesOrderDraftLines_ProductVariant] FOREIGN KEY([ProductVariantId])
REFERENCES [dbo].[ProductVariants] ([ProductVariantId])
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] CHECK CONSTRAINT [FK_SalesOrderDraftLines_ProductVariant]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines]  WITH CHECK ADD  CONSTRAINT [FK_SalesOrderDraftLines_Provider] FOREIGN KEY([ProviderId])
REFERENCES [dbo].[Providers] ([ProviderId])
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] CHECK CONSTRAINT [FK_SalesOrderDraftLines_Provider]
GO

ALTER TABLE [dbo].[SalesOrderDraftLines]  WITH CHECK ADD  CONSTRAINT [FK_SalesOrderDraftLines_ProviderProduct] FOREIGN KEY([ProviderProductId])
REFERENCES [dbo].[ProviderProducts] ([ProviderProductId])
GO

ALTER TABLE [dbo].[SalesOrderDraftLines] CHECK CONSTRAINT [FK_SalesOrderDraftLines_ProviderProduct]
GO
s