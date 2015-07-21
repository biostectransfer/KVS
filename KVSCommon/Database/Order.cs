using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse für die DB Tabelle Order
    /// </summary>
    public partial class Order : ILogging
    {
        static void Main(string[] args)
        {
            if (args.Count() > 0)
                SendOrderStatusPerEmail(true); // falls parameter - täglich gesendet
            else
                SendOrderStatusPerEmail(false); // falls keine - stundlich
        }

        public DataClasses1DataContext LogDBContext
        {
            get;
            set;
        }

        public object ItemId
        {
            get
            {
                return this.Id;
            }
        }

        public EntityState EntityState
        {
            get;
            set;
        }

        /// <summary>
        /// Sendet das Auftragsstatus per Email
        /// </summary>
        public static void SendOrderStatusPerEmail(bool daily)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            var userId = Int32.Parse(ConfigurationManager.AppSettings["UserIdForEmailJob"]);
            string fromEmail = ConfigurationManager.AppSettings["FromEmail"];

            try 
            { 
                if (daily == true) // täglich gesendet
                {
                    SendOrderFinishedNotes(true, userId, fromEmail, "mail.newdirection.de");
                }

                else //stundlich gesendet
                {
                    SendOrderFinishedNotes(false, userId, fromEmail, "mail.newdirection.de");
                }
            }

            catch
            {
                throw new Exception("Fehler während der Job mit AuftragStatus per Email");
            }
        }

        /// <summary>
        /// Erstellt einen Auftrag.
        /// </summary>
        /// <param name="userId">Id des Benutzers.</param>
        /// <param name="customerId">Id des Kunden.</param>
        /// <param name="orderTypeId">Id der Auftragsart.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Den neuen Auftrag.</returns>
        public static Order CreateOrder(int userId, int customerId, int orderTypeId, int zulassungsstelleId, DataClasses1DataContext dbContext)
        {
            if (userId == 0)
            {
                throw new Exception("Die BenutzerId ist nicht gesetzt.");
            }

            Order order = new Order()
            {
                CreateDate = DateTime.Now,
                CustomerId = customerId,
                Status = (int)OrderState.Offen,
                OrderTypeId = orderTypeId,
                UserId = userId,
                Zulassungsstelle = zulassungsstelleId
            };

            dbContext.WriteLogItem("Auftrag angelegt.", LogTypes.INSERT, order.Id, "Order");
            dbContext.Order.InsertOnSubmit(order);
            return order;
        }

        /// <summary>
        /// Aktualisiert den Auftragsstatus
        /// </summary>
        /// <param name="orderIds">List mit OrderIds</param>
        /// <param name="dbContext">DB Kontext</param>
        public static void UpdateOrderStates(List<int> orderIds, DataClasses1DataContext dbContext)
        {
            IQueryable<Order> orders = dbContext.Order.Where(q => orderIds.Contains(q.Id));
            foreach (var order in orders.Where(q => q.OrderItem.Any(p => p.Status == (int)OrderItemState.Abgerechnet) && !q.OrderItem.All(r => r.Status == (int)OrderItemState.Abgerechnet || r.Status == (int)OrderItemState.Storniert)))
            {
                order.LogDBContext = dbContext;
                order.Status = (int)OrderState.Teilabgerechnet;
            }

            foreach (var order in orders.Where(q => q.OrderItem.All(p => p.Status == (int)OrderItemState.Storniert || p.Status == (int)OrderItemState.Abgerechnet)))
            {
                order.LogDBContext = dbContext;
                order.Status = (int)OrderState.Abgerechnet;
            }
        }

        /// <summary>
        /// Sendet eine Benachrichtigungsemail über den Abschluss des angegebenen Auftrag.
        /// </summary>
        /// <param name="order">Der Auftrag.</param>
        /// <param name="fromEmailAddress">Absenderemailadresse.</param>
        /// <param name="smtpServer">SMTP-Server für den Emailversand.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <remarks>Die Methode ruft dbContext.SubmitChanges() auf, um den Status der Versendung zu speichern.</remarks>
        public static void SendOrderFinishedNote(Order order, string fromEmailAddress, string smtpServer, DataClasses1DataContext dbContext)
        {
            List<string> emails = new List<string>();
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
                order.LogDBContext = dbContext;
                order.HasFinishedNoteBeenSent = true;
                dbContext.SubmitChanges();
            }
        }

        /// <summary>
        /// Sendet eine Email mit einer Auflistung der in <paramref name="orders"/> übergebenen Aufträge.
        /// </summary>
        /// <param name="orders">Aufträge, deren Daten versendet werden sollen.</param>
        /// <param name="toEmailAddresses">Liste der Empfängeremailadressen.</param>
        /// <param name="fromEmailAddress">Absenderemailadresse.</param>
        /// <param name="smtpServer">SMTP-Server für den Emailversand.</param>
        private static void SendOrderFinishedNote(IEnumerable<Order> orders, List<string> toEmailAddresses, string fromEmailAddress, string smtpServer)
        {
            StringBuilder sb = new StringBuilder();
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
          
            foreach (var order in orders.OrderBy(q => q.Ordernumber))
            {
                var vehicle = order.RegistrationOrder != null ? order.RegistrationOrder.Vehicle : order.DeregistrationOrder.Vehicle;
                var registration = order.RegistrationOrder != null ? order.RegistrationOrder.Registration : order.DeregistrationOrder.Registration;
                sb.AppendLine("<tr>");
                sb.AppendLine("<td>" + order.Ordernumber.ToString() + "</td>");
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

   
    
            Utility.Email.SendMail(fromEmailAddress, toEmailAddresses, "Benachrichtigung über erledigte Aufträge", sb.ToString(), null, null, smtpServer, null);
        }

        /// <summary>
        /// Sendet Benachrichtigungsemails für alle erledigten Aufträge ohne Benachrichtigungsemail.
        /// </summary>
        /// <param name="sendDailyMails">Gibt an, ob beim Durchlauf auch die täglichen Emails mit abgearbeitet werden sollen.</param>
        /// <param name="userId">UserId für den Datenbankzugriff.</param>
        /// <param name="fromEmailAddress">Emailadresse des Absenders.</param>
        /// <param name="smtpServer">SMTP-Server für den Emailversand.</param>
        public static void SendOrderFinishedNotes(bool sendDailyMails, int userId, string fromEmailAddress, string smtpServer)
        {
            using (DataClasses1DataContext dbContext = new DataClasses1DataContext(userId))
            {
                var customers = dbContext.LargeCustomer.Select(q => q);
                // Definition OrderFinishedNoteSendType: 0=nicht senden, 1=sofort senden, 2=stündlich senden, 3=täglich senden
                if (sendDailyMails)
                {
                    customers = customers.Where(q => q.OrderFinishedNoteSendType >= 2);
                }
                else
                {
                    customers = customers.Where(q => q.OrderFinishedNoteSendType == 2);
                }

                foreach (var customer in customers)
                {
                    var orders = customer.Customer.Order.Where(q => q.Status >= (int)OrderState.Abgeschlossen && !q.HasFinishedNoteBeenSent.GetValueOrDefault(false));
                    if (customer.SendOrderFinishedNoteToLocation.GetValueOrDefault(false))
                    {
                        var locationOrders = orders.GroupBy(q => q.Location);
                        foreach (var group in locationOrders)
                        {
                            var location = group.Key;
                            List<string> emails = new List<string>();
                            emails.AddRange(customer.GetMailinglistAdresses(dbContext, location.Id, "Auftragserledigung"));
                            if (customer.SendOrderFinishedNoteToCustomer.GetValueOrDefault(false))
                            {
                                emails.AddRange(customer.GetMailinglistAdresses(dbContext, null, "Auftragserledigung"));
                            }

                            if (emails.Count > 0)
                            {
                                SendOrderFinishedNote(group, emails, fromEmailAddress, smtpServer);
                                foreach (var order in group)
                                {
                                    order.LogDBContext = dbContext;
                                    order.HasFinishedNoteBeenSent = true;
                                }

                                dbContext.SubmitChanges();
                            }
                        }
                    }
                    else if (customer.SendOrderFinishedNoteToCustomer.GetValueOrDefault(false))
                    {
                        List<string> emails = customer.GetMailinglistAdresses(dbContext, null, "Auftragserledigung");
                        if (emails.Count > 0)
                        {
                            SendOrderFinishedNote(orders, emails, fromEmailAddress, smtpServer);
                            foreach (var order in orders)
                            {
                                order.LogDBContext = dbContext;
                                order.HasFinishedNoteBeenSent = true;
                            }

                            dbContext.SubmitChanges();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gibt die Anzahl der noch Offenen Aufträge zurück
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="OrdertypeName">z. B Zulassung</param>
        /// <returns>Int</returns>
        public static int getUnfineshedOrdersCount(DataClasses1DataContext dbContext, string OrdertypeName, int orderstate)
        {
            //ord.Status == 100 && ordtype.Name == "Zulassung" && ord.HasError.GetValueOrDefault(false) != true
            return dbContext.Order.Count(q => q.Status == orderstate && q.OrderType.Name == OrdertypeName && q.HasError.GetValueOrDefault(false) != true);
        }
        /// <summary>
        /// Fügt dem Auftrag eine neue Position hinzu.
        /// </summary>
        /// <param name="productId">Id des Produkts.</param>
        /// <param name="priceAmount">Preis für die Position.</param>
        /// <param name="count">Anzahl für die Position.</param>
        /// <param name="costCenterId">Id der Kostenstelle, falls benötigt.</param>
        /// <param name="superOrderItemId">Id der übergeordneten Auftragsposition, falls benoetigt.</param>
        /// <param name="isAuthorativeCharge">Gibt an, ob es sich um eine behoerdliche Gebühr handelt oder nicht.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Die neue Auftragsposition.</returns>
        public OrderItem AddOrderItem(int productId, decimal priceAmount, int count, CostCenter costCenter, int? superOrderItemId, bool isAuthorativeCharge, DataClasses1DataContext dbContext)
        {
            var product = dbContext.Product.Where(q => q.Id == productId).Single();
            OrderItem item = new OrderItem()
            {
                Amount = priceAmount,
                CostCenter = costCenter,
                ProductId = productId,
                Status = (int)OrderItemState.Offen,
                ProductName = product.Name,
                SuperOrderItemId = superOrderItemId,
                Count = count,
                IsAuthorativeCharge = isAuthorativeCharge,
                NeedsVAT = isAuthorativeCharge ? false : product.NeedsVAT
            };

            this.OrderItem.Add(item);
            dbContext.WriteLogItem("Auftragsposition " + product.Name + " für Auftrag " + this.Ordernumber + " angelegt.", LogTypes.INSERT, item.Id, "OrderItem");
            return item;
        }
        /// <summary>
        /// Erstellt die amtlichen Gebuehren
        /// </summary>
        /// <param name="dbContext">DB Kontext</param>
        /// <param name="authId">Amtliche Gebuehr ID</param>
        /// <param name="itemId">Auftragspositionen Id</param>
        /// <param name="amount">Betrag</param>
        /// <returns>bool</returns>
        public static bool GenerateAuthCharge(DataClasses1DataContext dbContext, int? authId, string itemId, string amount)
        {
            if (amount == string.Empty)
                amount = "0";

            if (amount != "kein Preis")
            {
                if (!EmptyStringIfNull.IsNumber(amount))
                    throw new Exception("Achtung, Sie haben keinen gültigen Preis eingegeben");

                string amoutToSave = amount;
                if (amoutToSave.Contains("."))
                    amoutToSave = amoutToSave.Replace(".", ",");


                if (!authId.HasValue)
                {
                    var myOrder = (from order in dbContext.Order
                                   join ordItem in dbContext.OrderItem on order.Id equals ordItem.OrderId
                                   where ordItem.Id == Int32.Parse(itemId)
                                   select new tempOrder
                                   {
                                       order = order,
                                       orderItem = ordItem
                                   }).First();
        
                
                    myOrder.order.AddOrderItem(myOrder.orderItem.ProductId, Convert.ToDecimal(amoutToSave), myOrder.orderItem.Count, myOrder.orderItem.CostCenter, 
                        myOrder.orderItem.Id, true, dbContext);
                }
                else
                {
                    var orderItem = dbContext.OrderItem.FirstOrDefault(q => q.Id == authId);
                    orderItem.LogDBContext = dbContext;
                 
                    orderItem.Amount = Convert.ToDecimal(amoutToSave);
                }
                return true;

            }
            else
            {

                return false;
            }


        }

        /// <summary>
        /// Entfernt die LieferscheinId und versetzt den Auftag in den Status Zulassungsstelle
        ///</summary>
        ///<param name="dbContext">DB Kontext</param>
        ///<param name="packingListId">Lieferschein ID</param>
        public static void TryToRemovePackingListIdAndSetStateToRegistration(DataClasses1DataContext dbContext, int packingListId)
        {
            var orders = dbContext.Order.Where(q => q.PackingListId == packingListId);
            foreach(var order in orders)
            {
                if (order != null)
                {
                    order.LogDBContext = dbContext;
                    int temp_packingListId = 0;

                    if (order.PackingList != null)
                    {
                        order.PackingList.OldOrderId = order.Id;
                        temp_packingListId = order.PackingList.PackingListNumber;

                        dbContext.WriteLogItem("Lieferschein: " + temp_packingListId + " zum Auftrag: " + order.Ordernumber + "  wurde gelöscht. ", LogTypes.UPDATE, 
                            order.PackingList.Id, "PackingList");

                    }
                    order.PackingList = null;
                    order.Status = (int)OrderState.Zulassungsstelle;
                    order.ReadyToSend = false;
                    order.FinishDate = null;

                    foreach (var orIt in order.OrderItem)
                    {
                        orIt.LogDBContext = dbContext;
                        orIt.Status = (int)OrderItemState.InBearbeitung;

                    }

                    dbContext.WriteLogItem("Auftrag: " + order.Ordernumber + "  wurde wieder in die Zulassungsstelle versetzt.", LogTypes.UPDATE, order.Id, "Order");
                }
                dbContext.SubmitChanges();

            }
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnCreated()
        {
            this.EntityState = Database.EntityState.New;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnLoaded()
        {
            this.EntityState = Database.EntityState.Loaded;
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnStatusChanging(int value)
        {
            if (value == (int)OrderState.Storniert && this.Status >= (int)OrderState.Teilabgerechnet)
            {
                throw new Exception("Der Auftrag kann nicht storniert werden, da er bereits (teilweise) abgerechnet ist.");
            }

            if (value == (int)OrderState.Abgeschlossen && !this.FinishDate.HasValue)
            {
                this.FinishDate = DateTime.Now;
            }

            this.WriteUpdateLogItem("Status", this.Status, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnExecutionDateChanging(DateTime? value)
        {
            this.WriteUpdateLogItem("Erledigungsdatum", this.ExecutionDate, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnErrorReasonChanging(string value)
        {
            this.WriteUpdateLogItem("Fehlerbegründung", this.ErrorReason, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnCreateDateChanging(DateTime value)
        {
            if (this.EntityState != EntityState.New)
            {
                throw new Exception("Das Auftragserstelldatum darf nicht geändert werden.");
            }
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnHasErrorChanging(bool? value)
        {
            this.WriteUpdateLogItem("Fehlermarkierung", this.HasError, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnFreeTextChanging(string value)
        {
            this.WriteUpdateLogItem("Freitext", this.FreeText, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnHasFinishedNoteBeenSentChanging(bool? value)
        {
            this.WriteUpdateLogItem("Auftragserledigungsemail gesendet", this.HasFinishedNoteBeenSent, value);
        }
    }
}
