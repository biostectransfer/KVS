
CREATE VIEW [dbo].[GetAccountNumbers]
AS
SELECT        invItem.InvoiceId, invItem.Id, account.AccountId, account.AccountNumber, pr.LocationId
FROM            dbo.Invoice AS inv INNER JOIN
                         dbo.InvoiceItem AS invItem ON inv.Id = invItem.InvoiceId INNER JOIN
                         dbo.OrderItem AS ordItem ON invItem.OrderItemId = ordItem.Id INNER JOIN
                         dbo.[Order] AS ord ON ordItem.OrderNumber = ord.OrderNumber INNER JOIN
                         dbo.Price AS pr ON ordItem.ProductId = pr.ProductId AND ord.LocationId = pr.LocationId INNER JOIN
                         dbo.PriceAccount AS priceAccount ON pr.Id = priceAccount.PriceId INNER JOIN
                         dbo.Accounts AS account ON priceAccount.AccountId = account.AccountId

GO

UPDATE [dbo].[PathPosition]
   SET [PostionName] = 'Export->DateV Export'
 WHERE [Path] = '/ImportExport/ImportExport.aspx'
GO
