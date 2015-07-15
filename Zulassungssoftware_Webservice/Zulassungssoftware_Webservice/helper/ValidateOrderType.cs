using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KVSCommon.Database;
using System.Configuration;
namespace Zulassungssoftware_Webservice.helper
{

    public static class ValidateOrderType 
    {     
        static  Guid currentUserId = new Guid(ConfigurationManager.AppSettings["currentUser"]);

        public static void ValidateAndSaveReadmissionValues(Readmission registrationObject, DataClasses1DataContext dbContext, out List<ResultMessages> messages)
        {
            ResultMessages result = new ResultMessages();
            messages = new List<ResultMessages>();

            foreach (ReadmissionWiederzulassung wiederzulassung in registrationObject.Wiederzulassung)
            {
                result = new ResultMessages();
                if (getOpenedRegistrationOrder(wiederzulassung.Vehicle.VIN).Count == 0)
                {
                    if (ValidateGuid(wiederzulassung.Header))
                    {
                        string valGuid = ValidateGuid(wiederzulassung.Header, dbContext);
                        if (valGuid == string.Empty)
                        {
                            Vehicle car = IsVehicleInSystem(wiederzulassung.Vehicle, dbContext);
                          
                                try
                                {
                                    CreateReadmission(wiederzulassung, car, dbContext);
                                    result.Sucessed = "Sucessed imported VIN: " + wiederzulassung.Vehicle.VIN;
                                    messages.Add(result);
                                }
                                catch (Exception ex)
                                {
                                    result.Error = ex.Message;
                                    result.Readmission = wiederzulassung;
                                    messages.Add(result);
                                }
                        }
                        else
                        {
                            result.Error = valGuid;
                            result.Readmission = wiederzulassung;
                            messages.Add(result);
                        }

                    }
                    else
                    {
                        result.Error = "Eins oder mehrere Felder im Header sind keine gültigen Guids";
                        result.Readmission = wiederzulassung;
                        messages.Add(result);
                    }
                }
                else
                {
                    result.Error = "Für dieses Fahrzeug gibt es noch offene Aufträge";
                    result.Readmission = wiederzulassung;
                    messages.Add(result);
                }
            }
        }

        public static void ValidateAndSaveRegistrationValues(Registration registrationObject, DataClasses1DataContext dbContext, out List<ResultMessages> messages)
        {     
            ResultMessages result = new ResultMessages();
            messages = new List<ResultMessages>();
            
            foreach (RegistrationNeuzulassung neuzulassung in registrationObject.Neuzulassung)
            {
                result = new ResultMessages();
                
                if (getOpenedRegistrationOrder(neuzulassung.Vehicle.VIN).Count == 0)
                {
                    if (ValidateGuid(neuzulassung.Header))
                    {
                        string valGuid = ValidateGuid(neuzulassung.Header, dbContext);
                        if (valGuid == string.Empty)
                        {
                            if (IsVehicleInSystem(neuzulassung.Vehicle, dbContext) == null)
                            {
                                try
                                {
                                    CreateRegistration(neuzulassung, dbContext);
                                    result.Sucessed = "Sucessed imported VIN: " + neuzulassung.Vehicle.VIN;
                                    messages.Add(result);
                                }
                                catch (Exception ex)
                                {
                                    result.Error = ex.Message;
                                    result.Registration = neuzulassung;
                                    messages.Add(result);
                                }

                            }
                            else
                            {
                                result.Error = "Dieses Fahrzeug ist bereits im System vorhanden, meinten Sie vielleicht eine Wiederzulassung? Benutzen Sie bitte dafür die entsprechende Methode!";
                                result.Registration = neuzulassung;
                                messages.Add(result);
                            }

                        }
                        else
                        {
                            result.Error = valGuid;
                            result.Registration = neuzulassung;
                            messages.Add(result);
                        }

                    }
                    else
                    {
                        result.Error = "Eins oder mehrere Felder im Header sind keine gültigen Guids";
                        result.Registration = neuzulassung;
                        messages.Add(result);
                    }
                }
                else
                {
                    result.Error = "Für dieses Fahrzeug gibt es noch offene Aufträge";
                    result.Registration = neuzulassung;
                    messages.Add(result);
                }             
            }     
        }

        public static void ValidateAndSaveDerigistration(Deregistration deregistrationObject, DataClasses1DataContext dbContext, out List<ResultMessages> messages)
        {
            ResultMessages result = new ResultMessages();
            messages = new List<ResultMessages>();
           
            foreach (DeregistrationAbmeldung deregistration in deregistrationObject.Abmeldung)
            {             
                result = new ResultMessages();

                if (getOpenedDeregistrationOrder(deregistration.Vehicle.VIN).Count == 0)
                {
                    if (ValidateGuid(deregistration.Header))
                    {
                        string valGuid = ValidateGuid(deregistration.Header, dbContext);
                        if (valGuid == string.Empty)
                        {
                            try
                            {
                                CreateDeregOrder(deregistration, dbContext);
                                result.Sucessed = "Sucessed imported VIN: " + deregistration.Vehicle.VIN;
                                messages.Add(result);
                            }
                            catch (Exception ex)
                            {
                                result.Error = ex.Message;
                                result.Deregistration = deregistration;
                                messages.Add(result);
                            }

                        }
                        else
                        {
                            result.Error = valGuid;
                            result.Deregistration = deregistration;
                            messages.Add(result);
                        }
                    }
                    else
                    {
                        result.Error = "Eins oder mehrere Felder im Header sind keine gültigen Guids";
                        result.Deregistration = deregistration;
                        messages.Add(result);
                    }
                }
                else
                {
                    result.Error = "Für dieses Fahrzeug gibt es noch offene Aufträge";
                    result.Deregistration = deregistration;
                    messages.Add(result);
                }
            }
        }

        public static void ValidateAndSaveRemark(Remark remarkObject, DataClasses1DataContext dbContext, out List<ResultMessages> messages)
        {
            ResultMessages result = new ResultMessages();
            messages = new List<ResultMessages>();

            foreach (RemarkUmkennzeichnung  remark in remarkObject.Umkennzeichnung)
            {
                result = new ResultMessages();

                if (getOpenedDeregistrationOrder(remark.Vehicle.VIN).Count == 0  && getOpenedRegistrationOrder(remark.Vehicle.VIN).Count==0)
                {
                    if (ValidateGuid(remark.Header))
                    {
                        string valGuid = ValidateGuid(remark.Header, dbContext);
                        if (valGuid == string.Empty)
                        {
                            try
                            {
                                CreateRemarkOrder(remark, dbContext);
                                result.Sucessed = "Sucessed imported VIN: " + remark.Vehicle.VIN;
                                messages.Add(result);
                            }
                            catch (Exception ex)
                            {
                                result.Error = ex.Message;
                                result.Remark = remark;
                                messages.Add(result);
                            }

                        }
                        else
                        {
                            result.Error = valGuid;
                            result.Remark = remark;
                            messages.Add(result);
                        }
                    }
                    else
                    {
                        result.Error = "Eins oder mehrere Felder im Header sind keine gültigen Guids";
                        result.Remark = remark;
                        messages.Add(result);
                    }
                }
                else
                {
                    result.Error = "Für dieses Fahrzeug gibt es noch offene Aufträge";
                    result.Remark = remark;
                    messages.Add(result);
                }
            }
        }

