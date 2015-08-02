using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KVSWebApplication.BasePages
{
    /// <summary>
    /// View model for orders
    /// </summary>
    public class OrderViewModel
    {
        public int OrderNumber { get; set; }
        public int customerId { get; set; }
        public int locationId { get; set; }
        public DateTime CreateDate { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public string CustomerLocation { get; set; }
        public string Kennzeichen { get; set; }
        public string VIN { get; set; }
        public string TSN { get; set; }
        public string HSN { get; set; }
        public string OrderTyp { get; set; }
        public string Freitext { get; set; }
        public string Geprueft { get; set; }
        public DateTime? Datum { get; set; }
        public bool? HasError { get; set; }
        public string ErrorReason { get; set; }

        #region Small Customer Fields

        public DateTime? Inspection { get; set; }
        public string Variant { get; set; }
        public string eVBNum { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string BankName { get; set; }
        public string AccountNum { get; set; }
        public string Prevkennzeichen { get; set; }
        public string BankCode { get; set; }
        public string Street { get; set; }
        public string StreetNr { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }

        public int OrderTypeId { get; set; }
        public int OrderStatusId { get; set; }

        public bool? ReadyToSend { get; set; }

        #endregion
    }
}
