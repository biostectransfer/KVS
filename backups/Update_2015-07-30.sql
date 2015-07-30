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

