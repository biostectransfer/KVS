using KVSCommon.Database;
using KVSCommon.Entities;
using KVSCommon.Enums;
using KVSCommon.Managers;
using KVSDataAccess.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSDataAccess.Managers
{
    public partial class OrderManager : EntityManager<Order, int>, IOrderManager
    {
        public OrderManager(IKVSEntities context) : base(context) { }

        /// <summary>
        /// Fügt dem Auftrag eine neue Position hinzu.
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="productId">Id des Produkts.</param>
        /// <param name="priceAmount">Preis für die Position.</param>
        /// <param name="count">Anzahl für die Position.</param>
        /// <param name="costCenterId">Id der Kostenstelle, falls benötigt.</param>
        /// <param name="superOrderItemId">Id der übergeordneten Auftragsposition, falls benoetigt.</param>
        /// <param name="isAuthorativeCharge">Gibt an, ob es sich um eine behoerdliche Gebühr handelt oder nicht.</param>
        /// <returns>Die neue Auftragsposition.</returns>
        public OrderItem AddOrderItem(Order order, int productId, decimal priceAmount, int count, CostCenter costCenter, int? superOrderItemId, bool isAuthorativeCharge)
        {
            var product = DataContext.GetSet<Product>().Where(q => q.Id == productId).Single();

            var item = new OrderItem()
            {
                Amount = priceAmount,
                CostCenter = costCenter,
                ProductId = productId,
                Status = (int)OrderItemStatusTypes.Open,
                ProductName = product.Name,
                SuperOrderItemId = superOrderItemId,
                Count = count,
                IsAuthorativeCharge = isAuthorativeCharge,
                NeedsVAT = isAuthorativeCharge ? false : product.NeedsVAT
            };

            order.OrderItem.Add(item);
            SaveChanges();
            DataContext.WriteLogItem("Auftragsposition " + product.Name + " für Auftrag " + order.Id + " angelegt.", LogTypes.INSERT, item.Id, "OrderItem");

            return item;
        }

        /// <summary>
        ///  Update order item amount
        /// </summary>
        /// <param name="orderItemId"></param>
        /// <param name="amount"></param>
        public void UpdateOrderItemAmount(int orderItemId, decimal amount)
        {
            var orderItem = DataContext.GetSet<OrderItem>().FirstOrDefault(o => o.Id == orderItemId);
            orderItem.Amount = amount;
            SaveChanges();
        }

        /// <summary>
        ///  Update order item
        /// </summary>
        /// <param name="orderItemId"></param>
        /// <param name="amount"></param>
        /// <param name="comment"></param>
        public void UpdateOrderItem(int orderItemId, decimal amount, string comment)
        {
            var orderItem = DataContext.GetSet<OrderItem>().FirstOrDefault(o => o.Id == orderItemId);
            orderItem.Amount = amount;
            orderItem.Comment = comment;
            SaveChanges();
        }

        /// <summary>
        /// Erstellt die amtlichen Gebuehren
        /// </summary>
        /// <param name="authId">Amtliche Gebuehr ID</param>
        /// <param name="itemId">Auftragspositionen Id</param>
        /// <param name="amount">Betrag</param>
        /// <returns>bool</returns>
        public bool GenerateAuthCharge(int? authId, int itemId, string amount)
        {
            if (amount == string.Empty)
                amount = "0";

            if (amount != "kein Preis")
            {
                decimal temp;
                if (!decimal.TryParse(amount.Replace('.', ','), out temp))
                    throw new Exception("Achtung, Sie haben keinen gültigen Preis eingegeben");

                string amoutToSave = amount;
                if (amoutToSave.Contains("."))
                    amoutToSave = amoutToSave.Replace(".", ",");


                if (!authId.HasValue)
                {
                    var orderItem = DataContext.GetSet<OrderItem>().FirstOrDefault(o => o.Id == itemId);

                    AddOrderItem(orderItem.Order, orderItem.ProductId, Convert.ToDecimal(amoutToSave), orderItem.Count, orderItem.CostCenter, orderItem.Id, true);
                }
                else
                {
                    var orderItem = DataContext.GetSet<OrderItem>().FirstOrDefault(q => q.Id == authId);
                    orderItem.Amount = Convert.ToDecimal(amoutToSave);
                    SaveChanges();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get order items
        /// </summary>
        /// <param name="orderNumber">Order number</param>
        /// <returns></returns>
        public IQueryable<OrderItem> GetOrderItems(int orderNumber)
        {
            return DataContext.GetSet<OrderItem>().Where(o => o.OrderNumber == orderNumber);
        }

        /// <summary>
        /// Get order items
        /// </summary>
        /// <returns></returns>
        public IQueryable<OrderItem> GetOrderItems()
        {
            return DataContext.GetSet<OrderItem>();
        }

        /// <summary>
        /// Loescht eine Auftragsposition und ggf. die Amtlichen Gebuehren dazu
        /// </summary>
        /// <param name="orderItemId">AuftragspositionID</param>
        public void RemoveOrderItem(int orderItemId)
        {
            var orderItemToDelete = DataContext.GetSet<OrderItem>().FirstOrDefault(q => q.Id == orderItemId);
            if (orderItemToDelete != null)
            {
                if (orderItemToDelete.Status > (int)OrderItemStatusTypes.Open)
                    throw new Exception("Der Auftragsstatus ist nicht mehr Offen, löschen nicht möglich");

                if (orderItemToDelete.Order.DocketList != null)
                    throw new Exception("Laufzettel wurde bereits erstellt, löschen nicht möglich");

                if (orderItemToDelete.Order.PackingList != null)
                    throw new Exception("Lieferschein wurde bereits erstellt, löschen nicht möglich");

                var itemsAnzahl = DataContext.GetSet<OrderItem>().Count(q => q.Id != orderItemId && q.SuperOrderItemId != orderItemId && q.OrderNumber == orderItemToDelete.OrderNumber);
                if (itemsAnzahl == 0)
                    throw new Exception("Mind. eine Position muss pro Auftrag verfügbar sein");

                var hasChildItems = DataContext.GetSet<OrderItem>().FirstOrDefault(q => q.SuperOrderItemId == orderItemToDelete.Id);
                DataContext.DeleteObject(hasChildItems);
                                
                if (orderItemToDelete.SuperOrderItemId.HasValue == true)
                {
                    RemoveOrderItem(orderItemToDelete.SuperOrderItemId.Value);
                }

                DataContext.DeleteObject(orderItemToDelete);
                DataContext.WriteLogItem("Auftragsposition " + orderItemToDelete.ProductName + " mit der Auftragsnummer " + orderItemToDelete.Order.Id + " wurde gelöscht.",
                    LogTypes.DELETE, orderItemToDelete.Id, "OrderItem");
            }
        }

        /// <summary>
        /// Sendet eine Benachrichtigungsemail über den Abschluss des angegebenen Auftrag.
        /// </summary>
        /// <param name="order">Der Auftrag.</param>
        /// <param name="fromEmailAddress">Absenderemailadresse.</param>
        /// <param name="smtpServer">SMTP-Server für den Emailversand.</param>
        /// <remarks>Die Methode ruft dbContext.SubmitChanges() auf, um den Status der Versendung zu speichern.</remarks>
        public void SendOrderFinishedNote(Order order, string fromEmailAddress, string smtpServer)
        {
            var emails = new List<string>();
            var customer = order.Customer.LargeCustomer;
            if (customer == null)
            {
                return;
            }

            if (customer.SendOrderFinishedNoteToLocation.GetValueOrDefault(false))
            {
                emails.AddRange(order.Location.Mailinglist.Where(q => q.MailinglistType.Name == "Auftragserledigung").Select(q => q.Email).ToList());
            }

            if (customer.SendOrderFinishedNoteToCustomer.GetValueOrDefault(false))
            {
                emails.AddRange(customer.Mailinglist.Where(q => q.MailinglistType.Name == "Auftragserledigung").Select(q => q.Email).ToList());
            }

            if (emails.Count > 0)
            {
                SendOrderFinishedNote(new List<Order>() { order }.AsEnumerable(), emails, fromEmailAddress, smtpServer);
                order.HasFinishedNoteBeenSent = true;
                SaveChanges();
            }
        }

        /// <summary>
        /// Sendet eine Email mit einer Auflistung der in <paramref name="orders"/> übergebenen Aufträge.
        /// </summary>
        /// <param name="orders">Aufträge, deren Daten versendet werden sollen.</param>
        /// <param name="toEmailAddresses">Liste der Empfängeremailadressen.</param>
        /// <param name="fromEmailAddress">Absenderemailadresse.</param>
        /// <param name="smtpServer">SMTP-Server für den Emailversand.</param>
        private void SendOrderFinishedNote(IEnumerable<Order> orders, List<string> toEmailAddresses, string fromEmailAddress, string smtpServer)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<p>Sehr geehrte Damen und Herren, </p>");
            if (orders.Count() > 1)
            {
                sb.AppendLine("<p>im Anschluss finden Sie die Liste der kürzlich erledigten Zulassungs- und Abmeldeaufträge.</p>");
            }
            else
            {
                sb.AppendLine("<p>nachfolgender Auftrag wurde kürzlich erledigt.</p>");
            }

            sb.AppendLine("<table border=1>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th>Auftragsnummer</th>");
            sb.AppendLine("<th>FIN</th>");
            sb.AppendLine("<th>Kennzeichen</th>");
            sb.AppendLine("<th>Halter</th>");
            sb.AppendLine("<th>Erledigungsdatum</th>");
            sb.AppendLine("<tr/>");

            foreach (var order in orders.OrderBy(q => q.Id))
            {
                var vehicle = order.RegistrationOrder != null ? order.RegistrationOrder.Vehicle : order.DeregistrationOrder.Vehicle;
                var registration = order.RegistrationOrder != null ? order.RegistrationOrder.Registration : order.DeregistrationOrder.Registration;
                sb.AppendLine("<tr>");
                sb.AppendLine("<td>" + order.Id.ToString() + "</td>");
                sb.AppendLine("<td>" + vehicle.VIN + "</td>");
                sb.AppendLine("<td>" + registration.Licencenumber + "</td>");
                sb.AppendLine("<td>" + (registration.CarOwner != null ? registration.CarOwner.FullName : string.Empty) + "</td>");
                sb.AppendLine("<td>" + (order.FinishDate.HasValue ? order.FinishDate.Value.ToString("dd.MM.yyyy HH:mm") : string.Empty) + "</td>");
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");
            sb.AppendLine("<br/><br/>");
            sb.AppendLine("<p>Mit freundlichen Grüßen,<br/>");
            sb.AppendLine("Ihr CASE-Team</p>");

            KVSCommon.Utility.Email.SendMail(fromEmailAddress, toEmailAddresses, "Benachrichtigung über erledigte Aufträge", sb.ToString(), null, null, smtpServer, null);
        }

        /// <summary>
        /// Entfernt die LieferscheinId und versetzt den Auftag in den Status Zulassungsstelle
        ///</summary>
        ///<param name="packingListNumber">Lieferschein ID</param>
        public void TryToRemovePackingListIdAndSetStateToRegistration(int packingListNumber)
        {
            var orders = DataContext.GetSet<Order>().Where(q => q.PackingListNumber == packingListNumber);
            foreach (var order in orders)
            {
                if (order != null)
                {
                    int temp_packingListId = 0;

                    if (order.PackingList != null)
                    {
                        order.PackingList.OldOrderNumber = order.Id;
                        temp_packingListId = order.PackingList.Id;

                        DataContext.WriteLogItem("Lieferschein: " + temp_packingListId + " zum Auftrag: " + order.Id + "  wurde gelöscht. ", LogTypes.UPDATE,
                            order.PackingList.Id, "PackingList");

                    }

                    order.PackingList = null;
                    order.Status = (int)OrderStatusTypes.AdmissionPoint;
                    order.ReadyToSend = false;
                    order.FinishDate = null;

                    foreach (var orIt in order.OrderItem)
                    {
                        orIt.Status = (int)OrderItemStatusTypes.InProgress;
                    }

                    DataContext.WriteLogItem("Auftrag: " + order.Id + "  wurde wieder in die Zulassungsstelle versetzt.",
                        LogTypes.UPDATE, order.Id, "Order");
                }

                SaveChanges();
            }
        }
    }
}