        private static void CreateRemarkOrder(RemarkUmkennzeichnung umkennzeichnung, DataClasses1DataContext dbContext)
        {
            if (umkennzeichnung.Header.ProductCostCenterList.Length > 0)
            {
                var newVehicle = IsVehicleInSystem(umkennzeichnung.Vehicle, dbContext);
                Price price = null;
                string myIban = "";
                if (String.IsNullOrEmpty(umkennzeichnung.Bank.Iban))
                    myIban = generateIban(umkennzeichnung.Bank.AccountNumber, umkennzeichnung.Bank.BankCode);
                else
                    myIban = umkennzeichnung.Bank.Iban;

                //wird geprüft ob die Farbe nur Digit ist
                int myColor = 0;
                bool colorIsDigit = false;
                if (!String.IsNullOrEmpty(umkennzeichnung.Vehicle.ColorCode))
                {
                    foreach (char a in umkennzeichnung.Vehicle.ColorCode)
                    {
                        if (Char.IsDigit(a))
                            colorIsDigit = true;
                        else
                            colorIsDigit = false;
                    }
                }
                if (colorIsDigit)
                    myColor = Convert.ToInt32(umkennzeichnung.Vehicle.ColorCode);


                if (newVehicle == null)
                {
                    newVehicle = Vehicle.CreateVehicle(umkennzeichnung.Vehicle.VIN, umkennzeichnung.Vehicle.HSN, umkennzeichnung.Vehicle.TSN, umkennzeichnung.Vehicle.VehicleVariant, umkennzeichnung.Vehicle.RegistrationDate, myColor, dbContext);

                }
                var newAdress = Adress.CreateAdress(umkennzeichnung.CarOwnerAdress.Street, umkennzeichnung.CarOwnerAdress.Streetnumber, umkennzeichnung.CarOwnerAdress.Zipcode, umkennzeichnung.CarOwnerAdress.City,
                    umkennzeichnung.CarOwnerAdress.Country, dbContext);
                var newContact = Contact.CreateContact(umkennzeichnung.CarOwnerContact.Phone, umkennzeichnung.CarOwnerContact.Fax, umkennzeichnung.CarOwnerContact.MobilePhone, umkennzeichnung.CarOwnerContact.Email, dbContext);
                var newBankAccount = BankAccount.CreateBankAccount(dbContext, umkennzeichnung.Bank.CarOwnerBankName, umkennzeichnung.Bank.AccountNumber, umkennzeichnung.Bank.BankCode, myIban, umkennzeichnung.Bank.Bic);
                var newCarOwner = CarOwner.CreateCarOwner(umkennzeichnung.CarOwnerPersonal.Name, umkennzeichnung.CarOwnerPersonal.Firstname, newBankAccount.Id, newContact.Id, newAdress.Id, dbContext);

                var newRegistration = KVSCommon.Database.Registration.CreateRegistration(newCarOwner.Id, newVehicle.Id, umkennzeichnung.Vehicle.LicenceNumber, umkennzeichnung.Vehicle.eVBNumber, umkennzeichnung.Vehicle.GeneralInspectionDate,
                    DateTime.Now, umkennzeichnung.Vehicle.RegistrationDocumentNumber, umkennzeichnung.Vehicle.EmissionCode, dbContext);

                price = findPrice(umkennzeichnung.Header);

                var newRegistrationOrder = RegistrationOrder.CreateRegistrationOrder(currentUserId,
                     new Guid(umkennzeichnung.Header.CustomerId), umkennzeichnung.Vehicle.LicenceNumber, umkennzeichnung.Vehicle.PreviousLicenceNumber, umkennzeichnung.Vehicle.eVBNumber,
                     newVehicle.Id, newRegistration.Id, new Guid(RegTypes.Umkennzeichnung),
                     new Guid(umkennzeichnung.Header.LocationId), new Guid(umkennzeichnung.Header.ZulassungsstelleId), dbContext);

                newRegistrationOrder.Order.FreeText = umkennzeichnung.FreetextField;

                //add only the first product
                var newOrderItem1 = newRegistrationOrder.Order.AddOrderItem(new Guid(umkennzeichnung.Header.ProductCostCenterList[0].ProductId), price.Amount, 1, new Guid(umkennzeichnung.Header.ProductCostCenterList[0].CostCenterId), null, false, dbContext);
                if (price.AuthorativeCharge.HasValue)
                {
                    var newOrderItem2 = newRegistrationOrder.Order.AddOrderItem(new Guid(umkennzeichnung.Header.ProductCostCenterList[0].ProductId), price.AuthorativeCharge.Value, 1, new Guid(umkennzeichnung.Header.ProductCostCenterList[0].CostCenterId), newOrderItem1.Id, true, dbContext);
                }
                dbContext.SubmitChanges();

                //add all other products
                AddAnotherProducts(newRegistrationOrder, new Guid(umkennzeichnung.Header.LocationId), umkennzeichnung.Header, currentUserId);

                newVehicle.CurrentRegistrationId = newRegistration.Id;
                dbContext.SubmitChanges();
            }

            else
            {
                throw new Exception("Keine Dienstleistung / Kostenstelle mitgegeben!");
            }           
        }
    
        private static string generateIban(string account, string bankCode)
        {
            string myIban = "";
            if (account != string.Empty && bankCode != string.Empty
               && EmptyStringIfNull.IsNumber(account) && EmptyStringIfNull.IsNumber(bankCode))
            {
                myIban = "DE" + (98 - ((62 * ((1 + long.Parse(bankCode) % 97)) +
                    27 * (long.Parse(account) % 97)) % 97)).ToString("D2");
                myIban += long.Parse(bankCode).ToString("00000000").Substring(0, 4);
                myIban += long.Parse(bankCode).ToString("00000000").Substring(4, 4);
                myIban += long.Parse(account).ToString("0000000000").Substring(0, 4);
                myIban += long.Parse(account).ToString("0000000000").Substring(4, 4);
                myIban += long.Parse(account).ToString("0000000000").Substring(8, 2);
            }
            return myIban;
        
        }

