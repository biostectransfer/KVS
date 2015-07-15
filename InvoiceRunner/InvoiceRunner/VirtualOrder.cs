using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InvoiceRunner
{
    /// <summary>
    /// Hilfklasse fuer der Rechnungslauf
    /// </summary>
    class VirtualOrder
    {
        /// <summary>
        /// Auftragsobjektid
        /// </summary>
        public Guid OrderId { get; set; }
        /// <summary>
        /// AuftragspositionsID
        /// </summary>
        public Guid OrderItemId { get; set; }
        /// <summary>
        /// Standort
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// KostenstelleID
        /// </summary>
        public Guid? CostCenterId { get; set; }
        /// <summary>
        /// Kostenstellenname
        /// </summary>
        public string CostCenterName { get; set; }
        /// <summary>
        /// Betrag
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Anzahl der Items
        /// </summary>
        public int ItemCount { get; set; }
        /// <summary>
        /// Auftragsnummer
        /// </summary>
        public int OrderNumber { get; set; }
        /// <summary>
        /// Positionsname
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// Auftragspositionsstatus
        /// </summary>
        public string ItemStatus { get; set; }
        /// <summary>
        /// Erstellungsdatum
        /// </summary>
        public DateTime? ExecutionDate { get; set; }
        /// <summary>
        /// Auftragsstandort
        /// </summary>
        public Guid? OrderLocation { get; set; }
        /// <summary>
        /// Auftragsdatum
        /// </summary>
        public DateTime? OrderDate { get; set; }
        /// <summary>
        /// Kundenid
        /// </summary>
        public Guid CustomerId { get; set; }
        /// <summary>
        /// Rechnungstypid
        /// </summary>
        public Guid? IvoiceTypeId { get; set; }
        /// <summary>
        /// Standortname
        /// </summary>
        public string LocationName { get; set; }
        /// <summary>
        /// StandortID
        /// </summary>
        public Guid? LocationId { get; set; }
        /// <summary>
        /// Id des aktuellen Reports
        /// </summary>
        public Guid? ReportId { get; set; }
        /// <summary>
        /// Auftragsobjekt
        /// </summary>
        public KVSCommon.Database.Order Order { get; set; }
    }
}
