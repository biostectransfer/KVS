using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using KVSCommon.Database;
using System.Configuration;

namespace Importer
{
    class Program
    {

       

        static void Main(string[] args)
        {

            if (ConfigurationManager.AppSettings["CreateCategory"] == "1")
           CreateCategorys();

            if (ConfigurationManager.AppSettings["CreateProducts"] == "1")
            CreateProducts();

            if (ConfigurationManager.AppSettings["CreateCustomers"] == "1")
            CreateCustomers();

        }
        public  static void CreateCategorys()
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext(new Guid("E6232890-B6E9-4AA4-B68C-36CF5C9A0918"));
            var allCategories = from cat in dbContext.temp_Products
                                group cat by cat.Warengruppe.Trim() into g
                                select new { gruppe = g.Key };
            foreach(var allc in allCategories)
            {
                if (dbContext.ProductCategory.FirstOrDefault(s => s.Name.Trim() == allc.gruppe.Trim()) == null)
                {
                    ProductCategory pr = new ProductCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = allc.gruppe
                    };
                    dbContext.ProductCategory.InsertOnSubmit(pr);
                    dbContext.SubmitChanges();
                }
            }
        }
        public static void CreateCustomers()
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext(new Guid("E6232890-B6E9-4AA4-B68C-36CF5C9A0918"));
            var allCustomers = from cat in dbContext.temp_Customers2
                                
                                select new { cat };
            foreach (var allc in allCustomers)
            {

                var existsCustomer = dbContext.Customer.FirstOrDefault(q => q.CustomerNumber == allc.cat.Kundennummer);
                if (existsCustomer != null)
                {

                    existsCustomer.LogDBContext = dbContext;
                    existsCustomer.Name = allc.cat.Firma;
                    existsCustomer.Adress.Street = allc.cat.Strasse;
                    existsCustomer.Adress.StreetNumber = allc.cat.Hausnummer;
                    existsCustomer.Adress.Zipcode = allc.cat.Zip;
                    existsCustomer.Adress.City = allc.cat.Ort;
                    existsCustomer.Adress.Country = allc.cat.Land;
                    existsCustomer.Contact.LogDBContext = dbContext;
                    existsCustomer.Contact.Phone = allc.cat.Telefon;
                    existsCustomer.Contact.Fax = allc.cat.Telefax;
                    existsCustomer.Contact.Email = allc.cat.EMAIL;
                    existsCustomer.MatchCode = allc.cat.Matchcode;
                    existsCustomer.Debitornumber = allc.cat.Debitorenkonto;
                    Guid? myPersonId = null;
                    if ((allc.cat.Name != string.Empty || allc.cat.Vorname != string.Empty || allc.cat.Zusatz != string.Empty) && existsCustomer.LargeCustomer.PersonId == null)
                    {
                        myPersonId = Person.CreatePerson(dbContext, allc.cat.Vorname, allc.cat.Name, allc.cat.Anrede, allc.cat.Zusatz).Id;
                    }
                    else if ((allc.cat.Name != string.Empty || allc.cat.Vorname != string.Empty || allc.cat.Zusatz != string.Empty) && existsCustomer.LargeCustomer.PersonId != null)
                    {
                        existsCustomer.LargeCustomer.Person.LogDBContext= dbContext;
                        existsCustomer.LargeCustomer.Person.Title = allc.cat.Anrede;
                        existsCustomer.LargeCustomer.Person.Name = allc.cat.Name;
                        existsCustomer.LargeCustomer.Person.FirstName = allc.cat.Vorname;
                        existsCustomer.LargeCustomer.Person.Extension = allc.cat.Zusatz;
                    }
                    dbContext.SubmitChanges();

                }
                else
                {
                    Guid? myPersonId = null;
                    if (allc.cat.Name != string.Empty || allc.cat.Vorname != string.Empty || allc.cat.Zusatz != string.Empty)
                    {
                        myPersonId = Person.CreatePerson(dbContext, allc.cat.Vorname, allc.cat.Name, allc.cat.Anrede, allc.cat.Zusatz).Id;
                    }

                    LargeCustomer.CreateLargeCustomer(dbContext, allc.cat.Firma, allc.cat.Strasse, allc.cat.Hausnummer, allc.cat.Zip, allc.cat.Ort, allc.cat.Land, allc.cat.Telefon, allc.cat.Telefax, "", allc.cat.EMAIL, 0, true, false, null, 
                        allc.cat.Kundennummer,
                        allc.cat.Matchcode, allc.cat.Debitorenkonto, myPersonId, new Guid("DB455FDA-46E8-432E-9251-662F91258F6B"));
                }
                //LargeCustomer.CreateLargeCustomer(dbContext, allc.cat.Name, allc.cat.Strasse, allc.cat.Hausnummer, allc.cat.Zip, allc.cat.Ort, allc.cat.Ort, allc.cat.Telefon, allc.cat.Telefax, "", allc.cat.EMAIL, 0, true, false, null, allc.cat.Kundennummer, allc.cat.Matchcode, allc.cat.Debitorenkonto);
            }
        }
        //Artikelnummer 1000 bis 3999 sind MWST-pflichtige Erlöse.
        //Artikelnummer 4000 bis 5999  sind amtliche Gebühren ohne MWSt
        //Artikelnummer 6000 bis 8000 sind Versicherungskarten ohne MWSt
        public static void CreateProducts()
        {
            DataClasses1DataContext dbContext = new DataClasses1DataContext(new Guid("E6232890-B6E9-4AA4-B68C-36CF5C9A0918"));
            var allProducts = from cat in dbContext.temp_Products
                                select new { cat};
            decimal? autCharge;
            decimal? amount ;
            bool needsVat ;
            foreach (var allp in allProducts)
            {
                autCharge = null;
                amount = null;
                needsVat = false;
                var ProductCategory = dbContext.ProductCategory.FirstOrDefault(q => q.Name == allp.cat.Warengruppe.Trim());
                if (ProductCategory != null)
                {
                    if (int.Parse(allp.cat.Artikelnummer) > 4000 && int.Parse(allp.cat.Artikelnummer) < 5999)
                    {
                        autCharge = decimal.Parse(allp.cat.Preis);
                        amount = null;
                    }
                    else
                    {
                        amount = decimal.Parse(allp.cat.Preis);

                    }
                    if(int.Parse(allp.cat.Artikelnummer) >1000 && int.Parse(allp.cat.Artikelnummer) <3999)
                    {
                       needsVat = true;
                    }
                    var getProduct = dbContext.Product.FirstOrDefault(q =>q.Name == allp.cat.Bezeichnung && q.ItemNumber == allp.cat.Artikelnummer);
                 
                    var itemType = dbContext.OrderType.FirstOrDefault(q => q.Name == allp.cat.Type);

                    if (getProduct == null)
                    {
                        Price price = null;
                        if (itemType != null)
                        {

                            price = Product.CreateProduct(allp.cat.Bezeichnung, ProductCategory.Id, decimal.Parse(allp.cat.Preis.Replace(',', '.')),
                                autCharge, allp.cat.Artikelnummer,
                                itemType.Id, null, needsVat, true, dbContext);
                        }
                        else
                        {
                            var regType = dbContext.RegistrationOrderType.FirstOrDefault(q => q.Name == allp.cat.Type);
                            price = Product.CreateProduct(allp.cat.Bezeichnung, ProductCategory.Id, decimal.Parse(allp.cat.Preis.Replace(',', '.')),
                             autCharge, allp.cat.Artikelnummer,
                              new Guid("C7D1B831-ADF5-4A36-AD2A-E70B2590E755"), regType.Id, needsVat, true, dbContext);

                        }

                        CreateAccount(allp.cat.KontoInland, price.Id, dbContext);

                       // Console.WriteLine("Produkt aktualisert: " + allp.cat.Bezeichnung);

                       //dbContext.SubmitChanges();
                    }
                    else
                    {

                        getProduct.LogDBContext = dbContext;



                        var tempPrice = dbContext.Price.FirstOrDefault(s => s.Product == getProduct  && s.Location == null);
                        getProduct.Name = allp.cat.Bezeichnung;
                        getProduct.ProductCategoryId = ProductCategory.Id;
                        if (itemType == null)
                        {
                            
                         var   orderType = dbContext.OrderType.FirstOrDefault(q => q.Name == allp.cat.Type);
                         getProduct.OrderTypeId = orderType.Id;
                            getProduct.RegistrationOrderTypeId = null;
                        }
                        else
                        {
                            getProduct.OrderTypeId =  new Guid("C7D1B831-ADF5-4A36-AD2A-E70B2590E755");
                            getProduct.RegistrationOrderTypeId = itemType.Id;


                        }
                        Console.WriteLine("Produkt aktualisert: " + getProduct.Id);
                        if(tempPrice!=null)
                         tempPrice.Amount = decimal.Parse(allp.cat.Preis.Replace(',', '.'));

                        getProduct.NeedsVAT = needsVat;
                        //getProduct.ItemNumber = allp.cat.Artikelnummer;
                        CreateAccount(allp.cat.KontoInland, tempPrice.Id, dbContext);

                     

                       dbContext.SubmitChanges();


                    }
                   
                }
          
               
            }
            Console.ReadLine();
        }

        public static void CreateAccount(string accountNumber, Guid PriceId, DataClasses1DataContext dbContext)
        {
            var account = dbContext.Accounts.FirstOrDefault(s => s.AccountNumber == accountNumber && s.CustomerId == null);
                Accounts acc = null;
                if (account == null)
                {
                    acc = Accounts.CreateAccount(null, accountNumber, dbContext);
                  

                }

        
                if (dbContext.PriceAccount.FirstOrDefault(q => q.PriceId == PriceId && q.AccountId == ((account != null) ? account.AccountId : acc.AccountId)) == null)
                {
                    var myNewAccount = new PriceAccount
                    {
                        PriceId = PriceId,
                        AccountId = ((account != null) ? account.AccountId : acc.AccountId)
                    };

                    dbContext.PriceAccount.InsertOnSubmit(myNewAccount);
                    dbContext.SubmitChanges();
          
                }

            
            
        }

    }
}