        private static void CreateDeregOrder(DeregistrationAbmeldung deregistration, DataClasses1DataContext dbContext)
        {
            if (deregistration.Header.ProductCostCenterList.Length > 0)
            {
                string myIban = "";
                if (String.IsNullOrEmpty(deregistration.Bank.Iban))
                    myIban = generateIban(deregistration.Bank.AccountNumber, deregistration.Bank.BankCode);
                else
                    myIban = deregistration.Bank.Iban;

                //wird geprüft ob die Farbe nur Digit ist
                int myColor = 0;
                bool colorIsDigit = false;
                if (!String.IsNullOrEmpty(deregistration.Vehicle.ColorCode))
                {
                    foreach (char a in deregistration.Vehicle.ColorCode)
                    {
                        if (Char.IsDigit(a))
                            colorIsDigit = true;
                        else
                            colorIsDigit = false;
                    }
                }
                if (colorIsDigit)
                    myColor = Convert.ToInt32(deregistration.Vehicle.ColorCode);

                Vehicle car = IsVehicleInSystem(deregistration.Vehicle, dbContext);
                DeregistrationOrder newDeregOrder = null;
                OrderItem newOrderItem1 = null;
                OrderItem newOrderItem2 = null;
                Price price = null;
                Adress newAdress = null;
                Contact newContact = null;
                BankAccount newBankAccount = null;
                CarOwner newCarOwner = null;
                KVSCommon.Database.Registration newRegistration = null;
                if (car != null && car.Registration1 != null)
                {
                    newRegistration = car.Registration1;
                }
                else
                {
                    car = Vehicle.CreateVehicle(deregistration.Vehicle.VIN, deregistration.Vehicle.HSN, deregistration.Vehicle.TSN, deregistration.Vehicle.VehicleVariant, deregistration.Vehicle.RegistrationDate, myColor, dbContext);
                    newAdress = Adress.CreateAdress(deregistration.CarOwnerAdress.Street, deregistration.CarOwnerAdress.Streetnumber, deregistration.CarOwnerAdress.Zipcode, deregistration.CarOwnerAdress.City, deregistration.CarOwnerAdress.Country, dbContext);
                    newContact = Contact.CreateContact(deregistration.CarOwnerContact.Phone, deregistration.CarOwnerContact.Fax, deregistration.CarOwnerContact.MobilePhone, deregistration.CarOwnerContact.Email, dbContext);
                    newBankAccount = BankAccount.CreateBankAccount(dbContext, deregistration.Bank.CarOwnerBankName, deregistration.Bank.AccountNumber, deregistration.Bank.BankCode, myIban, deregistration.Bank.Bic);
                    newCarOwner = CarOwner.CreateCarOwner(deregistration.CarOwnerPersonal.Name, deregistration.CarOwnerPersonal.Firstname, newBankAccount.Id, newContact.Id, newAdress.Id, dbContext);
                    newRegistration = KVSCommon.Database.Registration.CreateRegistration(newCarOwner.Id, car.Id, deregistration.Vehicle.LicenceNumber, deregistration.Vehicle.eVBNumber,
                        deregistration.Vehicle.GeneralInspectionDate, DateTime.Now, deregistration.Vehicle.RegistrationDocumentNumber, deregistration.Vehicle.EmissionCode, dbContext);
                }

                price = findPrice(deregistration.Header);
                //neues DeregistrationOrder erstellen
                newDeregOrder = DeregistrationOrder.CreateDeregistrationOrder(currentUserId, new Guid(deregistration.Header.CustomerId), car.Id, newRegistration.Id, new Guid(deregistration.Header.LocationId), new Guid(deregistration.Header.ZulassungsstelleId), dbContext);
                //adding new Deregestrationorder Items
                //adding the first product and costcenter
                newOrderItem1 = newDeregOrder.Order.AddOrderItem(new Guid(deregistration.Header.ProductCostCenterList[0].ProductId), price.Amount, 1, new Guid(deregistration.Header.ProductCostCenterList[0].CostCenterId), null, false, dbContext);
                if (price.AuthorativeCharge.HasValue)
                {
                    newOrderItem2 = newDeregOrder.Order.AddOrderItem(new Guid(deregistration.Header.ProductCostCenterList[0].ProductId), price.AuthorativeCharge.Value, 1, new Guid(deregistration.Header.ProductCostCenterList[0].CostCenterId), newOrderItem1.Id, true, dbContext);
                }

                dbContext.SubmitChanges();

                //add all other products
                AddAnotherProducts(newDeregOrder, new Guid(deregistration.Header.LocationId), deregistration.Header, currentUserId);

                car.CurrentRegistrationId = newRegistration.Id;
                dbContext.SubmitChanges();        
            }

            else
            {
                throw new Exception("Dienstleistung / Kostenstelle ist nicht miteingegeben!");
            }
        }

