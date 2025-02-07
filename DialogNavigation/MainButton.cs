//-----------------------------------------------------------------------
// <copyright file="MainRibbonButton.cs" company="NRM Netzdienste Rhein-Main GmbH">
//     Class: MainRibbonButton
//     Copyright � NRM Netzdienste Rhein-Main GmbH 2023
// </copyright>
//
// <author>DeveloperName - NRM Netzdienste Rhein-Main GmbH</author>
// <email>DeveloperName@nrm-netzdienste.de</email>
// <date>02.07.2024</date>
//
// <summary>
// Enum Klasse f�r 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Core
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public enum MainButton : int
    {
        [Description("Keine Auswahl")]
        None = 0,
        [Description("Home")]
        Home = 1,
        [Description("Login")]
        Login = 2,
        [Description("Logoff")]
        Logoff = 3,
        [Description("Passwort �ndern")]
        ChangePassword = 4,
        [Description("Einstellungen")]
        AppSettings = 5,
        [Description("�ber ...")]
        About = 6,
        [Description("Zur�ck")]
        Back = 7,
    }
}
