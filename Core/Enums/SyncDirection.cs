//-----------------------------------------------------------------------
// <copyright file="SyncDirection.cs" company="Lifeprojects.de">
//     Class: SyncDirection
//     Copyright © Lifeprojects.de 2025
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>24.02.2025 09:46:19</date>
//
// <summary>
// Enum Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Core.Enums
{
    using System;
    using System.ComponentModel;

    public enum SyncDirection : int
    {
        [Description("Keine Auswahl")]
        None = 0,
        [Description("Daten zum synchroniseren exportieren")]
        SyncExport = 1,
        [Description("Daten zum synchroniseren importieren")]
        SyncImport = 2,
    }
}