        private static void CreateRegistration(RegistrationNeuzulassung neuzulassung, DataClasses1DataContext dbContext)
        {
            if (neuzulassung.Header.ProductCostCenterList.Length > 0)
            {
                string myIban = "";
                if (String.IsNullOrEmpty(neuzulassung.Bank.Iban))
                    myIban = generateIban(neuzulassung.Bank.AccountNumber, neuzulassung.Bank.BankCode);
                else
                    myIban = neuzulassung.Bank.Iban;

                //wird geprüft ob die Farbe nur Digit ist
                int myColor = 0;
                bool colorIsDigit = false;
                if (!String.IsNullOrEmpty(neuzulassung.Vehicle.ColorCode))
                {
                    foreach (char a in neuzulassung.Vehicle.ColorCode)
                    {
                        if (Char.IsDigit(a))
                            colorIsDigit = true;
                        else
                            colorIsDigit = false;
                    }
                }
                if (colorIsDigit)
                    myColor = Convert.ToInt32(neuzulassung.Vehicle.ColorCode);

                var newVehicle = IsVehicleInSystem(neuzulassung.Vehicle, dbContext);
                if (newVehicle == null)
                {
                    newVehicle = Vehicle.CreateVehicle(neuzulassung.Vehicle.VIN, neuzulassung.Vehicle.HSN, neuzulassung.Vehicle.TSN, neuzulassung.Vehicle.VehicleVariant, neuzulassung.Vehicle.RegistrationDate,myColor, dbContext);
                }

                var newAdress = Adress.CreateAdress(neuzulassung.CarOwnerAdress.Street, neuzulassung.CarOwnerAdress.Streetnumber, neuzulassung.CarOwnerAdress.Zipcode, neuzulassung.CarOwnerAdress.City, neuzulassung.CarOwnerAdress.Country, dbContext);
                var newContact = Contact.CreateContact(neuzulassung.CarOwnerContact.Phone, neuzulassung.CarOwnerContact.Fax, neuzulassung.CarOwnerContact.MobilePhone, neuzulassung.CarOwnerContact.Email, dbContext);
                var newBankAccount = BankAccount.CreateBankAccount(dbContext, neuzulassung.Bank.CarOwnerBankName, neuzulassung.Bank.AccountNumber, neuzulassung.Bank.BankCode, myIban, neuzulassung.Bank.Bic);
                var newCarOwner = CarOwner.CreateCarOwner(neuzulassung.CarOwnerPersonal.Name, neuzulassung.CarOwnerPersonal.Firstname, newBankAccount.Id, newContact.Id, newAdress.Id, dbContext);
                var newRegistration = KVSCommon.Database.Registration.CreateRegistration(newCarOwner.Id, newVehicle.Id, neuzulassung.Vehicle.LicenceNumber, neuzulassung.Vehicle.eVBNumber, neuzulassung.Vehicle.GeneralInspectionDate,
                    DateTime.Now, neuzulassung.Vehicle.RegistrationDocumentNumber, neuzulassung.Vehicle.EmissionCode, dbContext);

                Price price = findPrice(neuzulassung.Header);
                var newRegistrationOrder = RegistrationOrder.CreateRegistrationOrder(currentUserId,
                   new Guid(neuzulassung.Header.CustomerId), neuzulassung.Vehicle.LicenceNumber, "", neuzulassung.Vehicle.eVBNumber, newVehicle.Id, newRegistration.Id, new Guid(RegTypes.Neuzulassung),
                   new Guid(neuzulassung.Header.LocationId), new Guid (neuzulassung.Header.ZulassungsstelleId), dbContext);
                newRegistrationOrder.Order.FreeText = neuzulassung.FreetextField;

                // add first product and costcenter
                var newOrderItem1 = newRegistrationOrder.Order.AddOrderItem(new Guid(neuzulassung.Header.ProductCostCenterList[0].ProductId), price.Amount, 1, new Guid(neuzulassung.Header.ProductCostCenterList[0].CostCenterId), null, false, dbContext);

                if (price.AuthorativeCharge.HasValue)
                {
                    var newOrderItem2 = newRegistrationOrder.Order.AddOrderItem(new Guid(neuzulassung.Header.ProductCostCenterList[0].ProductId), price.AuthorativeCharge.Value, 1, new Guid(neuzulassung.Header.ProductCostCenterList[0].CostCenterId), newOrderItem1.Id, true, dbContext);
                }

                dbContext.SubmitChanges();

                //add all other products
                AddAnotherProducts(newRegistrationOrder, new Guid(neuzulassung.Header.LocationId), neuzulassung.Header, currentUserId);

                newVehicle.CurrentRegistrationId = newRegistration.Id;
                dbContext.SubmitChanges();
            }
            else
            {
                throw new Exception("Keine Dienstleistung / Kostenstelle miteingegeben!");
            }
        }

        private static void CreateReadmission(ReadmissionWiederzulassung wiederzulassung, Vehicle car, DataClasses1DataContext dbContext)
        {
            if (wiederzulassung.Header.ProductCostCenterList.Length > 0)
            {
                string myIban = "";
                if (String.IsNullOrEmpty(wiederzulassung.Bank.Iban))
                    myIban = generateIban(wiederzulassung.Bank.AccountNumber, wiederzulassung.Bank.BankCode);
                else
                    myIban = wiederzulassung.Bank.Iban;

                //wird geprüft ob die Farbe nur Digit ist
                int myColor = 0;
                bool colorIsDigit = false;
                if (!String.IsNullOrEmpty(wiederzulassung.Vehicle.ColorCode))
                {
                    foreach (char a in wiederzulassung.Vehicle.ColorCode)
                    {
                        if (Char.IsDigit(a))
                            colorIsDigit = true;
                        else
                            colorIsDigit = false;
                    }
                }
                if (colorIsDigit)
                    myColor = Convert.ToInt32(wiederzulassung.Vehicle.ColorCode);

                if (car == null)
                {
                    car = Vehicle.CreateVehicle(wiederzulassung.Vehicle.VIN, wiederzulassung.Vehicle.HSN, wiederzulassung.Vehicle.TSN, wiederzulassung.Vehicle.VehicleVariant, wiederzulassung.Vehicle.RegistrationDate, myColor, dbContext);
                }

                var newAdress = Adress.CreateAdress(wiederzulassung.CarOwnerAdress.Street, wiederzulassung.CarOwnerAdress.Streetnumber, wiederzulassung.CarOwnerAdress.Zipcode, wiederzulassung.CarOwnerAdress.City, wiederzulassung.CarOwnerAdress.Country, dbContext);
                var newContact = Contact.CreateContact(wiederzulassung.CarOwnerContact.Phone, wiederzulassung.CarOwnerContact.Fax, wiederzulassung.CarOwnerContact.MobilePhone, wiederzulassung.CarOwnerContact.Email, dbContext);
                var newBankAccount = BankAccount.CreateBankAccount(dbContext, wiederzulassung.Bank.CarOwnerBankName, wiederzulassung.Bank.AccountNumber, wiederzulassung.Bank.BankCode, myIban, wiederzulassung.Bank.Bic);
                var newCarOwner = CarOwner.CreateCarOwner(wiederzulassung.CarOwnerPersonal.Name, wiederzulassung.CarOwnerPersonal.Firstname, newBankAccount.Id, newContact.Id, newAdress.Id, dbContext);
                var newRegistration = KVSCommon.Database.Registration.CreateRegistration(newCarOwner.Id, car.Id,
                    wiederzulassung.Vehicle.LicenceNumber, wiederzulassung.Vehicle.eVBNumber,
                    wiederzulassung.Vehicle.GeneralInspectionDate, null, wiederzulassung.Vehicle.RegistrationDocumentNumber, wiederzulassung.Vehicle.EmissionCode, dbContext);

                Price price = findPrice(wiederzulassung.Header);

                var newRegistrationOrder = RegistrationOrder.CreateRegistrationOrder(currentUserId,
                   new Guid(wiederzulassung.Header.CustomerId), wiederzulassung.Vehicle.LicenceNumber, wiederzulassung.Vehicle.PreviousLicenceNumber, wiederzulassung.Vehicle.eVBNumber,
                   car.Id, newRegistration.Id, new Guid(RegTypes.Wiederzulassung),
                   new Guid(wiederzulassung.Header.LocationId), new Guid (wiederzulassung.Header.ZulassungsstelleId), dbContext);
                newRegistrationOrder.Order.FreeText = wiederzulassung.FreetextField;

                // add first product and costcenter
                var newOrderItem1 = newRegistrationOrder.Order.AddOrderItem(new Guid(wiederzulassung.Header.ProductCostCenterList[0].ProductId), price.Amount, 1, new Guid(wiederzulassung.Header.ProductCostCenterList[0].CostCenterId), null, false, dbContext);

                if (price.AuthorativeCharge.HasValue)
                {
                    var newOrderItem2 = newRegistrationOrder.Order.AddOrderItem(new Guid(wiederzulassung.Header.ProductCostCenterList[0].ProductId), price.AuthorativeCharge.Value, 1, new Guid(wiederzulassung.Header.ProductCostCenterList[0].CostCenterId), newOrderItem1.Id, true, dbContext);
                }

                dbContext.SubmitChanges();

                //add all other products
                AddAnotherProducts(newRegistrationOrder, new Guid(wiederzulassung.Header.LocationId), wiederzulassung.Header, currentUserId);

                car.CurrentRegistrationId = newRegistration.Id;
                dbContext.SubmitChanges();
            }
            else
            {
                throw new Exception("Dienstleistung / Kostenstellen sind nicht miteingegeben!");
            }          
        }

        private static List<string> getOpenedRegistrationOrder(string vin)
        {
            List<string> orderNumbers = new List<string>();
            using (DataClasses1DataContext dbContext = new DataClasses1DataContext())
            {
                var ordernumber = from ro in dbContext.RegistrationOrder
                                  join o in dbContext.Order on ro.OrderId equals o.Id
                                  where ro.Vehicle.VIN == vin && o.Status < 600
                                  select new { o.Ordernumber };
                foreach (var item in ordernumber)
                {

                    orderNumbers.Add(item.Ordernumber.ToString());

                }
                return orderNumbers;
            }
        }

        private static List<string> getOpenedDeregistrationOrder(string vin)
        {
            List<string> orderNumbers = new List<string>();
            using (DataClasses1DataContext dbContext = new DataClasses1DataContext())
            {
                var ordernumber = from ro in dbContext.DeregistrationOrder
                                  join o in dbContext.Order on ro.OrderId equals o.Id
                                  where ro.Vehicle.VIN == vin && o.Status < 600
                                  select new { o.Ordernumber };
                foreach (var item in ordernumber)
                {
                    orderNumbers.Add(item.Ordernumber.ToString());
                }

                return orderNumbers;
            }
        }

        private static Price findPrice(RegistrationNeuzulassungHeader neuzulassung)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            Guid? locationId = null;
            Price newPrice = null;

            if (!String.IsNullOrEmpty(neuzulassung.ProductCostCenterList[0].ProductId))
            {           
                if (!String.IsNullOrEmpty(neuzulassung.LocationId))
                {
                    locationId = new Guid(neuzulassung.LocationId);
                    newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == new Guid(neuzulassung.ProductCostCenterList[0].ProductId) && q.LocationId == locationId);
                }

