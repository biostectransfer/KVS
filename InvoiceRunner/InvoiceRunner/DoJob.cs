using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KVSCommon.Database;
using System.IO;
using System.Transactions;
using System.Configuration;
using System.Globalization;
namespace InvoiceRunner
{
    /// <summary>
    /// Rechnungslauf Klasse
    /// </summary>
    class InvoiceRunPilot
    {
        /// <summary>
        /// Liste der Rechnunslaufpositionen
        /// </summary>
        private List<InvoiceRunReport> rep;
        /// <summary>
        /// Datenbank Kontext
        /// </summary>
        private DataClasses1DataContext dbContext;
        /// <summary>
        /// Rechnungslauf ID
        /// </summary>
        private Guid invoicePilotId;
        /// <summary>
        /// Liste der Rechnungen
        /// </summary>
        private List<Invoice> invoices;
        /// <summary>
        /// Anzahl der Elemente die zu erledigen sind
        /// </summary>
        private int elementsToDoCount = 0;
        /// <summary>
        /// Instanz der Logger Klasse
        /// </summary>
        Logger log = new Logger();
        /// <summary>
        /// Aktueller Auftragsindex
        /// </summary>
        private int currOrderIndex = 1;
        /// <summary>
        /// Grosskunden Rechnungstypen
        /// </summary>
        public LargeCustomer customerInvoiceType { get; set; }
        /// <summary>
        /// Hilfsobejkt fuer die gruppierten Auftraege
        /// </summary>
        IEnumerable<IGrouping<string, VirtualOrder>> invoiceTypeGroup { set; get; }
        /// <summary>
        /// Standartkonstruktor fuer den Rechnungslauf
        /// </summary>
        /// <param name="InvoicePilotId"></param>
        public InvoiceRunPilot(Guid InvoicePilotId)
        {
            this.invoicePilotId = InvoicePilotId;
            try
            {
                log.Info("Rechnungslauf wurde gestartet");
                dbContext = new DataClasses1DataContext(invoicePilotId);
                invoices = new List<Invoice>();
                rep = dbContext.InvoiceRunReport.Where(q => q.FinishedDate == null).ToList();

            }
            catch (Exception ex)
            {
                log.Error("Fehler beim Rechnungslauf im Standartkonstruktor: " + ex.Message);
            }
        }
        /// <summary>
        /// Erstelle einen neuen Rechnungslauf
        /// </summary>
        public void RunPilot()
        {
                try
                {
                    GenerateInvoice();
                    if (rep.Count > 0)
                    {
                        sendMessage(log.returnCompanyMessagesComplete(), ConfigurationManager.AppSettings["toEmail"].ToString().Split(';').ToList(),
                            ConfigurationManager.AppSettings["FromEmail"], ConfigurationManager.AppSettings["smtpHost"].ToString());
                    }
               }
                catch (Exception ex)
                {
                    log.Error("Fehler beim Rechnungslauf in RunPilot: " + ex.Message);
                    dbContext.WriteLogItem("Fehler beim Rechnungslauf in RunPilot: " + ex.Message, LogTypes.ERROR);
                }
                log.Info("Rechnungslauf wurde beendet");
        }
      /// <summary>
      /// Schreibt den aktuellen Rechnungslaufstatus in die Datenbank
      /// </summary>
      /// <param name="currIndex">Auftragsindex</param>
      /// <param name="pil">RechnungslaufDatensatz</param>
        private void WritePilotStatus( int currIndex, InvoiceRunReport pil)
        {
            int lastIndexState = 0;
            try
            {
                var invoiceRunReport = dbContext.InvoiceRunReport.FirstOrDefault(q => q.Id == pil.Id);
                if (lastIndexState < 100)
                {
                    double myValue = elementsToDoCount;
                    lastIndexState =  Convert.ToInt32(100/myValue  * currIndex);
                   
                    if (invoiceRunReport != null)
                    {
                        invoiceRunReport.InvoiceRunProgress = lastIndexState;
                      
                    }
                }
                else
                {
                    lastIndexState = 100;
                    invoiceRunReport.InvoiceRunProgress = lastIndexState;
                    invoiceRunReport.FinishedDate = DateTime.Now;

                }
                dbContext.SubmitChanges();
            }
            catch (Exception ex)
            {
                log.Error("Fehler beim Rechnungslauf in WritePilotStatus: " + ex.Message);
                dbContext.WriteLogItem("Fehler beim Rechnungslauf in WritePilotStatus: " + ex.Message, LogTypes.ERROR);
            }
        }
        /// <summary>
        /// Gibt alle fuer den Rechnungslauf relevanten Auftraege zurueck
        /// </summary>
        /// <param name="currentReport"></param>
        /// <returns></returns>
        private IQueryable<VirtualOrder> getOrders(InvoiceRunReport currentReport)
        {
            IQueryable<VirtualOrder> v_orders = null;
            try
            {
                log.Info("Lese aufträge für den Rechnungslauf:" + currentReport.Id);
                v_orders = from cust in dbContext.Customer
                           join ord in dbContext.Order on cust.Id equals ord.CustomerId
                           join lrcust in dbContext.LargeCustomer on cust.Id equals lrcust.CustomerId
                           where (ord.Status == 600 || ord.Status == 700)
                           orderby ord.Ordernumber descending
                           select new VirtualOrder
                            {
                                
                                OrderId = ord.Id,
                                Location = ord.Location.Name,
                                OrderNumber = ord.Ordernumber,
                                ExecutionDate = ord.ExecutionDate,
                                OrderLocation = ord.LocationId,
                                OrderDate = ord.ExecutionDate,
                                CustomerId = cust.Id,
                                IvoiceTypeId = lrcust.InvoiceTypesID,
                                LocationName = ord.Location.Name,
                                LocationId = ord.LocationId,
                                ReportId = currentReport.Id
                           
                            };
               
                if (currentReport.CustomerId != null)
                {
                   v_orders= v_orders.Where(q => q.CustomerId == currentReport.CustomerId);
                }
                if (currentReport.InvoiceTypeId != null)
                {
                  v_orders=  v_orders.Where(q => q.IvoiceTypeId == currentReport.InvoiceTypeId);
                }
                log.Info("Auslesen erfolgreich:" + currentReport.Id);
            }
            catch (Exception ex)
            {
                v_orders = null;
                log.AddcompanyMessages("Fehler beim Rechnungslauf, bitte kontaktieren Sie den Administrator, dieser hat eine Einsicht in die Detaillogs");
                log.Error("Fehler beim Rechnungslauf in getOrders: " + ex.Message);
                dbContext.WriteLogItem("Fehler beim Rechnungslauf in getOrders: " + ex.Message, LogTypes.ERROR);
            }
            return v_orders;

        }
        /// <summary>
        /// Addiert Rechnungen und Auftraege zusammen damit bekannt ist, wie viele Postiionen zu erledigen sind
        /// </summary>
        /// <param name="currentReport"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        private int elementsToDo(InvoiceRunReport currentReport, IQueryable<VirtualOrder> orders)
        {
            try
            {
                var invoices = getInvoices(currentReport).ToList();
                return invoices.Count + orders.ToList().Count();
            }
            catch (Exception ex)
            {
                log.Error("Fehler beim Rechnungslauf in elementsToDo: " + ex.Message);
                dbContext.WriteLogItem("Fehler beim Rechnungslauf in elementsToDo: " + ex.Message, LogTypes.ERROR);

            }
            return 0;
        }
        /// <summary>
        /// Gibt alle Rechnungen zurueck, die zum Fertigstellen sind
        /// </summary>
        /// <param name="currentReport"></param>
        /// <returns></returns>
        private List<Invoice> getInvoices(InvoiceRunReport currentReport)
        {

            List<Invoice> v_invoices = null;
            try
            {
                log.Info("Zähle Rechnungen  für den Rechnungslauf:" + currentReport.Id);
                v_invoices = (from inv in dbContext.Invoice
                                  where (inv.canceled == null || inv.canceled == false)
                                  && inv.IsPrinted == false 
                                  && !invoices.Contains(inv)
                                  orderby inv.CreateDate descending
                                  select inv).ToList();

                if (currentReport.CustomerId != null)
                {
                    v_invoices = v_invoices.Where(q => q.CustomerId == currentReport.CustomerId).ToList();
                }
                if (currentReport.InvoiceTypeId != null)
                {
                    v_invoices = v_invoices.Where(q => q.Customer.LargeCustomer.InvoiceTypesID == currentReport.InvoiceTypeId).ToList();
                }


                log.Info("Abzählen getInvoices erfolgreich:" + currentReport.Id);
            }
            catch (Exception ex)
            {
                v_invoices = null;
                log.Error("Fehler beim Rechnungslauf in getOrders: " + ex.Message);
                dbContext.WriteLogItem("Fehler beim Rechnungslauf in getOrders: " + ex.Message, LogTypes.ERROR);
            }
            return v_invoices;
        }
        /// <summary>
        /// Rechnungen drucken
        /// </summary>
        /// <param name="rep"></param>
        private void PrintInvoices(InvoiceRunReport rep)
        {
            try
            {
                TransactionScope ts=null;
                List<_Accounts> acc = null;
                invoices.AddRange(getInvoices(rep).ToList());
                log.Info("Erstelle Rechnungen für den Rechnungslauf:" + rep.Id);
                log.AddcompanyMessages("Abgeschlossene Aufträge werden nun verrechnet, insgesamt: " + invoices.Count);
                foreach (var inv in invoices)
                {
                    WritePilotStatus(currOrderIndex, rep);
                    currOrderIndex++;
                    log.Info("Erstelle Buchungskosten für die Rechnung mit der ID:" + inv.Id);
                    try
                    {
                        if (inv.InvoiceItem.Count <= 0)
                        {
                            throw new Exception("Die Rechnung konnte nicht gebucht werden, da für diese Rechnung keine Rechnungspositionen verbucht wurden. Rechnungsid: " + inv.Id);
                        }
                        acc = Accounts.generateAccountNumber(dbContext, inv.Id).ToList();
                        using (ts = new TransactionScope())
                        {
                        
                            if (acc != null && acc.Count() == inv.InvoiceItem.Count)
                            {
                                foreach (var thisItems in acc)
                                {
                                    var myAccount = new InvoiceItemAccountItem
                                    {
                                        IIACCID = Guid.NewGuid(),
                                        InvoiceItemId = thisItems.InvoiceItemId,
                                        RevenueAccountText = thisItems.AccountNumber
                                    };
                                    log.AddcompanyMessages("Erlöskonto mit der Nummer:" + myAccount.RevenueAccountText + " wird in die Rechnung übernommen");
                                    
                                    log.Info("Erstellt Erlöskonto mit der ID:" + myAccount.IIACCID);
                                    var contains = dbContext.InvoiceItemAccountItem.FirstOrDefault(q => q.InvoiceItemId ==
                                        thisItems.InvoiceItemId && q.RevenueAccountText == thisItems.AccountNumber.Trim());
                                    if (contains != null)
                                    {
                                        log.Error("Fehler, für diese Rechnungspostion mit der id:" + thisItems.InvoiceItemId +
                                            " wurde bereits ein Erlöskonto angelegt! Die Rechnung mit der ID:" + inv.Id + " kann nicht erstellt werden.");
                                        contains.RevenueAccountText = thisItems.AccountNumber.Trim();
                                    }
                                    else
                                    {
                                        dbContext.InvoiceItemAccountItem.InsertOnSubmit(myAccount);
                                    }
                                    dbContext.SubmitChanges();
                                  

                                }
                            
                                using (MemoryStream memS = new MemoryStream())
                                {
                                    inv.LogDBContext = dbContext;
                                    inv.Print(dbContext, memS, "");
                                    dbContext.SubmitChanges();

                                }

                                Guid? locationId = null;
                                locationId = (from inv_ in dbContext.Invoice
                                              join oi in dbContext.OrderInvoice on inv_.Id equals oi.InvoiceId
                                              join myorder in dbContext.Order on oi.OrderId equals myorder.Id
                                              where inv_.Id == inv.Id
                                              select myorder).First().LocationId;
                                        //inv.OrderInvoice.First().Order.LocationId;
                                if (inv.Customer.LargeCustomer.SendInvoiceByEmail)
                                {
                                    var emails = inv.Customer.LargeCustomer.GetMailinglistAdresses(dbContext, locationId, "Rechnung");
                                    foreach (var mails in emails)
                                    {
                                        log.AddcompanyMessages("Rechnung: " + inv.InvoiceNumber.Number +
                                            " wird per Email an: " + mails + " versendet");
                                    }
                                    Invoice.SendByMail(dbContext, inv.Id, ConfigurationManager.AppSettings["smtpHost"],
                                     ConfigurationManager.AppSettings["FromEmail"], emails);
                                }
                                else
                                {
                                    log.AddcompanyMessages("Rechnung: " + inv.InvoiceNumber.Number +
                                         " wurde erstellt und ist im CASE System zu finden, da der Kunde:"+inv.Customer.CustomerNumber+" kein Mailversand als Rechnungsversand definiert hat.");

                                }
                                ts.Complete();
                                log.Info("Rechnung: " + inv.InvoiceNumber.Number + " wurde gedruckt und abgeschlossen.");
                                log.AddcompanyMessages("Rechnung: " + inv.InvoiceNumber.Number + " wurde gedruckt und abgeschlossen.");
                               
                            }
                            else
                            {
                                throw new Exception("Die Rechnung enthält keine Erlöskontent. Rechnungsid: " + inv.Id);
                            }
                          
                           
                        }
                    }
                    catch (Exception ex)
                    {
                        if(ts!=null)
                        ts.Dispose();
                        log.Error("Fehler beim Rechnungslauf in PrintInvoices:"+ex.Message);
                        log.AddcompanyMessages("Fehler beim Rechnungslauf:" + ex.Message);
                        log.AddcompanyMessages("Bitte korrigieren Sie wenn möglich den Fehler und starten einen neuen Rechnungslauf, da die Änderungen verworfen wurden.");
                   
                        dbContext.WriteLogItem("Fehler beim Rechnungslauf in PrintInvoices:"+ex.Message, LogTypes.ERROR);
                        continue;
                    }
               
                }

                var invoiceRunReport = dbContext.InvoiceRunReport.FirstOrDefault(q => q.Id == rep.Id);
                invoiceRunReport.LogDBContext = dbContext;
                invoiceRunReport.InvoiceRunProgress = 100;
                invoiceRunReport.FinishedDate = DateTime.Now;
                dbContext.SubmitChanges();
                log.AddcompanyMessages("Rechnungslauf abgeschlossen");
            }
            catch (Exception ex)
            {
                log.AddcompanyMessages("Fehler beim Rechnungslauf/Rechnungsdruck, bitte kontaktieren Sie den Administrator, dieser hat die Einsicht in die Detaillogs.");
                log.Error("Fehler beim Rechnungslauf in PrintInvoices: " + ex.Message);
                dbContext.WriteLogItem("Fehler beim Rechnungslauf in PrintInvoices: " + ex.Message, LogTypes.ERROR);
            }

        }
        /// <summary>
        /// Erstelle Rechnungen
        /// </summary>
        private void GenerateInvoice()
        {
            try
            {
                var cal = new GregorianCalendar();
                foreach (var oneReport in rep)
                {
                    var orders = getOrders(oneReport);
                    elementsToDoCount = elementsToDo(oneReport, orders);
                    
                    log.Info("Beginne das abarbeiten vom Rechnungslauf:" + oneReport.Id);
                    log.AddcompanyMessages("Rechnungslauf wurde gestartet");
                    invoices.Clear();
                    Invoice tempInvoice = null;
                    TransactionScope ts = null;
                    currOrderIndex = 1;
                    Guid? typeId = null;
                    if (oneReport.InvoiceTypeId == null)
                        typeId = Guid.Empty;
                    else
                        typeId = oneReport.InvoiceTypeId;

                      var getInvoiceTypeName = dbContext.InvoiceTypes.FirstOrDefault(q => q.ID == typeId);
                      IQueryable<IGrouping<string, VirtualOrder>> groupedOrder=null;
                      groupedOrder = orders.GroupBy(q => q.LocationId.ToString());
                     
                      foreach (var grouped in groupedOrder)
                      {
                          WritePilotStatus(currOrderIndex, oneReport);
                          if (getInvoiceTypeName != null)
                          {
                              invoiceTypeGroup = getCurrentInvoiceType(getInvoiceTypeName, grouped);
                          }
                          else
                          {
                             customerInvoiceType = dbContext.LargeCustomer.FirstOrDefault(q => q.CustomerId == grouped.First().CustomerId);
                              if (customerInvoiceType == null || customerInvoiceType.InvoiceTypesID == null)
                              {
                                  throw new Exception("Fehler beim Auftrag " + grouped.First().OrderNumber +". Es wurde beim Kunden : "+ customerInvoiceType.Customer.CustomerNumber + " kein Rechnungstyp definiert"); 
                              }
                              getInvoiceTypeName = dbContext.InvoiceTypes.FirstOrDefault(q => q.ID == customerInvoiceType.InvoiceTypesID);
                              invoiceTypeGroup = getCurrentInvoiceType(getInvoiceTypeName, grouped);
                          }
                              foreach (var orderswithLocation in invoiceTypeGroup)
                              {
                                  using (ts = new TransactionScope())
                                  {
                                  var newAdress = Invoice.GetInitialInvoiceAdress(orderswithLocation.First().CustomerId, orderswithLocation.First().LocationId, dbContext);
                                      tempInvoice = Invoice.CreateInvoice(dbContext, invoicePilotId, orderswithLocation.First().LocationName, newAdress.Id, orderswithLocation.First().CustomerId, null, getFullInvoiceName(oneReport.InvoiceTypeId == null ? customerInvoiceType.InvoiceTypesID : oneReport.InvoiceTypeId));

                                  foreach (var order in orderswithLocation)
                                  {

                                      dbContext.SubmitChanges();
                                      log.Info("Temporäre Rechnung erstellet mit der ID:" + tempInvoice.Id);
                                      log.AddcompanyMessages("Rechnung erstellt für Auftragsnummer:" + order.OrderNumber);
                                      var orderItems = dbContext.OrderItem.Where(q => q.OrderId == order.OrderId && q.Status < 900 && q.Status >= 600);
                                      foreach (var item in orderItems)
                                      {
                                          InvoiceItem newInvoiceItem = tempInvoice.AddInvoiceItem(item.ProductName, Convert.ToDecimal(item.Amount),item.Count, item.Id, item.CostCenterId, dbContext);
                                          var OrderItemToUpdate = dbContext.OrderItem.SingleOrDefault(q => q.Id == item.Id);
                                          OrderItemToUpdate.LogDBContext = dbContext;
                                          OrderItemToUpdate.Status = 900;
                                          dbContext.SubmitChanges();
                                          UpdateOrderStatus(item.OrderId);
                                          log.Info("Temporäre Rechnungsposition erstellet mit der ID:" + newInvoiceItem.Id);
                                          log.AddcompanyMessages("Rechnungsposition erstellt für Auftragsnummer:" + order.OrderNumber + " mit der Dienstleistung: " + newInvoiceItem.Name);
                                      }
                                  }

                                  ts.Complete();
                                  }
                                  var cust = dbContext.Customer.SingleOrDefault(q => q.Id == grouped.First().CustomerId);
                                  var loc = dbContext.Location.FirstOrDefault(q => q.Id == grouped.First().LocationId);
                                  log.AddcompanyMessages("Rechnung wurde ohne Fehler erstellt für Kunden:" + cust.Name + ((loc != null) ? " und Standort: " + loc.Name : ""));

                                  invoices.Add(tempInvoice);

                                  currOrderIndex++;
                             
                          }
                        }
 

                    PrintInvoices(oneReport);
                }
              
            }
            catch (Exception ex)
            {
                log.Error("Fehler beim Rechnungslauf in GenerateInvoice: " + ex.Message);
                log.AddcompanyMessages("Fehler beim Rechnungslauf, bitte kontaktieren Sie den Administrator. Dieser hat eine Einsicht in die Detaillogs");

                dbContext.WriteLogItem("Fehler beim Rechnungslauf in GenerateInvoice: " + ex.Message, LogTypes.ERROR);
            }

        }
        /// <summary>
        /// Aktualisiere Auftragsstatuse
        /// </summary>
        /// <param name="orderId">Auftragsid</param>
        protected void UpdateOrderStatus(Guid orderId)
        {
            bool shouldBeUpdated = false;
            bool hasAbgerechnetItem = false;
        
            var orderQuery = dbContext.Order.SingleOrDefault(q => q.Id == orderId);
            orderQuery.LogDBContext = dbContext;

            foreach (OrderItem item in orderQuery.OrderItem)
            {
                if (item.Status == 600)
                {
                    shouldBeUpdated = true;
                }

                if (item.Status == 900)
                {
                    hasAbgerechnetItem = true;
                }
            }

            if (shouldBeUpdated == true && hasAbgerechnetItem == true) // teil
            {
                orderQuery.Status = 700;
            }

            else if (shouldBeUpdated == false && hasAbgerechnetItem == true) //komplet abgerechnet
            {
                orderQuery.Status = 900;
            }

            dbContext.SubmitChanges();
        }
        /// <summary>
        /// Gibt den vollständigen Rechnungsnamen zurueck
        /// </summary>
        /// <param name="invTypeID"></param>
        /// <returns></returns>
        private string getFullInvoiceName(Guid? invTypeID)
        {
            if (invTypeID == null)
                return "";

            var type = dbContext.InvoiceTypes.FirstOrDefault(q => q.ID == invTypeID);
            if (type == null)
                return "";
            else
                return type.InvoiceTypeName;
        }
        /// <summary>
        /// Sendet den Report per Email an definierte Adressen
        /// </summary>
        /// <param name="logReport">Gespeicherte Logs</param>
        /// <param name="toEmailAddresses">Empfaengeradresse</param>
        /// <param name="fromEmailAddress">Absenderadressen</param>
        /// <param name="smtpServer">SMTP Server</param>
        public  void sendMessage(List<string> logReport, List<string> toEmailAddresses, string fromEmailAddress, string smtpServer)
        {

            log.Info("Beginne mit dem Email Versand");
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<p>Sehr geehrte Damen und Herren, </p>");
                sb.AppendLine("der Rechnungslauf ist abgeschlossen mit folgenden Meldungen: <br/> <br/>");

                sb.AppendLine("<table border=1>");
                sb.AppendLine("<tr>");
                sb.AppendLine("<th>Reportnummer</th>");
                sb.AppendLine("<th>Reportnachricht</th>");
                sb.AppendLine("<tr/>");
                int repNumber = 0;
                foreach (var message in logReport)
                {
                    sb.AppendLine("<tr>");
                    sb.AppendLine("<td>" + repNumber + "</td>");
                    sb.AppendLine("<td>" + message + "</td>");
                    sb.AppendLine("</tr>");
                    repNumber++;
                }
                sb.AppendLine("</table>");
                sb.AppendLine("<br/><br/>");
                sb.AppendLine("<p>Mit freundlichen Grüßen,<br/>");
                sb.AppendLine("Ihr CASE-Team</p>");
                KVSCommon.Utility.Email.SendMail(fromEmailAddress, toEmailAddresses, "Rechnungslauf Report", sb.ToString(), null, null, smtpServer, null);
                log.Info("Email wurde versendet");
            }
            catch (Exception ex)
            {
                log.Error("Fehler beim Rechnungslauf in Emailversand: " + ex.Message);
                log.AddcompanyMessages("Fehler beim Rechnungslauf, bitte kontaktieren Sie den Administrator. Dieser hat eine Einsicht in die Detaillogs");
                dbContext.WriteLogItem("Fehler beim Rechnungslauf in Emailversand: " + ex.Message, LogTypes.ERROR);
            }
 
        }
     
