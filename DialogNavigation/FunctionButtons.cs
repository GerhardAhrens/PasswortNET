//-----------------------------------------------------------------------
// <copyright file="MainRibbonButton.cs" company="NRM Netzdienste Rhein-Main GmbH">
//     Class: MainRibbonButton
//     Copyright © NRM Netzdienste Rhein-Main GmbH 2023
// </copyright>
//
// <author>DeveloperName - NRM Netzdienste Rhein-Main GmbH</author>
// <email>DeveloperName@nrm-netzdienste.de</email>
// <date>02.07.2024</date>
//
// <summary>
// Enum Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Core
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public enum FunctionButtons : int
    {
        [Description("Keine Auswahl")]
        None = 0,
        [Description("Home Dialog ohne Funktion")]
        Home = 1,
        [Description("Anmeldung durchführen, um PasswortNET verwenden zu können.")]
        Login = 2,
        [Description("Aktueller Benutzer abmelden")]
        Logoff = 3,
        [Description("Benutzer und/oder Passwort ändern")]
        ChangePassword = 4,
        [Description("Programmeinstellungen, Passwortdatenbank, Tag")]
        AppSettings = 5,
        [Description("Programminformationen und Statistik")]
        About = 6,
        [Description("Zurück")]
        Back = 7,
        [Description("Übersicht Passwort/Zugänge/Pin/Lizenzen")]
        MainOverview = 8,
        [Description("Bearbeiten Passwort/Zugänge/Pin/Lizenzen")]
        PasswordDetail = 9,
        [Description("Datenbank synchronisieren")]
        DataSync = 10,
        [Description("Export Passwortdaten")]
        Export = 11,
        [Description("Drucken Passwortdaten")]
        Print = 12,
        [Description("Hilfe")]
        Help = 13,
        [Description("Änderungsverfolgung")]
        AuditTrail = 14,
    }
}