                if (String.IsNullOrEmpty(neuzulassung.LocationId) || newPrice == null)
                {
                    newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == new Guid(neuzulassung.ProductCostCenterList[0].ProductId) && q.LocationId == null);
                }
            }

            return newPrice;
        }

        private static Price findPrice(ReadmissionWiederzulassungHeader neuzulassung)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            Guid? locationId = null;
            Price newPrice = null;
            if (!String.IsNullOrEmpty(neuzulassung.ProductCostCenterList[0].ProductId))
            {
                if (!String.IsNullOrEmpty(neuzulassung.LocationId))
                {
                    locationId = new Guid(neuzulassung.LocationId);
                    newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == new Guid(neuzulassung.ProductCostCenterList[0].ProductId) && q.LocationId == locationId);
                }

                if (String.IsNullOrEmpty(neuzulassung.LocationId) || newPrice == null)
                {
                    newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == new Guid(neuzulassung.ProductCostCenterList[0].ProductId) && q.LocationId == null);
                }
            }
            return newPrice;
        }


        private static Price findPrice(DeregistrationAbmeldungHeader abmeldung)
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext();
            Guid? locationId = null;
            Price newPrice = null;
            if (!String.IsNullOrEmpty(abmeldung.ProductCostCenterList[0].ProductId))
            {
                if (!String.IsNullOrEmpty(abmeldung.LocationId))
                {
                    locationId = new Guid(abmeldung.LocationId);
                    newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == new Guid(abmeldung.ProductCostCenterList[0].ProductId) && q.LocationId == locationId);
                }

                if (String.IsNullOrEmpty(abmeldung.LocationId) || newPrice == null)
                {
                    newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == new Guid(abmeldung.ProductCostCenterList[0].ProductId) && q.LocationId == null);
                }
            }
            return newPrice;
        }


         private static Price findPrice(RemarkUmkennzeichnungHeader umkennzeichnung)
         {
             DataClasses1DataContext dbContext = new DataClasses1DataContext();
             Guid? locationId = null;
             Price newPrice = null;

             if (!String.IsNullOrEmpty(umkennzeichnung.ProductCostCenterList[0].ProductId))
             {
                 if (!String.IsNullOrEmpty(umkennzeichnung.LocationId))
                 {
                     locationId = new Guid(umkennzeichnung.LocationId);

                     newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == new Guid(umkennzeichnung.ProductCostCenterList[0].ProductId) && q.LocationId == locationId);
                 }

                 if (String.IsNullOrEmpty(umkennzeichnung.LocationId) || newPrice == null)
                 {
                     newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == new Guid(umkennzeichnung.ProductCostCenterList[0].ProductId) && q.LocationId == null);
                 }
             }
             return newPrice;
         }

        static bool ValidateGuid(string theGuid)
        {
            try { Guid aG = new Guid(theGuid); }
            catch { return false; }

            return true;
        }

        /// <summary>
        /// Prüfe ob die Guids im Header, auch Guids sind
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private static bool ValidateGuid(RemarkUmkennzeichnungHeader header)
        {
            try { Guid aG = new Guid(header.CustomerId); }
            catch { return false; }

            try 
            {
                foreach (var prodCostPair in header.ProductCostCenterList)
                {
                    Guid aG = new Guid(prodCostPair.CostCenterId); 
                }
               
            }
            catch 
            { 
                return false;
            }


            try { Guid aG = new Guid(header.LocationId); }
            catch { return false; }

            try 
            {
                foreach (var prodCostPair in header.ProductCostCenterList)
                {
                    Guid aG = new Guid(prodCostPair.ProductId);
                }
            }
            catch
            { 
                return false; 
            }

            return true;
        }

        /// <summary>
        /// Prüfe ob die Guids im Header, auch Guids sind
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private static bool ValidateGuid(RegistrationNeuzulassungHeader header)
        {       
            try { Guid aG = new Guid(header.CustomerId); }
            catch { return false; }
            try
            {
                foreach (var prodCostPair in header.ProductCostCenterList)
                {
                    Guid aG = new Guid(prodCostPair.CostCenterId);
                }

            }
            catch
            {
                return false;
            }
            try { Guid aG = new Guid(header.LocationId); }
            catch { return false; }
            try
            {
                foreach (var prodCostPair in header.ProductCostCenterList)
                {
                    Guid aG = new Guid(prodCostPair.ProductId);
                }

            }
            catch
            {
                return false;
            }
           
            return true;
        }

        /// <summary>
        /// Prüfe ob die Guids im Header, auch Guids sind
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private static bool ValidateGuid(ReadmissionWiederzulassungHeader header)
        {
            try { Guid aG = new Guid(header.CustomerId); }
            catch { return false; }
            try
            {
                foreach (var prodCostPair in header.ProductCostCenterList)
                {
                    Guid aG = new Guid(prodCostPair.CostCenterId);
                }

            }
            catch
            {
                return false;
            }
            try { Guid aG = new Guid(header.LocationId); }
            catch { return false; }
            try
            {
                foreach (var prodCostPair in header.ProductCostCenterList)
                {
                    Guid aG = new Guid(prodCostPair.ProductId);
                }

            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Prüfe ob die Guids im Header, auch Guids sind
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private static bool ValidateGuid(DeregistrationAbmeldungHeader header)
        {

            try { Guid aG = new Guid(header.CustomerId); }
            catch { return false; }
            try
            {
                foreach (var prodCostPair in header.ProductCostCenterList)
                {
                    Guid aG = new Guid(prodCostPair.CostCenterId);
                }

            }
            catch
            {
                return false;
            }
            try { Guid aG = new Guid(header.LocationId); }
            catch { return false; }
            try
            {
                foreach (var prodCostPair in header.ProductCostCenterList)
                {
                    Guid aG = new Guid(prodCostPair.ProductId);
                }

            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Prüfe ob die Guids im Header, auch im System vorhanden sind
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private static string ValidateGuid(RegistrationNeuzulassungHeader header, DataClasses1DataContext dbContext)
        {

            Customer cust = dbContext.Customer.SingleOrDefault(q=>q.Id==new Guid(header.CustomerId));
            if (cust == null)
            {
                return "Unbekannter Kunde";
            }
            Location loc = dbContext.Location.SingleOrDefault(q => q.Id == new Guid(header.LocationId) && q.CustomerId == new Guid(header.CustomerId));
            if (loc == null)
            {
                return "Unbekannte LocationId bei diesem Kunden";
            }

            foreach (var costProdPair in header.ProductCostCenterList)
            {
                CostCenter cost = dbContext.CostCenter.SingleOrDefault(q => q.Id == new Guid(costProdPair.CostCenterId) && q.CustomerId == new Guid(header.CustomerId));
                if (cost == null)
                {
                    return "Unbekannter Standort(Kostenstelle) bei diesem Kunden";
                }
            }

            foreach (var costProdPair in header.ProductCostCenterList)
            {
                Product product = dbContext.Product.SingleOrDefault(q => q.Id == new Guid(costProdPair.ProductId));
                if (product == null)
                {
                    return "Unbekannte ProductId";
                }
            }

            return string.Empty;
        }

        private static string ValidateGuid(RemarkUmkennzeichnungHeader header, DataClasses1DataContext dbContext)
        {

            Customer cust = dbContext.Customer.SingleOrDefault(q => q.Id == new Guid(header.CustomerId));
            if (cust == null)
            {
                return "Unbekannter Kunde";
            }
            Location loc = dbContext.Location.SingleOrDefault(q => q.Id == new Guid(header.LocationId) && q.CustomerId == new Guid(header.CustomerId));
            if (loc == null)
            {
                return "Unbekannte LocationId bei diesem Kunden";
            }
            foreach (var costProdPair in header.ProductCostCenterList)
            {
                CostCenter cost = dbContext.CostCenter.SingleOrDefault(q => q.Id == new Guid(costProdPair.CostCenterId) && q.CustomerId == new Guid(header.CustomerId));
                if (cost == null)
                {
                    return "Unbekannter Standort(Kostenstelle) bei diesem Kunden";
                }
            }

            foreach (var costProdPair in header.ProductCostCenterList)
            {
                Product product = dbContext.Product.SingleOrDefault(q => q.Id == new Guid(costProdPair.ProductId));
                if (product == null)
                {
                    return "Unbekannte ProductId";
                }
            }
           
            return string.Empty;
        }

        private static string ValidateGuid(ReadmissionWiederzulassungHeader header, DataClasses1DataContext dbContext)
        {
            Customer cust = dbContext.Customer.SingleOrDefault(q => q.Id == new Guid(header.CustomerId));
            if (cust == null)
            {
                return "Unbekannter Kunde";
            }
            Location loc = dbContext.Location.SingleOrDefault(q => q.Id == new Guid(header.LocationId) && q.CustomerId == new Guid(header.CustomerId));
            if (loc == null)
            {
                return "Unbekannte LocationId bei diesem Kunden";
            }
            foreach (var costProdPair in header.ProductCostCenterList)
            {
                CostCenter cost = dbContext.CostCenter.SingleOrDefault(q => q.Id == new Guid(costProdPair.CostCenterId) && q.CustomerId == new Guid(header.CustomerId));
                if (cost == null)
                {
                    return "Unbekannter Standort(Kostenstelle) bei diesem Kunden";
                }
            }
            foreach (var costProdPair in header.ProductCostCenterList)
            {
                Product product = dbContext.Product.SingleOrDefault(q => q.Id == new Guid(costProdPair.ProductId));
                if (product == null)
                {
                    return "Unbekannte ProductId";
                }
            }


            return string.Empty;
        }

        private static string ValidateGuid(DeregistrationAbmeldungHeader header, DataClasses1DataContext dbContext)
        {
            Customer cust = dbContext.Customer.SingleOrDefault(q => q.Id == new Guid(header.CustomerId));
            if (cust == null)
            {
                return "Unbekannter Kunde";
            }

            Location loc = dbContext.Location.SingleOrDefault(q => q.Id == new Guid(header.LocationId) && q.CustomerId == new Guid(header.CustomerId));
            if (loc == null)
            {
                return "Unbekannte LocationId bei diesem Kunden";
            }

            foreach (var costProdPair in header.ProductCostCenterList)
            {
                CostCenter cost = dbContext.CostCenter.SingleOrDefault(q => q.Id == new Guid(costProdPair.CostCenterId) && q.CustomerId == new Guid(header.CustomerId));
                if (cost == null)
                {
                    return "Unbekannter Standort(Kostenstelle) bei diesem Kunden";
                }
            }

            foreach (var costProdPair in header.ProductCostCenterList)
            {
                Product product = dbContext.Product.SingleOrDefault(q => q.Id == new Guid(costProdPair.ProductId));
                if (product == null)
                {
                    return "Unbekannte ProductId";
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Prüft ob dieses Fahzeug bereits im System bekannt ist
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        private static Vehicle IsVehicleInSystem(RegistrationNeuzulassungVehicle vehicle, DataClasses1DataContext dbContext)
        {
            return dbContext.Vehicle.SingleOrDefault(q => q.VIN == vehicle.VIN);
        }

        private static Vehicle IsVehicleInSystem(DeregistrationAbmeldungVehicle vehicle, DataClasses1DataContext dbContext)
        {
            return dbContext.Vehicle.SingleOrDefault(q => q.VIN == vehicle.VIN);
        }

        private static Vehicle IsVehicleInSystem(ReadmissionWiederzulassungVehicle vehicle, DataClasses1DataContext dbContext)
        {
            return dbContext.Vehicle.SingleOrDefault(q => q.VIN == vehicle.VIN);
        }

        private static Vehicle IsVehicleInSystem(RemarkUmkennzeichnungVehicle vehicle, DataClasses1DataContext dbContext)
        {
            return dbContext.Vehicle.SingleOrDefault(q => q.VIN == vehicle.VIN);
        }

        public static Kundendaten GetCutomerInformation(out Kundendaten _object, List<Guid> allowedCustomers, DataClasses1DataContext dbContext)
        {
            _object = new Kundendaten();
            var queryCustomer = from cust in dbContext.Customer
                                where allowedCustomers.Contains(cust.Id)
                                select new { cust.Id, cust.Name };

            _object.Customer = new KundendatenCustomer[queryCustomer.Count()];

            int countCustomer = 0;

            foreach (var custItem in queryCustomer)
            {
                int countLocation = 0;
                _object.Customer[countCustomer] = new KundendatenCustomer();

                _object.Customer[countCustomer].Informationen = new KundendatenCustomerInformationen();

                var currentCustomerLocations = from cl in dbContext.Location where cl.CustomerId == custItem.Id select new { cl.Id, cl.Name };
                _object.Customer[countCustomer].Informationen.CustomerLocations = new KundendatenCustomerInformationenCustomerLocations[currentCustomerLocations.Count()];
                _object.Customer[countCustomer].Informationen.CustomerId = custItem.Id.ToString();
                _object.Customer[countCustomer].Informationen.CustomerName = custItem.Name.ToString();


                foreach (var locItems in currentCustomerLocations)
                {
                    _object.Customer[countCustomer].Informationen.CustomerLocations[countLocation] = new KundendatenCustomerInformationenCustomerLocations();
                    _object.Customer[countCustomer].Informationen.CustomerLocations[countLocation].LocationId = locItems.Id.ToString();
                    _object.Customer[countCustomer].Informationen.CustomerLocations[countLocation].LocationName = locItems.Name.ToString();
                    countLocation++;
                }

                int countCostCenter = 0;
         
                var currentCustomerCostCenter = from cl in dbContext.CostCenter where cl.CustomerId == custItem.Id select new { cl.Id, cl.Name };
                _object.Customer[countCustomer].Informationen.CustomerCostCenter= new KundendatenCustomerInformationenCustomerCostCenter[currentCustomerCostCenter.Count()];

                foreach (var costItems in currentCustomerCostCenter)
                {
                    _object.Customer[countCustomer].Informationen.CustomerCostCenter[countCostCenter] = new KundendatenCustomerInformationenCustomerCostCenter();
                    _object.Customer[countCustomer].Informationen.CustomerCostCenter[countCostCenter].CostCenterId = costItems.Id.ToString();
                    _object.Customer[countCustomer].Informationen.CustomerCostCenter[countCostCenter].CostCenterName = costItems.Name.ToString();
                    countCostCenter++;
                }

                countCustomer++;
            }

            var queryProducts = from pr in dbContext.Product select new { pr.Id, pr.Name, pr.ItemNumber };
            _object.Produkte = new KundendatenProdukte[queryProducts.Count()];

            int countProducts = 0;

            foreach (var productItem in queryProducts)
            {
                _object.Produkte[countProducts] = new KundendatenProdukte();
                _object.Produkte[countProducts].Informationen = new KundendatenProdukteInformationen();
                _object.Produkte[countProducts].Informationen.ProductId = productItem.Id.ToString();
                _object.Produkte[countProducts].Informationen.ProductName = productItem.Name;
                _object.Produkte[countProducts].Informationen.ProductId = productItem.Id.ToString();
                countProducts++;
            }

            return _object;
        }

        private static void AddAnotherProducts(RegistrationOrder regOrd, Guid? locationId, RemarkUmkennzeichnungHeader ProductCostCenterListHeader, Guid currentUserId)
        {
            bool allesHatGutGelaufen = false;
            string ProduktId = "";
            string CostCenterId = "";
            int skipFirst = 0;
            foreach (var productCostCenterPair in ProductCostCenterListHeader.ProductCostCenterList)
            {
                if (skipFirst > 0)
                {
                    if (!String.IsNullOrEmpty(productCostCenterPair.ProductId) && !String.IsNullOrEmpty(productCostCenterPair.CostCenterId))
                    {
                        try
                        {
                            DataClasses1DataContext dbContext = new DataClasses1DataContext(currentUserId);
                            Guid orderId = regOrd.OrderId;
                            Price newPrice;
                            OrderItem newOrderItem1 = null;
                            ProduktId = productCostCenterPair.ProductId;
                            CostCenterId = productCostCenterPair.CostCenterId;
                            if (!String.IsNullOrEmpty(ProduktId))
                            {
                                Guid productId = new Guid(ProduktId);
                                Guid costCenterId = Guid.Empty;
                                KVSCommon.Database.Product newProduct = dbContext.Product.SingleOrDefault(q => q.Id == productId);
                                if (locationId == null) //small
                                {
                                    newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == null);
                                }
                                else //large
                                {
                                    costCenterId = new Guid(CostCenterId);
                                    newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == locationId);
                                    if (newPrice == null)
                                        newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == null);
                                }
                                var orderToUpdate = dbContext.Order.SingleOrDefault(q => q.Id == orderId);
                                orderToUpdate.LogDBContext = dbContext;
                                if (orderToUpdate != null)
                                {
                                    newOrderItem1 = orderToUpdate.AddOrderItem(newProduct.Id, newPrice.Amount, 1, costCenterId, null, false, dbContext);
                                    if (newPrice.AuthorativeCharge.HasValue)
                                    {
                                        orderToUpdate.AddOrderItem(newProduct.Id, newPrice.AuthorativeCharge.Value, 1, costCenterId, newOrderItem1.Id, newPrice.AuthorativeCharge.HasValue, dbContext);
                                    }
                                    dbContext.SubmitChanges();
                                    allesHatGutGelaufen = true;
                                }
                            }
                            if (allesHatGutGelaufen)
                                dbContext.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
                else
                {
                    skipFirst = 1;
                }
            }
        }
        
        private static void AddAnotherProducts(RegistrationOrder regOrd, Guid? locationId, RegistrationNeuzulassungHeader ProductCostCenterListHeader, Guid currentUserId)
        {
            bool allesHatGutGelaufen = false;
            string ProduktId = "";
            string CostCenterId = "";
            int skipFirst = 0;
            foreach (var productCostCenterPair in ProductCostCenterListHeader.ProductCostCenterList)
            {
                if (skipFirst > 0)
                {
                    if (!String.IsNullOrEmpty(productCostCenterPair.ProductId) && !String.IsNullOrEmpty(productCostCenterPair.CostCenterId))
                    {
                        try
                        {
                            DataClasses1DataContext dbContext = new DataClasses1DataContext(currentUserId);
                            Guid orderId = regOrd.OrderId;
                            Price newPrice;
                            OrderItem newOrderItem1 = null;
                            ProduktId = productCostCenterPair.ProductId;
                            CostCenterId = productCostCenterPair.CostCenterId;
                            if (!String.IsNullOrEmpty(ProduktId))
                            {
                                Guid productId = new Guid(ProduktId);
                                Guid costCenterId = Guid.Empty;
                                KVSCommon.Database.Product newProduct = dbContext.Product.SingleOrDefault(q => q.Id == productId);
                                if (locationId == null) //small
                                {
                                    newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == null);
                                }
                                else //large
                                {
                                    costCenterId = new Guid(CostCenterId);
                                    newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == locationId);
                                    if (newPrice == null)
                                        newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == null);
                                }
                                var orderToUpdate = dbContext.Order.SingleOrDefault(q => q.Id == orderId);
                                orderToUpdate.LogDBContext = dbContext;
                                if (orderToUpdate != null)
                                {
                                    newOrderItem1 = orderToUpdate.AddOrderItem(newProduct.Id, newPrice.Amount, 1, costCenterId, null, false, dbContext);
                                    if (newPrice.AuthorativeCharge.HasValue)
                                    {
                                        orderToUpdate.AddOrderItem(newProduct.Id, newPrice.AuthorativeCharge.Value, 1, costCenterId, newOrderItem1.Id, newPrice.AuthorativeCharge.HasValue, dbContext);
                                    }
                                    dbContext.SubmitChanges();
                                    allesHatGutGelaufen = true;
                                }
                            }
                            if (allesHatGutGelaufen)
                                dbContext.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
                else
                {
                    skipFirst = 1;
                }
            }
        }


        private static void AddAnotherProducts(DeregistrationOrder deRegOrd, Guid? locationId, DeregistrationAbmeldungHeader ProductCostCenterListHeader, Guid currentUserId)
        {
            bool allesHatGutGelaufen = false;
            string ProduktId = "";
            string CostCenterId = "";
            int skipFirst = 0;
            foreach (var productCostCenterPair in ProductCostCenterListHeader.ProductCostCenterList)
            {
                if (skipFirst > 0)
                {
                    if (!String.IsNullOrEmpty(productCostCenterPair.ProductId) && !String.IsNullOrEmpty(productCostCenterPair.CostCenterId))
                    {
                        try
                        {
                            DataClasses1DataContext dbContext = new DataClasses1DataContext(currentUserId);
                            Guid orderId = deRegOrd.OrderId;
                            Price newPrice;
                            OrderItem newOrderItem1 = null;
                            ProduktId = productCostCenterPair.ProductId;
                            CostCenterId = productCostCenterPair.CostCenterId;
                            if (!String.IsNullOrEmpty(ProduktId))
                            {
                                Guid productId = new Guid(ProduktId);
                                Guid costCenterId = Guid.Empty;
                                KVSCommon.Database.Product newProduct = dbContext.Product.SingleOrDefault(q => q.Id == productId);
                                if (locationId == null) //small
                                {
                                    newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == null);
                                }
                                else //large
                                {
                                    costCenterId = new Guid(CostCenterId);
                                    newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == locationId);
                                    if (newPrice == null)
                                        newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == null);
                                }
                                var orderToUpdate = dbContext.Order.SingleOrDefault(q => q.Id == orderId);
                                orderToUpdate.LogDBContext = dbContext;
                                if (orderToUpdate != null)
                                {
                                    newOrderItem1 = orderToUpdate.AddOrderItem(newProduct.Id, newPrice.Amount, 1, costCenterId, null, false, dbContext);
                                    if (newPrice.AuthorativeCharge.HasValue)
                                    {
                                        orderToUpdate.AddOrderItem(newProduct.Id, newPrice.AuthorativeCharge.Value, 1, costCenterId, newOrderItem1.Id, newPrice.AuthorativeCharge.HasValue, dbContext);
                                    }
                                    dbContext.SubmitChanges();
                                    allesHatGutGelaufen = true;
                                }
                            }
                            if (allesHatGutGelaufen)
                                dbContext.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
                else
                {
                    skipFirst = 1;
                }
            }
        }

        private static void AddAnotherProducts(RegistrationOrder regOrd, Guid? locationId, ReadmissionWiederzulassungHeader ProductCostCenterListHeader, Guid currentUserId)
        {
            bool allesHatGutGelaufen = false;
            string ProduktId = "";
            string CostCenterId = "";
            int skipFirst = 0;
            foreach (var productCostCenterPair in ProductCostCenterListHeader.ProductCostCenterList)
            {
                if (skipFirst > 0)
                {
                    if (!String.IsNullOrEmpty(productCostCenterPair.ProductId) && !String.IsNullOrEmpty(productCostCenterPair.CostCenterId))
                    {
                        try
                        {
                            DataClasses1DataContext dbContext = new DataClasses1DataContext(currentUserId);
                            Guid orderId = regOrd.OrderId;
                            Price newPrice;
                            OrderItem newOrderItem1 = null;
                            ProduktId = productCostCenterPair.ProductId;
                            CostCenterId = productCostCenterPair.CostCenterId;
                            if (!String.IsNullOrEmpty(ProduktId))
                            {
                                Guid productId = new Guid(ProduktId);
                                Guid costCenterId = Guid.Empty;
                                KVSCommon.Database.Product newProduct = dbContext.Product.SingleOrDefault(q => q.Id == productId);
                                if (locationId == null) //small
                                {
                                    newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == null);
                                }
                                else //large
                                {
                                    costCenterId = new Guid(CostCenterId);
                                    newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == locationId);
                                    if (newPrice == null)
                                        newPrice = dbContext.Price.SingleOrDefault(q => q.ProductId == newProduct.Id && q.LocationId == null);
                                }
                                var orderToUpdate = dbContext.Order.SingleOrDefault(q => q.Id == orderId);
                                orderToUpdate.LogDBContext = dbContext;
                                if (orderToUpdate != null)
                                {
                                    newOrderItem1 = orderToUpdate.AddOrderItem(newProduct.Id, newPrice.Amount, 1, costCenterId, null, false, dbContext);
                                    if (newPrice.AuthorativeCharge.HasValue)
                                    {
                                        orderToUpdate.AddOrderItem(newProduct.Id, newPrice.AuthorativeCharge.Value, 1, costCenterId, newOrderItem1.Id, newPrice.AuthorativeCharge.HasValue, dbContext);
                                    }
                                    dbContext.SubmitChanges();
                                    allesHatGutGelaufen = true;
                                }
                            }
                            if (allesHatGutGelaufen)
                                dbContext.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
                else
                {
                    skipFirst = 1;
                }
            }
        }
    }
}