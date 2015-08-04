using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using KVSCommon.Enums;
using KVSCommon.Entities;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse für die DB Tabelle Order
    /// </summary>
    public partial class Order : ILogging, IHasId<int>, IRemovable, ISystemFields
    {
        static void Main(string[] args)
        {
            if (args.Count() > 0)
                SendOrderStatusPerEmail(true); // falls parameter - täglich gesendet
            else
                SendOrderStatusPerEmail(false); // falls keine - stundlich
        }

        public KVSEntities LogDBContext
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
            KVSEntities dbContext = new KVSEntities();
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
            using (KVSEntities dbContext = new KVSEntities(userId))
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
                    var orders = customer.Customer.Order.Where(q => q.Status >= (int)OrderStatusTypes.Closed && 
                        !q.HasFinishedNoteBeenSent.GetValueOrDefault(false));
                    if (customer.SendOrderFinishedNoteToLocation.GetValueOrDefault(false))
                    {
                        var locationOrders = orders.GroupBy(q => q.Location);
                        foreach (var group in locationOrders)
                        {
                            var location = group.Key;
                            List<string> emails = new List<string>();
                            emails.AddRange(customer.GetMailinglistAdresses(dbContext, location.Id, MailingListTypes.OrderComplete));
                            if (customer.SendOrderFinishedNoteToCustomer.GetValueOrDefault(false))
                            {
                                emails.AddRange(customer.GetMailinglistAdresses(dbContext, null, MailingListTypes.OrderComplete));
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
                        List<string> emails = customer.GetMailinglistAdresses(dbContext, null, MailingListTypes.OrderComplete);
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
            if (value == (int)OrderStatusTypes.Cancelled && this.Status >= (int)OrderStatusTypes.PartiallyPayed)
            {
                throw new Exception("Der Auftrag kann nicht storniert werden, da er bereits (teilweise) abgerechnet ist.");
            }

            if (value == (int)OrderStatusTypes.Closed && !this.FinishDate.HasValue)
            {
                this.FinishDate = DateTime.Now;
            }

            //TODO this.WriteUpdateLogItem("Status", this.Status, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnExecutionDateChanging(DateTime? value)
        {
            //TODO this.WriteUpdateLogItem("Erledigungsdatum", this.ExecutionDate, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnErrorReasonChanging(string value)
        {
            //TODO this.WriteUpdateLogItem("Fehlerbegründung", this.ErrorReason, value);
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

        //public OrderStatusTypes OrderStatusType
        //{
        //    get
        //    {
        //        return (OrderStatusTypes)this.Status;
        //    }
        //}
    }
}
