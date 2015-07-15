using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Security;
using KVSCommon.Database;
using System.Configuration;
using Zulassungssoftware_Webservice.helper;
using System.IO;

namespace Zulassungssoftware_Webservice
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für Service1
    /// </summary>
    [WebService(Namespace = "http://microsoft.com/webservices/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Um das Aufrufen dieses Webdiensts aus einem Skript mit ASP.NET AJAX zuzulassen, heben Sie die Auskommentierung der folgenden Zeile auf. 
    // [System.Web.Script.Services.ScriptService]
    public class Zulassungswebservice : System.Web.Services.WebService
    {
        [WebMethod(Description = "Neuen Zulassungsauftrag erstellen")]
        public List<ResultMessages> VehicleRegistration(Registration newRegestration, string username, string password, string InternalId)
        {
            List<ResultMessages> results = new List<ResultMessages>();
            List<Guid> allowedCustomers = new List<Guid>();
            XMLHelper orderHelper = new XMLHelper();
            ResultMessages message = new ResultMessages();
            allowedCustomers = CheckLogin.CheckUser(username, password, InternalId);
            if (allowedCustomers.Count > 0)
            {
                using (DataClasses1DataContext dbContext = new DataClasses1DataContext(new Guid(ConfigurationManager.AppSettings["currentUser"])))
                {
                    try
                    {
                        dbContext.WriteLogItem("Zugriff von der IP Adresse: " + this.Context.Request.UserHostAddress + "Webservice VehicleRegistration()", LogTypes.INFO);
                        string file=orderHelper.XmlSerializeAndSave(newRegestration, OrderTypes.VehicleRegistration);
                       
                        if (File.Exists(file))
                        {
                            ValidateOrderType.ValidateAndSaveRegistrationValues(newRegestration, dbContext, out results);
                        }
                        else
                        {
                            throw new Exception(file);
                        }
                    }

                    catch (Exception ex)
                    {
                        message.Error = "Fehler beim verabeiten der Daten, bitte wiederholen Sie den Vorgang" + Environment.NewLine + "Fehlermeldung: " + ex.Message;
                        results.Add(message);
                    }
                }
            }

            return results;
        }
        [WebMethod(Description = "Neuen Wiederzulassungsauftrag erstellen")]
        public List<ResultMessages> VehicleReadmission(Readmission newReadmission, string username, string password, string InternalId)
        {
            List<ResultMessages> results = new List<ResultMessages>();
            List<Guid> allowedCustomers = new List<Guid>();
            XMLHelper orderHelper = new XMLHelper();
            ResultMessages message = new ResultMessages();
            allowedCustomers = CheckLogin.CheckUser(username, password, InternalId);
            if (allowedCustomers.Count > 0)
            {
                using (DataClasses1DataContext dbContext = new DataClasses1DataContext(new Guid(ConfigurationManager.AppSettings["currentUser"])))
                {
                    dbContext.WriteLogItem("Zugriff von der IP Adresse: " + this.Context.Request.UserHostAddress + "Webservice VehicleReadmission()", LogTypes.INFO);
                    try
                    {                
                        string file = orderHelper.XmlSerializeAndSave(newReadmission, OrderTypes.VehicleReadmission);
                   
                        if (File.Exists(file))
                        {
                            ValidateOrderType.ValidateAndSaveReadmissionValues(newReadmission, dbContext, out results);
                        }
                        else
                        {
                            throw new Exception(file);
                        }
                    }

                    catch (Exception ex)
                    {
                        message.Error = "Fehler beim verabeiten der Daten, bitte wiederholen Sie den Vorgang" + Environment.NewLine + "Fehlermeldung: " + ex.Message;
                        results.Add(message);
                    }
                }
            }

            return results;
        }
        [WebMethod(Description = "Neuen Abmeldeauftrag erstellen")]
        public List<ResultMessages> VehicleDerigistration(Deregistration newDereg, string username, string password, string InternalId)
        {
            List<ResultMessages> results = new List<ResultMessages>();
            ResultMessages message = new ResultMessages();
            List<Guid> allowedCustomers = new List<Guid>();
            XMLHelper orderHelper = new XMLHelper();

            allowedCustomers = CheckLogin.CheckUser(username, password, InternalId);
            if (allowedCustomers.Count > 0)
            {
                using (DataClasses1DataContext dbContext = new DataClasses1DataContext(new Guid(ConfigurationManager.AppSettings["currentUser"])))
                {
                    try
                    {
                        dbContext.WriteLogItem("Zugriff von der IP Adresse: " + this.Context.Request.UserHostAddress + "Webservice VehicleDeregestration()", LogTypes.INFO);
                        string file = orderHelper.XmlSerializeAndSave(newDereg, OrderTypes.VehicleDeregistration);
                       
                        if (File.Exists(file))
                        {
                            ValidateOrderType.ValidateAndSaveDerigistration(newDereg, dbContext, out results);
                        }
                        else
                        {
                            throw new Exception(file);
                        }
                    }

                    catch (Exception ex)
                    {
                       message.Error="Fehler beim verabeiten der Daten, bitte wiederholen Sie den Vorgang" + Environment.NewLine + "Fehlermeldung: " + ex.Message;
                       results.Add(message);
                    }
                }
            }

            return results;
        }
         [WebMethod(Description = "Neuen Umkennzeichnungsauftrag erstellen")]
        public List<ResultMessages> VehicleRemark(Remark newRemark, string username, string password, string InternalId)
        {
            List<ResultMessages> results = new List<ResultMessages>();
            List<Guid> allowedCustomers = new List<Guid>();
            XMLHelper orderHelper = new XMLHelper();
            ResultMessages message = new ResultMessages();

            allowedCustomers = CheckLogin.CheckUser(username, password, InternalId);
            if (allowedCustomers.Count > 0)
            {
                using (DataClasses1DataContext dbContext = new DataClasses1DataContext(new Guid(ConfigurationManager.AppSettings["currentUser"])))
                {
                    try
                    {
                        dbContext.WriteLogItem("Zugriff von der IP Adresse: " + this.Context.Request.UserHostAddress + "Webservice VehicleRemark()", LogTypes.INFO);
                        string file = orderHelper.XmlSerializeAndSave(newRemark, OrderTypes.VehicleRemark);

                        if (File.Exists(file))
                        {
                            ValidateOrderType.ValidateAndSaveRemark(newRemark, dbContext, out results);
                        }
                        else
                        {
                            throw new Exception(file);
                        }
                    }

                    catch (Exception ex)
                    {
                        message.Error = "Fehler beim verabeiten der Daten, bitte wiederholen Sie den Vorgang" + Environment.NewLine + "Fehlermeldung: " + ex.Message;
                        results.Add(message);
                    }
                }
            }

            return results;
        }
        [WebMethod(Description = "Gibt alle Kundendaten incl. Produkte zurück, die im System bekannt sind für die jeweilige InternalId")]
         public Kundendaten GetCustomerProductInformation(string username, string password, string InternalId)
        {
           
            Kundendaten knd = new Kundendaten();
            List<Guid> allowedCustomers = new List<Guid>();
            try
            {
                allowedCustomers = CheckLogin.CheckUser(username, password, InternalId);
                if (allowedCustomers.Count > 0)
                {
                    using (DataClasses1DataContext dbContext = new DataClasses1DataContext(new Guid(ConfigurationManager.AppSettings["currentUser"])))
                    {
                        dbContext.WriteLogItem("Zugriff von der IP Adresse: " + this.Context.Request.UserHostAddress + "Webservice GetCustomerInformation()", LogTypes.INFO);
                        ValidateOrderType.GetCutomerInformation(out knd, allowedCustomers, dbContext);
                    }
                }
                else 
                {
                    throw new Exception("Für Ihre Daten konnten keine Kunden gefunden werden!");
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Fehler beim verabeiten der Daten, bitte wiederholen Sie den Vorgang" + Environment.NewLine + "Fehlermeldung: " + ex.Message);
            }

            return knd;
        }
    }
}