        /// <summary>
        /// Gibt Auftraege zurueck, die dem Rechnungstyp entsprechen
        /// </summary>
        /// <param name="type">Rechnungstyp</param>
        /// <param name="groupedOrder">Auftragsgruppe</param>
        /// <returns></returns>
        private IEnumerable<IGrouping<string, VirtualOrder>> getCurrentInvoiceType(InvoiceTypes type, IGrouping<string, VirtualOrder> groupedOrder)
        {
            IEnumerable<IGrouping<string, VirtualOrder>> _invoiceTypeGroup=null;
            if ((type != null) && type.InvoiceTypeName == "Sammelrechnung")
            {
                _invoiceTypeGroup = groupedOrder.GroupBy(q => q.LocationName.ToString());
            }
            if ((type != null) && type.InvoiceTypeName == "Einzelrechnung")
            {
                _invoiceTypeGroup = groupedOrder.GroupBy(q => q.OrderNumber.ToString());
            }
            if ((type != null) && type.InvoiceTypeName == "Wochenrechnung")
            {
                _invoiceTypeGroup = groupedOrder.GroupBy(q => Math.Floor((decimal)q.ExecutionDate.Value.DayOfYear / 7).ToString());
            }
            if ((type != null) && type.InvoiceTypeName == "Monatsrechnung")
            {
                _invoiceTypeGroup = groupedOrder.GroupBy(q => q.ExecutionDate.Value.Month.ToString());
            }
            return _invoiceTypeGroup;
        }


     
    }
}
