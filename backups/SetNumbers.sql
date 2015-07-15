ALTER TABLE [dbo].[DocketList]
DROP COLUMN [DocketListNumber]
GO
ALTER TABLE [dbo].[DocketList]
ADD [DocketListNumber] int NOT NULL IDENTITY(30000, 1)
GO
ALTER TABLE [dbo].[PackingList]
DROP COLUMN [PackingListNumber]
GO
ALTER TABLE [dbo].[PackingList]
ADD [PackingListNumber] int NOT NULL IDENTITY(20000, 1)
GO
ALTER TABLE [dbo].[InvoiceNumber]
DROP COLUMN [Number]
GO
ALTER TABLE [dbo].[InvoiceNumber]
ADD [Number] int NOT NULL IDENTITY(10000, 1)
GO