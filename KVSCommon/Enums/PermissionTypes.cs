using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KVSCommon.Enums
{
    /// <summary>
    /// Berechtigungen
    /// </summary>
    public enum PermissionTypes
    {
        /// <summary>
        /// Erlaubt die Standortanlage 
        /// </summary>
        STANDORT_ANLAGE = 1,

        /// <summary>
        /// Erlaubt das Sperren von Produkten. 
        /// </summary>
        PRODUKTE_SPERREN = 2,

        /// <summary>
        /// Erlaubt das bearbeiten der Emailadressen 
        /// </summary>
        EMAIL_BEARBEITEN = 3,

        /// <summary>
        /// Erlaubt die Anlage der Emailadressen 
        /// </summary>
        EMAIL_ANLEGEN = 4,

        /// <summary>
        /// Erlaubt das Bearbeiten von Zulassungsaufträgen. 
        /// </summary>
        ZULASSUNGSAUFTRAG_BEARBEITEN = 5,

        /// <summary>
        /// Erlaubt das Anlegen von Kunden. 
        /// </summary>
        KUNDEN_ANLEGEN = 6,

        /// <summary>
        /// Erlaubt die Kundenübersicht 
        /// </summary>
        KUNDEN_UEBERSICHT = 7,

        /// <summary>
        /// Erlaubt die Kostenstellenanlage 
        /// </summary>
        KOSTENSTELLEN_ANLEGEN = 8,

        /// <summary>
        /// Erlaubt das exportieren von Rechnungsdaten 
        /// </summary>
        RECHNUNG_EXPORT = 9,

        /// <summary>
        /// Erlaubt das Bearbeiten von Rechnungen / Rechnungspositionen. 
        /// </summary>
        RECHNUNG_BEARBEITEN = 10,

        /// <summary>
        /// Erlaubt das Anlegen von Zulasssungsaufträgen. 
        /// </summary>
        ZULASSUNGSAUFTRAG_ANLEGEN = 11,

        /// <summary>
        /// Erlaubt die Übersicht der Emailadressen 
        /// </summary>
        EMAIL_UEBERSICHT = 12,

        /// <summary>
        /// Erlaubt das einsehen der Änderugshistorie 
        /// </summary>
        CHANGELOG = 13,

        /// <summary>
        /// Erlaubt die Produktübersicht 
        /// </summary>
        PRODUKTE_UEBERSICHT = 14,

        /// <summary>
        /// Erlaubt das Anlegen von Produkten. 
        /// </summary>
        PRODUKTE_ANLEGEN = 15,

        /// <summary>
        /// Erlaubt die Bearbeitung der Kostenstellen 
        /// </summary>
        KOSTENSTELLEN_BEARBEITEN = 16,

        /// <summary>
        /// Ermöglicht das löschen der Auftragspositionen, solange sie im Reiter offen sind 
        /// </summary>
        LOESCHEN_AUFTRAGSPOSITION = 17,

        /// <summary>
        /// Erlaubt das Bearbeiten von Preisen. 
        /// </summary>
        PREISE_BEARBEITEN = 18,

        /// <summary>
        /// Erlaubt das Anlegen von Abmeldeaufträgen. 
        /// </summary>
        ABMELDEAUFTRAG_ANLEGEN = 19,

        /// <summary>
        /// Erlaubt das Bearbeiten von Produkten. 
        /// </summary>
        PRODUKTE_BEARBEITEN = 20,

        /// <summary>
        /// Erlaubt das erstellen eines Rechnungslaufs 
        /// </summary>
        RECHNUNGSLAUF = 21,

        /// <summary>
        /// Erlaubt die Kostenstellenübersicht 
        /// </summary>
        KOSTENSTELLEN_ANSICHT = 22,

        /// <summary>
        /// Erlaubt das Anlegen / Bearbeiten / Löschen von Rechteprofilen. 
        /// </summary>
        RECHTEPROFIL_BEARBEITEN = 23,

        /// <summary>
        /// Erlaubt das Bearbeiten von Kunden. 
        /// </summary>
        KUNDEN_BEARBEITEN = 24,

        /// <summary>
        /// Administratorrecht. Erlaubt es dem Benutzer, die Passwörter von anderen Benutzern zu ändern. 
        /// </summary>
        ADMIN_PASSWORT_AENDERN = 25,

        /// <summary>
        /// Erlaubt das Bearbeiten von Benutzern. 
        /// </summary>
        BENUTZER_BEARBEITEN = 26,

        /// <summary>
        /// Erlaubt das Anlegen von Benutzern. 
        /// </summary>
        BENUTZER_ANLEGEN = 27,

        /// <summary>
        /// Erlaubt das löschen der Emailadressen 
        /// </summary>
        EMAIL_LOESCHEN = 28,

        /// <summary>
        /// Erlaubt das Bearbeiten von allgemeinen Aufträgen. 
        /// </summary>
        AUFRAG_BEARBEITEN = 29,

        /// <summary>
        /// Erlaubt die Standortbearbeitung 
        /// </summary>
        STANDORT_BEARBEITEN = 30,

        /// <summary>
        /// Erlaubt die Rechteübersicht 
        /// </summary>
        RECHTEPROFIL_UEBERSICHT = 31,

        /// <summary>
        /// Erlaubt das Anlegen / Bearbeiten von Fahrzeugherstellern und -modellen. 
        /// </summary>
        HERSTELLER_MODELLE_BEARBEITEN = 32,

        /// <summary>
        /// Erlaubt das Erstellen von Rechnungen. 
        /// </summary>
        RECHNUNG_ERSTELLEN = 33,

        /// <summary>
        /// Erlaubt das importieren von Kundendaten 
        /// </summary>
        KUNDEN_IMPORT = 34,

        /// <summary>
        /// Erlaubt das Sperren von Benutzern. 
        /// </summary>
        BENUTZER_SPERREN = 35,

        /// <summary>
        /// Erlaubt das Anlegen von allgemeinen Aufträgen. 
        /// </summary>
        AUFTRAG_ANLEGEN = 36,

        /// <summary>
        /// Erlaubt die Standortübersicht 
        /// </summary>
        STANDORT_UEBERSICHT = 37,

        /// <summary>
        /// Erlaubt das Bearbeiten von Abmeldeaufträgen. 
        /// </summary>
        ABMELDEAUFTRAG_BEARBEITEN = 38,

        /// <summary>
        /// Erlaubt das löschen (Kunden, Standorte, Produkte, Kostenstellen) 
        /// </summary>
        LOESCHEN = 39,	   
    }
}
