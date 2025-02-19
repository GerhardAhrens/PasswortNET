//-----------------------------------------------------------------------
// <copyright file="ExportImportFormat.cs" company="Lifeprojects.de">
//     Class: ExportImportFormat
//     Copyright © Lifeprojects.de 2025
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>Gerhard Ahrens@Lifeprojects.de</email>
// <date>19.02.2025 14:26:07</date>
//
// <summary>
// Enum Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Core.Enums
{
    using System;
    using System.ComponentModel;

    public enum ExportImportFormat : int
    {
        [Description("Keine Auswahl")]
        None = 0,
        [Description("Exceldatei (ab 2007)")]
        XLSX = 1,
        [Description("XML Datei")]
        XML = 2
    }
}
