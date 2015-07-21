delete from [dbo].[Accounts]
delete from [dbo].[DeregistrationOrder]
delete from [dbo].[RegistrationOrder]
--delete from [dbo].[RegistrationLocation]

UPDATE [dbo].[Vehicle]
 SET [CurrentRegistrationId] = NULL

delete from [dbo].[Registration]
delete from [dbo].[Vehicle]
delete from [dbo].[CarOwner]
delete from [dbo].[BankAccount]

delete from [dbo].[OrderInvoice]
delete from [dbo].[InvoiceItem]
delete from [dbo].[OrderItem]

UPDATE [dbo].[PackingList]
 SET [OldOrderId] = NULL

delete from [dbo].[Order]
delete from [dbo].[PackingList]

delete from [dbo].[DocketList]
delete from [dbo].[Price]
delete from [dbo].[LargeCustomerRequiredField]
delete from [dbo].[CostCenter]

UPDATE [dbo].[LargeCustomer]
 SET [MainLocationId] = NULL

delete from [dbo].[CustomerProduct]
delete from [dbo].[InvoiceNumber]
delete from [dbo].[Location]
delete from [dbo].[Mailinglist]
delete from [dbo].[LargeCustomer]

delete from [dbo].[Invoice]
delete from [dbo].[Customer]

delete from [dbo].[Adress]

UPDATE [dbo].[User]
 SET [ContactId] = NULL
delete from [dbo].[Contact]

delete from [dbo].[Document]
delete from [dbo].[InvoiceAccountBackup]
delete from [dbo].[Systemlog]