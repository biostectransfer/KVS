using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse der Grosskunden DB
    /// </summary>
    public partial class LargeCustomer : ILogging
    {
        public DataClasses1DataContext LogDBContext
        {
            get;
            set;
        }

        public object ItemId
        {
            get
            {
                return this.CustomerId;
            }
        }

        public EntityState EntityState
        {
            get;
            set;
        }
        /// <summary>
        /// Loescht einen Großkunden, aber nur wenn der Kunde mit keinen Aufträgen und Rechnungen verknüpft ist
        /// </summary>
        /// <param name="dbContext">DB Kontext</param>
        /// <param name="smallCustomerId">Laufkundenid</param>
        public static void RemoveLargeCutomer(DataClasses1DataContext dbContext, Guid smallCustomerId)
        {
            var lmCustomer = dbContext.LargeCustomer.FirstOrDefault(q => q.CustomerId == smallCustomerId);
            if (lmCustomer == null)
                throw new Exception("Der Kunde wurde in der Datenbank nicht gefunden");
            if (lmCustomer.Customer.Order != null && lmCustomer.Customer.Order.Count > 0)
                throw new Exception("Das löschen ist nicht möglich, da der Kunde bereits mit Aufträgen verknüpft ist");
            if (lmCustomer.Customer.Invoice != null && lmCustomer.Customer.Invoice.Count > 0)
                throw new Exception("Das löschen ist nicht möglich, da der Kunde bereits mit Rechnungen verknüpft ist");
            if (lmCustomer.Customer.CustomerProduct != null && lmCustomer.Customer.CustomerProduct.Count > 0)
                throw new Exception("Das löschen ist nicht möglich, da der Kunde bereits mit Produkten verknüpft ist");

            if (lmCustomer.MainLocationId != null)
                KVSCommon.Database.Location.ChangeLocations(dbContext, lmCustomer.MainLocation);
            if (lmCustomer.Location != null && lmCustomer.Location.Count > 0)
            {
                foreach (var loc in lmCustomer.Location)
                {
                    KVSCommon.Database.Location.ChangeLocations(dbContext, loc);
                   
                }
            }
            
            foreach (var csId in lmCustomer.CostCenter)
                KVSCommon.Database.CostCenter.RemoveCostCenter(csId.Id, dbContext);

            if (lmCustomer.Customer.ContactId != null)
                Contact.DeleteContact(lmCustomer.Customer.ContactId.Value, dbContext);

            Adress.DeleteAdress(lmCustomer.Customer.AdressId, dbContext);
            Adress.DeleteAdress(lmCustomer.Customer.InvoiceAdressId, dbContext);
            Adress.DeleteAdress(lmCustomer.Customer.InvoiceDispatchAdressId, dbContext);
            //if(lmCustomer.PersonId != null)
            //Person.DeletePerson(lmCustomer.PersonId.Value, dbContext);

            if(lmCustomer.MainLocationId != null)
            KVSCommon.Database.Location.RemoveLocation(lmCustomer.MainLocationId.Value, dbContext);


            if (lmCustomer.Location != null && lmCustomer.Location.Count > 0)
            {
                foreach (var loc in lmCustomer.Location)
                {
                    KVSCommon.Database.Location.RemoveLocation(loc.Id, dbContext);
                }
            }
            if(lmCustomer.PersonId.HasValue)
            Person.DeletePerson(lmCustomer.PersonId.Value, dbContext);
            //if (lmCustomer.BankAccountId != null)
            //    BankAccount.DeleteBank(smCustomer.BankAccountId.Value, dbContext);

            dbContext.LargeCustomer.DeleteOnSubmit(lmCustomer);
            dbContext.Customer.DeleteOnSubmit(lmCustomer.Customer);

            dbContext.WriteLogItem("Kunde " + lmCustomer.CustomerId + " mit dem Namen: " + lmCustomer.Customer.Name + " wurde gelöscht.", LogTypes.DELETE, lmCustomer.CustomerId, "LargeCustomer");


        }
        /// <summary>
        /// Trägt einen neuen Großkunden in die Datenbank ein.
        /// </summary>
        /// <param name="name">Name des Kunden.</param>
        /// <param name="street">Straße des Kunden.</param>
        /// <param name="streetnumber">Hausnummer des Kunden.</param>
        /// <param name="zipcode">Postleitzahl des Kunden.</param>
        /// <param name="city">Ort des Kunden.</param>
        /// <param name="country">Land des Kunden.</param>
        /// <param name="phone">Telefonnummer des Kunden.</param>
        /// <param name="fax">Faxnummer des Kunden.</param>
        /// <param name="mobilephone">Mobilnummer des Kunden.</param>
        /// <param name="email">Emailadresse des Kunden.</param>
        /// <param name="vat">Mehrwertsteuersatz für diesen Kunden.</param>
        /// <param name="sendInvoiceByEmail">Gibt an, ob die Rechnungen per Email versendet werden sollen.</param>
        /// <param name="sendInvoiceToMainLocation">Gibt an, ob für den Rechnungsversand der Hauptstandort verwendet werden soll.</param>
        /// <param name="termOfCredit">Zahlungsziel (in Tagen).</param>
        /// <param name="customerNumber">Kundennummer.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Den neuen Großkunden.</returns>
        /// <remarks>Innerhalb des Aufrufs wird bereits dbContext.SubmitChanges() aufgerufen!</remarks>
        public static LargeCustomer CreateLargeCustomer( DataClasses1DataContext dbContext ,string name, string street, string streetnumber, string zipcode, string city, string country, string phone, string fax, string mobilephone, string email, decimal vat, bool sendInvoiceToMainLocation, bool sendInvoiceByEmail, int? termOfCredit, string customerNumber,string matchcode, string Debitornumber, Guid? PersonId, Guid? InvoiceType, string evbNumber)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Es wurde kein Name für den Kunden angegeben.");
            }

            var customer = Customer.CreateCustomer(name, street, streetnumber, zipcode, city, country, phone, fax, mobilephone, email, vat, termOfCredit, customerNumber, matchcode, Debitornumber, evbNumber);
            customer._dbContext = dbContext;
            var largeCustomer = new LargeCustomer()
            {
                Customer = customer,
                PersonId = PersonId,
                SendInvoiceByEmail = sendInvoiceByEmail,
                SendInvoiceToMainLocation = sendInvoiceToMainLocation,
                OrderFinishedNoteSendType = 0,
                InvoiceTypesID = InvoiceType
            };

            dbContext.WriteLogItem("Kunde " + name + " wurde angelegt.", LogTypes.INSERT, customer.Id, "LargeCustomer");
            dbContext.LargeCustomer.InsertOnSubmit(largeCustomer);
            dbContext.SubmitChanges();
            var mainLocation = largeCustomer.AddNewLocation("Hauptstandort", street, streetnumber, zipcode, city, country, phone, fax, mobilephone, email, vat, dbContext);
            largeCustomer.MainLocation = mainLocation;
            mainLocation._dbContext = dbContext;
            dbContext.SubmitChanges();
            return largeCustomer;
        }

        /// <summary>
        /// Fügt dem Grosskunden einen neuen Standort hinzu.
        /// </summary>
        /// <param name="name">Name des Standorts.</param>
        /// <param name="street">Straße des Standorts.</param>
        /// <param name="streetnumber">Hausnummer des Standorts.</param>
        /// <param name="zipcode">Postleitzahl des Standorts.</param>
        /// <param name="city">Ort des Standorts.</param>
        /// <param name="country">Land des Standorts.</param>
        /// <param name="phone">Telefonnummer des Standorts.</param>
        /// <param name="fax">Faxnummer des Standorts.</param>
        /// <param name="mobilephone">Mobiltelefonnummer des Standorts.</param>
        /// <param name="email">Emailadresse des Standorts.</param>        
        /// <param name="vat">Mehrwertsteuersatz für den Standort.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Den neuen Standort.</returns>
        public Location AddNewLocation(string name, string street, string streetnumber, string zipcode, string city, string country, string phone, string fax, string mobilephone, string email, decimal? vat, DataClasses1DataContext dbContext)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Der Name des Standorts darf nicht leer sein.");
            }

            if (this.Location.Any(q => q.Name == name))
            {
                throw new Exception("Der Kunde " + this.Customer.Name + " besitzt bereits einen Standort mit Namen " + name + ".");
            }
            
            Location location = new Location()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Adress = new Adress()
                {
                    Id = Guid.NewGuid(),
                    Zipcode = zipcode,
                    Country = country,
                    City = city,
                    StreetNumber = streetnumber,
                    Street = street
                },
                Contact = new Contact()
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    MobilePhone = mobilephone,
                    Fax = fax,
                    Phone = phone
                },
                VAT = vat
            };
            location._dbContext = dbContext;
            this.Location.Add(location);
            
            dbContext.WriteLogItem("Neuer Standort " + name + " angelegt.", LogTypes.INSERT, this.CustomerId, "Customer", location.Id);
            return location;
        }

        /// <summary>
        /// Fügt dem Grosskunden eine neue Kostenstelle hinzu,
        /// </summary>
        /// <param name="name">Name der Kostenstelle.</param>
        /// <param name="costcenterNumber">Kostenstellennummer.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Die neue Kostenstelle.</returns>
        public CostCenter AddNewCostCenter(string name, string costcenterNumber, DataClasses1DataContext dbContext)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Der Name der Kostenstelle darf nicht leer sein.");
            }

            if (string.IsNullOrEmpty(costcenterNumber))
            {
                throw new ArgumentNullException("Die Kostenstellennummer darf nicht leer sein.");
            }

            if (this.CostCenter.Any(q => q.Name == name))
            {
                throw new Exception("Der Kunde " + this.Customer.Name + " besitzt bereits eine Kostenstelle mit Namen " + name + ".");
            }

            CostCenter costcenter = new CostCenter()
            {
                Id = Guid.NewGuid(),
                Name = name,
                CostcenterNumber = costcenterNumber
            };
            costcenter._dbContext = dbContext;
            this.CostCenter.Add(costcenter);
            dbContext.WriteLogItem("Neue Kostenstelle " + name + " angelegt.", LogTypes.INSERT, this.CustomerId, "Customer", costcenter.Id);
            return costcenter;
        }

        /// <summary>
        /// Fügt einem Emailverteiler des Kunden einen Eintrag hinzu.
        /// </summary>
        /// <param name="email">Emailadresse.</param>
        /// <param name="typeId">Id des Typs des Verteilers.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Den neuen Eintrag.</returns>
        public Mailinglist AddNewMailinglistItem(string email, Guid typeId, DataClasses1DataContext dbContext)
        {
            var type = dbContext.MailinglistType.Single(q => q.Id == typeId);
            if (this.Mailinglist.Any(q => q.Email == email && q.LocationId.HasValue == false && q.MailinglistTypeId == typeId))
            {
                throw new Exception("Die Emailadresse " + email + " ist bereits im Verteiler " + type.Name + ".");
            }

            var ml = Database.Mailinglist.CreateMailinglistItem(email, typeId, this.CustomerId, null, dbContext);
            dbContext.WriteLogItem("Neue Email " + email + " in den Verteiler " + type + " des Kunden " + this.Customer.Name + " aufgenommen.", LogTypes.INSERT, this.CustomerId, "LargeCustomer", ml.Id);
            return ml;
        }

        /// <summary>
        /// Entfernt einen Eintrag aus dem Emailverteiler des Kunden.
        /// </summary>
        /// <param name="mailinglistId">Id des Eintrags.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        public void RemoveMailinglistItem(Guid mailinglistId, DataClasses1DataContext dbContext)
        {
            Mailinglist ml = this.Mailinglist.SingleOrDefault(q => q.Id == mailinglistId);
            if (ml == null)
            {
                throw new Exception("Eintrag im Mailverteiler des Kunden " + this.Customer.Name + " mit der Id " + mailinglistId + " ist nicht vorhanden.");
            }

            dbContext.WriteLogItem("Email " + ml.Email + " aus dem Verteiler " + ml.MailinglistType.Name + " des Kunden entfernt.", LogTypes.DELETE, this.CustomerId, "LargeCustomer");
            dbContext.Mailinglist.DeleteOnSubmit(ml);
        }

        /// <summary>
        /// Fügt beim Grosskunden ein Feld zur den Pflichtfeldern bei der Auftragsanlage hinzu.
        /// </summary>
        /// <param name="requiredFieldId">Id des Felds.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        public void AddRequiredField(Guid requiredFieldId, DataClasses1DataContext dbContext)
        {
            var requiredField = dbContext.RequiredField.Single(q => q.Id == requiredFieldId);
            if (!this.LargeCustomerRequiredField.Any(q => q.RequiredFieldId == requiredFieldId))
            {
                dbContext.WriteLogItem("Feld " + requiredField.Name + " beim Kunden " + this.Customer.Name + " als Pflichtfeld festgelegt.", LogTypes.INSERT, this.CustomerId, "LargeCustomer", requiredFieldId);
                this.LargeCustomerRequiredField.Add(new LargeCustomerRequiredField()
                {
                    RequiredFieldId = requiredFieldId
                });
            }
        }

        /// <summary>
        /// Entfernt beim Grosskunden ein Feld aus den Pflichtfeldern für die Auftragsanlage.
        /// </summary>
        /// <param name="requiredFieldId">Id des Felds.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        public void RemoveRequiredField(Guid requiredFieldId, DataClasses1DataContext dbContext)
        {
            var requiredField = dbContext.RequiredField.Single(q => q.Id == requiredFieldId);
            LargeCustomerRequiredField largeCustomerRequiredField = this.LargeCustomerRequiredField.SingleOrDefault(q => q.RequiredFieldId == requiredFieldId);
            if (largeCustomerRequiredField == null)
            {
                throw new Exception("Das Feld " + requiredField.Name + " ist beim Kunden " + this.Customer.Name + " nicht als Pflichtfeld definiert.");
            }

            dbContext.WriteLogItem("Feld " + requiredField.Name + " beim Kunden " + this.Customer.Name + " als Pflichtfeld entfernt.", LogTypes.DELETE, this.CustomerId, "LargeCustomer", requiredFieldId);
            dbContext.LargeCustomerRequiredField.DeleteOnSubmit(largeCustomerRequiredField);
        }

        /// <summary>
        /// Gibt eine Liste mit den für die Auftragsanlage benötigten Feldern zurück.
        /// </summary>
        /// <param name="orderTypeId">Id der Auftragsart.</param>
        /// <returns></returns>
        public List<string> GetRequiredFieldNames(Guid orderTypeId)
        {
            return this.LargeCustomerRequiredField
                .Select(q => q.RequiredField)
                .Where(q => q.OrderTypeId == orderTypeId)
                .Select(q => q.Name)
                .ToList();
        }

        /// <summary>
        /// Gibt die Liste der Emailadressen aus dem angegebenen Verteiler zurück.
        /// </summary>
        /// <param name="locationId">Id des Standortes, falls gewünscht.</param>
        /// <param name="type">Art des Verteilers.</param>
        /// <returns>Liste mit Emailadressen.</returns>
        /// <remarks>Wenn der Verteiler eines Standorts abgefragt wird, und dieser leer ist, werden die Adressen aus dem Verteiler des Kunden zurückgegeben.</remarks>
        public List<string> GetMailinglistAdresses(DataClasses1DataContext dbContext, Guid? locationId, string type)
        {
            if (locationId.HasValue)
            {
          
                    var emails = dbContext.Mailinglist.Where(q => q.LocationId == locationId.Value && q.MailinglistType.Name == type).Select(q => q.Email);
                    if (emails.Count() > 0)
                    {
                        return emails.ToList();
                    }
              
            }

            return this.Mailinglist.Where(q => q.LocationId.HasValue == false && q.MailinglistType.Name == type).Select(q => q.Email).ToList();
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
        partial void OnOrderFinishedNoteSendTypeChanging(byte value)
        {
            this.WriteUpdateLogItem("Sendeart für Auftragserledigungsemails", this.OrderFinishedNoteSendType, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnSendInvoiceByEmailChanging(bool value)
        {
            this.WriteUpdateLogItem("Rechnung Senden per Email", this.SendInvoiceByEmail, value);
        }

        partial void OnSendInvoiceToMainLocationChanging(bool value)
        {
            this.WriteUpdateLogItem("Rechnung Senden an Hauptstandort", this.SendInvoiceToMainLocation, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnSendOrderFinishedNoteToCustomerChanging(bool? value)
        {
            this.WriteUpdateLogItem("Auftragserledigungsemails an Kunden", this.SendOrderFinishedNoteToCustomer, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnSendOrderFinishedNoteToLocationChanging(bool? value)
        {
            this.WriteUpdateLogItem("Auftragserledigungsemails an Standort", this.SendOrderFinishedNoteToLocation, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnSendPackingListToCustomerChanging(bool? value)
        {
            this.WriteUpdateLogItem("Lieferschein an Kunden", this.SendPackingListToCustomer, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnSendPackingListToLocationChanging(bool? value)
        {
            this.WriteUpdateLogItem("Lieferschein an Standort", this.SendPackingListToLocation, value);
        }
    }
}
