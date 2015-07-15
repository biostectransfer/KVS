using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    /// <summary>
    /// Erweiterungsklasse der Location DB
    /// </summary>
    public partial class Location : ILogging
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
                return this.Id;
            }
        }

        public EntityState EntityState
        {
            get;
            set;
        }
        private DataClasses1DataContext myDbContext;
        public DataClasses1DataContext _dbContext
        {
            get { return myDbContext; }
            set {  myDbContext = value; }
        } 
        /// <summary>
        /// Fügt dem Verteiler des angegebenen Typs des Standorts einen neuen Eintrag hinzu.
        /// </summary>
        /// <param name="email">Emailadresse.</param>
        /// <param name="typeId">Id des Verteilertyps.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        /// <returns>Den neuen Verteilereintrag.</returns>
        public Mailinglist AddNewMailinglistItem(string email, Guid typeId, DataClasses1DataContext dbContext)
        {
            MailinglistType type = dbContext.MailinglistType.Single(q => q.Id == typeId);
            if (this.Mailinglist.Any(q => q.Email == email && q.MailinglistTypeId == typeId))
            {
                throw new Exception("Die Emailadresse " + email + " ist bereits im Verteiler " + type.Name + ".");
            }

            var ml = Database.Mailinglist.CreateMailinglistItem(email, typeId, null, this.Id, dbContext);
            dbContext.WriteLogItem("Neue Email " + email + " in den Verteiler " + type.Name + " des Standorts " + this.Name + " aufgenommen.", LogTypes.INSERT, this.Id, "Location", ml.Id);
            return ml;
        }

        /// <summary>
        /// Entfernt einen Eintrag aus dem Emailverteiler des Standorts.
        /// </summary>
        /// <param name="mailinglistId">Id des Eintrags.</param>
        /// <param name="dbContext">Datenbankkontext für die Transaktion.</param>
        public void RemoveMailinglistItem(Guid mailinglistId, DataClasses1DataContext dbContext)
        {
            Mailinglist ml = this.Mailinglist.SingleOrDefault(q => q.Id == mailinglistId);
            if (ml == null)
            {
                throw new Exception("Eintrag im Mailverteiler des Standorts " + this.Name + " mit der Id " + mailinglistId + " ist nicht vorhanden.");
            }

            dbContext.WriteLogItem("Email " + ml.Email + " aus dem Verteiler " + ml.MailinglistType.Name + " des Standorts entfernt.", LogTypes.DELETE, this.Id, "Location");
            dbContext.Mailinglist.DeleteOnSubmit(ml);
        }
        /// <summary>
        /// Löscht die angegebenen Standort aus der Datenbank.
        /// </summary>
        /// <param name="adressId">Id des Standorts.</param>
        /// <param name="dbContext">Datenbankkontext</param>
        public static void RemoveLocation(Guid locationId, DataClasses1DataContext dbContext)
        {
            Location lc = dbContext.Location.SingleOrDefault(q => q.Id ==locationId);
            if (lc != null)
            {
                if (lc.Order != null && lc.Order.Count > 0)
                    throw new Exception("Der Standort " + lc.Name + " kann nicht gelöscht werden, diese ist mit Aufträgen verknüft");

                if (lc.Price != null && lc.Price.Count > 0)
                    throw new Exception("Der Standort " + lc.Name + "  kann nicht gelöscht werden, dieser ist mit Preisen verknüft");
            

                if (lc.ContactId.HasValue)
                    Contact.DeleteContact(lc.ContactId.Value, dbContext);
                if (lc.InvoiceDispatchAdressId.HasValue)
                    Adress.DeleteAdress(lc.InvoiceDispatchAdressId.Value, dbContext);


                if (lc.InvoiceAdressId.HasValue)
                    Adress.DeleteAdress(lc.InvoiceAdressId.Value, dbContext);

                var supLoc = dbContext.Location.Where(q => q.SuperLocationId == lc.Id);
                foreach (var sl in supLoc)
                    sl.SuperLocationId = null;




                dbContext.WriteLogItem("Location mit der Id:" + lc.Id + " und dem Namen: " + lc.Name + " gelöscht.", LogTypes.DELETE, locationId, "Location");
                dbContext.Location.DeleteOnSubmit(lc);
            }
        }
        /// <summary>
        /// Loescht den aktuellen Standort
        /// </summary>
        /// <param name="dbContext">DB Kontext</param>
        /// <param name="lc">Standtort Objekt</param>
        public static void ChangeLocations(DataClasses1DataContext dbContext, Location lc)
        {

            var changeAddress = dbContext.Adress.FirstOrDefault(q => q.Id != lc.AdressId);
          
            Guid tempId = lc.AdressId;
            Guid? tempIA = lc.InvoiceAdressId;
            Guid? tempIDA = lc.InvoiceDispatchAdressId;
            if (changeAddress != null)
            {
                lc.AdressId = changeAddress.Id;
                lc.InvoiceAdressId = null;
                lc.InvoiceDispatchAdressId = null;
            }
            else
            {
                throw new Exception("Das löschen vom Standort " + lc.Name + "  ist aus Refernzierungsgründen nicht möglich, bitte legen Sie Datensatz an bei dem mind. eine neue Adresse erstellt wird");
            }
      


            var changeCustomer = dbContext.LargeCustomer.FirstOrDefault(q => q.CustomerId == lc.CustomerId && q.MainLocationId == lc.Id);
            
           // tempId = lc.CustomerId.Value;
         
            if (changeCustomer != null )
            {
                changeCustomer.Customer._dbContext = dbContext;
                lc._dbContext = dbContext;
                changeCustomer.Location1= null;
            }

            lc.LogDBContext = dbContext;
            //if (changeCustomer != null)
            //{
                //changeCustomer.LogDBContext = dbContext;
                lc.Name = lc.Name + "_" + DateTime.Now.Ticks;
                dbContext.SubmitChanges();
                //lc.CustomerId = changeCustomer.CustomerId;

                lc.CustomerId = null;
            //}
            //else
            //{
            //    throw new Exception("Das löschen vom Standort " + lc.Name + "  ist aus Refernzierungsgründen nicht möglich, bitte legen Sie Datensatz an bei dem mind. ein neuer Kunde erstellt wird");
            //}
            //Adress.DeleteAdress(tempId, dbContext);

            if (tempIA != null)
                Adress.DeleteAdress(tempIA.Value, dbContext);

            if (tempIDA != null)
                Adress.DeleteAdress(tempIDA.Value, dbContext);

            dbContext.SubmitChanges();

        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnValidate(System.Data.Linq.ChangeAction action)
        {
            if (action == System.Data.Linq.ChangeAction.Insert)
            {
                if (this.myDbContext == null)
                {
                    using (DataClasses1DataContext dbContext = new DataClasses1DataContext())
                    {
                        if (dbContext.Location.Any(q => q.CustomerId == this.CustomerId && q.Name == this.Name))
                        {
                            var customerName = dbContext.Customer.Single(q => q.Id == this.CustomerId).Name;
                            throw new Exception("Der Kunde " + customerName + " besitzt bereits einen Standort mit Namen " + this.Name + ".");
                        }
                    }
                }
                else
                {
                    if (myDbContext.Location.Any(q => q.CustomerId == this.CustomerId && q.Name == this.Name))
                    {
                        var customerName = myDbContext.Customer.Single(q => q.Id == this.CustomerId).Name;
                        throw new Exception("Der Kunde " + customerName + " besitzt bereits einen Standort mit Namen " + this.Name + ".");
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
        partial void OnNameChanging(string value)
        {
            this.WriteUpdateLogItem("Name", this.Name, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnSuperLocationIdChanging(Guid? value)
        {
            if (value.HasValue)
            {

                if (_dbContext != null)
                {
                    var superLocation = _dbContext.Location.SingleOrDefault(q => q.Id == value.Value);
                    this.LogDBContext.WriteLogItem("Standort " + this.Name + " wurde dem Standort " + superLocation.Name + " untergeordnet.", LogTypes.UPDATE, superLocation.Id, "Location", this.Id);
                }
                else
                {
                    using (DataClasses1DataContext dbContext = new DataClasses1DataContext())
                    {
                        var superLocation = dbContext.Location.SingleOrDefault(q => q.Id == value.Value);
                        this.LogDBContext.WriteLogItem("Standort " + this.Name + " wurde dem Standort " + superLocation.Name + " untergeordnet.", LogTypes.UPDATE, superLocation.Id, "Location", this.Id);
                    }
                }
            }
            else
            {
                this.LogDBContext.WriteLogItem("Standort " + this.Name + " wurde dem Standort " + this.Location1.Name + " entzogen.", LogTypes.UPDATE, this.Location1.Id, "Location", this.Id);
            }
        }
    }
}
