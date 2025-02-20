//-----------------------------------------------------------------------
// <copyright file="AccessTyp.cs" company="Lifeprojects.de">
//     Class: AccessTyp
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>24.04.2022</date>
//
// <summary>
// Enum Klasse für (Passwort Art) AccessTyp
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Core
{
    using System;
    using System.ComponentModel;

    public enum AccessTyp : int
    {
        [Description("Keine Auswahl")]
        None = 0,
        [Description("Pin")]
        Pin = 1,
        [Description("Webseitenzugang")]
        Website = 2,
        [Description("Passwort")]
        Passwort = 3,
        [Description("Softwarelizenz")]
        License = 4,
        [Description("Unspezifisch")]
        All = 99
    }
}
