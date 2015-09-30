USE KVS;

ALTER TABLE [dbo].[BIC_DE]
 ADD [Id] [int] NOT NULL IDENTITY(1,1)
GO
ALTER TABLE dbo.BIC_DE ADD CONSTRAINT
	PK_BIC_DE PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE [dbo].[BIC_DE]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[User]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[Order]
 ADD [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO
ALTER TABLE [dbo].[Price]
 ADD [CreateDate] datetime2(2) NOT NULL default(GetDate()),
 [ChangeDate] datetime2(2) NOT NULL default(GetDate()),
 [DeleteDate] datetime2(2) NULL
GO

