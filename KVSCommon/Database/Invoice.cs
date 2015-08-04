using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Database
{
    using System.IO;
    using PdfSharp.Pdf;
    using PdfSharp.Drawing;
    using System.Net.Mail;
    using System.Configuration;
    using KVSCommon.Enums;
    using Entities;

    /// <summary>
    /// Erweiterungsklasse für die Tabelle Invoice
    /// </summary>
    public partial class Invoice : ILogging, IHasId<int>, IRemovable, ISystemFields
    {
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
        /// Nettobetrag
        /// </summary>
        public decimal NetSum
        {
            get
            {

                var query = this.InvoiceItem.Where(q => q.OrderItem == null || q.OrderItem.IsAuthorativeCharge == false).Where(q => q.VAT > 0);
                decimal? discount = null;
                if (this.InvoiceItem.FirstOrDefault() != null)
                {
                    discount = this.InvoiceItem.FirstOrDefault().Invoice.discount;
                }
                if (query.Count() > 0)
                {
                    if (discount.HasValue && discount.Value != 0)
                    {
                        return query.Sum(q => q.Amount * q.Count) * ((100 - discount.Value) / 100);
                    }
                    else
                    {
                        return query.Sum(q => q.Amount * q.Count);
                    }
                }

                return 0;
            }
        }
        /// <summary>
        /// MwSt
        /// </summary>
        public decimal TaxValue
        {
            get
            {
                var query = this.InvoiceItem.Where(q => q.OrderItem == null || q.OrderItem.IsAuthorativeCharge == false).Where(q => q.VAT > 0);
                decimal? discount = null;
                if (this.InvoiceItem.FirstOrDefault() != null)
                {
                    discount = this.InvoiceItem.FirstOrDefault().Invoice.discount;
                }
                if (query.Count() > 0)
                {
                    if (discount.HasValue && discount.Value != 0)
                    {

                        return query.Sum(q => (q.Amount * q.Count) * ((100 - discount.Value) / 100) * q.VAT / 100);
                    }
                    else
                    {
                        return query.Sum(q => q.Amount * q.Count * q.VAT / 100);

                    }
                }

                return 0;
            }
        }
        /// <summary>
        /// Gruppe der Steuerfreien Rechnungspositionen
        /// </summary>
        public List<KeyValuePair<string, decimal>> TaxFreeItemsGrouped
        {
            get
            {
                var query = this.InvoiceItem.Where(q => q.OrderItem == null || q.OrderItem.IsAuthorativeCharge == false).Where(q => q.VAT == 0);
                if (query.Count() > 0)
                {
                    return query.GroupBy(q => q.Name).Select(q => new KeyValuePair<string, decimal>(q.Key, q.Sum(p => p.Amount * p.Count))).ToList();
                }

                return new List<KeyValuePair<string, decimal>>();
            }
        }
        /// <summary>
        /// Summe der amtlichen Gebuehren
        /// </summary>
        public decimal AuthorativeChargeSum
        {
            get
            {
                var query = this.InvoiceItem.Where(q => q.OrderItem != null && q.OrderItem.IsAuthorativeCharge);
                if (query.Count() > 0)
                {
                    return query.Sum(q => q.Amount * q.Count);
                }

                return 0;
            }
        }
        /// <summary>
        /// Endbetrag
        /// </summary>
        public decimal GrandTotal
        {
            get
            {
                return this.NetSum + this.TaxValue + this.AuthorativeChargeSum + this.TaxFreeItemsGrouped.Sum(q => q.Value);
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
        partial void OnCreateDateChanging(DateTime value)
        {
            if (this.EntityState != EntityState.New)
            {
                throw new Exception("Das Rechnungserstelldatum darf nicht geändert werden.");
            }
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnInvoiceRecipientChanging(string value)
        {
            this.WriteUpdateLogItem("Rechnungsempfänger", this.InvoiceRecipient, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnIsPrintedChanging(bool value)
        {
            this.WriteUpdateLogItem("IstGedruckt", this.IsPrinted, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnPrintDateChanging(DateTime? value)
        {
            this.WriteUpdateLogItem("Druckdatum", this.PrintDate, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnCustomerIdChanging(int value)
        {
            if (this.EntityState != Database.EntityState.New)
            {
                throw new Exception("Der Kunde einer Rechnung kann nicht geändert werden.");
            }
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnInvoiceTextChanging(string value)
        {
            this.WriteUpdateLogItem("Rechnungstext", this.InvoiceText, value);
        }
        /// <summary>
        /// Aenderungsevents für die Historie
        /// </summary>
        /// <param name="value"></param>
        partial void OnInvoiceTypeChanging(int? value)
        {
            this.WriteUpdateLogItem("Rechnungstype", this.InvoiceType, value);
        }
    }
}
