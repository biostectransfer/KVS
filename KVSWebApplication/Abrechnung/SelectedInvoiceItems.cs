using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KVSWebApplication.Abrechnung
{
    public class SelectedInvoiceItems 
    {
        private string productName;

        public string ProductName
        {
            get { return productName; }
            set { productName = value; }
        }
        private string amount;
        public string Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        private int orderItemId;
        public int OrderItemId
        {
            get { return orderItemId; }
            set { orderItemId = value; }
        }
     
        private int? costCenterId;
        public int? CostCenterId
        {
            get { return costCenterId; }
            set { costCenterId = value; }
        }
        private int orderNumber;
        public int OrderNumber
        {
            get { return orderNumber; }
            set { orderNumber = value; }
        }
        private int itemCount;
        public int ItemCount
        {
            get { return itemCount; }
            set { itemCount = value; }
        }
        private int? orderLocationId;
        public int? OrderLocationId
        {
            get { return orderLocationId; }
            set { orderLocationId = value; }
        }
        private string orderLocationName;
        public string OrderLocationName
        {
            get { return orderLocationName; }
            set { orderLocationName = value; }
        }
    }
}