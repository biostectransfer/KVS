using KVSCommon.Database;
using KVSCommon.Managers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Managers
{
    public interface IOrderManager : IEntityManager<Order, int>
    {
        /// <summary>
        /// Fügt dem Auftrag eine neue Position hinzu.
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="productId">Id des Produkts.</param>
        /// <param name="priceAmount">Preis für die Position.</param>
        /// <param name="count">Anzahl für die Position.</param>
        /// <param name="costCenterId">Id der Kostenstelle, falls benötigt.</param>
        /// <param name="superOrderItemId">Id der übergeordneten Auftragsposition, falls benoetigt.</param>
        /// <param name="isAuthorativeCharge">Gibt an, ob es sich um eine behoerdliche Gebühr handelt oder nicht.</param>
        /// <returns>Die neue Auftragsposition.</returns>
        OrderItem AddOrderItem(Order order, int productId, decimal priceAmount, int count, CostCenter costCenter, int? superOrderItemId, bool isAuthorativeCharge);

        /// <summary>
        /// Update order item amount
        /// </summary>
        /// <param name="orderItemId"></param>
        /// <param name="amount"></param>
        void UpdateOrderItemAmount(int orderItemId, decimal amount);

        /// <summary>
        /// Erstellt die amtlichen Gebuehren
        /// </summary>
        /// <param name="authId">Amtliche Gebuehr ID</param>
        /// <param name="itemId">Auftragspositionen Id</param>
        /// <param name="amount">Betrag</param>
        /// <returns>bool</returns>
        bool GenerateAuthCharge(int? authId, int itemId, string amount);

        /// <summary>
        /// Get order items
        /// </summary>
        /// <returns></returns>
        IQueryable<OrderItem> GetOrderItems();

        /// <summary>
        /// Get order items
        /// </summary>
        /// <param name="orderNumber">Order number</param>
        /// <returns></returns>
        IQueryable<OrderItem> GetOrderItems(int orderNumber);

        /// <summary>
        /// Loescht eine Auftragsposition und ggf. die Amtlichen Gebuehren dazu
        /// </summary>
        /// <param name="orderItemId">AuftragspositionID</param>
        void RemoveOrderItem(int orderItemId);

        /// <summary>
        /// Sendet eine Benachrichtigungsemail über den Abschluss des angegebenen Auftrag.
        /// </summary>
        /// <param name="order">Der Auftrag.</param>
        /// <param name="fromEmailAddress">Absenderemailadresse.</param>
        /// <param name="smtpServer">SMTP-Server für den Emailversand.</param>
        /// <remarks>Die Methode ruft dbContext.SubmitChanges() auf, um den Status der Versendung zu speichern.</remarks>
        void SendOrderFinishedNote(Order order, string fromEmailAddress, string smtpServer);

        /// <summary>
        /// Entfernt die LieferscheinId und versetzt den Auftag in den Status Zulassungsstelle
        ///</summary>
        ///<param name="packingListNumber">Lieferschein ID</param>
        void TryToRemovePackingListIdAndSetStateToRegistration(int packingListNumber);
    }
}
