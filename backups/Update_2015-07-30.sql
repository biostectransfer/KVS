USE KVS;

ALTER TABLE [dbo].[Product]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[LargeCustomerRequiredField]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[Location]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[InvoiceItemAccountItem]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[Invoice]
 ADD [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[Adress]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[CostCenter]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[Customer]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[RegistrationLocation]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[Vehicle]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[Registration]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[InvoiceItem]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[OrderStatus]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[OrderType]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[RegistrationOrderType]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[Contact]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[CarOwner]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[BankAccount]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[DeregistrationOrder]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[RegistrationOrder]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[DocketList]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO