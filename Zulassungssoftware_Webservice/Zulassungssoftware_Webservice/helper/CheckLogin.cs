using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security;
using KVSCommon.Database;
using System.Runtime.InteropServices;
using System.Net;
using System.Configuration;

namespace Zulassungssoftware_Webservice
{
    public  class CheckLogin 
    {
        public static List<Guid> CheckUser(string username, string password, string InternalId)
        {

            string UserNameToBe = ConfigurationManager.AppSettings["Username"];
            string PasswordToBe = ConfigurationManager.AppSettings["Password"];
            List<Guid> customers = new List<Guid>();

            if (!String.IsNullOrEmpty(username) || !String.IsNullOrEmpty(password) || !String.IsNullOrEmpty(InternalId))
            {
                if (UserNameToBe == username || PasswordToBe == password)
                {
                    getCustomers(out customers, InternalId);
                }
                else
                {
                    throw new Exception("Username oder Passwort sind falsch!");
                }
            }
            else
            {
                throw new Exception("Username/Passwort/InternalID sind leer!");
            }

            return customers;
        }

        static void getCustomers(out List<Guid> customers, string internalId)
        {
            customers = new List<Guid>();

            using (DataClasses1DataContext dbContext = new DataClasses1DataContext())
            {

                var queryCustomer = from cust in dbContext.Customer
                                    where cust.InternalId == new Guid(internalId)
                                    select new { cust.Id };

                foreach (var custId in queryCustomer)
                {
                    customers.Add(custId.Id);
                }
            }
        }

        static void testCustomers(out List<Guid> customers)
        {
            customers = new List<Guid>();
      
            using (DataClasses1DataContext dbContext = new DataClasses1DataContext())
            {
              
                var queryCustomer = from cust in dbContext.Customer
                                    select new { cust.Id };
             
                foreach (var custId in queryCustomer)
                {
                    customers.Add(custId.Id);
                }
            }
        }

        //static String SecureStringToString(SecureString value)
        //{
        //    IntPtr bstr = Marshal.SecureStringToBSTR(value);

        //    try
        //    {
        //        return Marshal.PtrToStringBSTR(bstr);
        //    }
        //    finally
        //    {
        //        Marshal.FreeBSTR(bstr);
        //    }
        //}


       static String SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

    }
}