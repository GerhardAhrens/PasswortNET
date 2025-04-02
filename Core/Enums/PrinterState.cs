//-------------------------------------------
// <copyright file="PrinterState.cs" company="Lifeprojects.de">
//     Class: PrinterState
//     Copyright © Lifeprojects.de 2017
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>08.11.2017</date>
//
// <summary>
// Enum für Druckerstatus
// </summary>
//----------------------------

namespace PasswortNET.Core.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum PrinterState
    {
        None = 0,
        Other = 1,
        Unknown = 2,
        Idle = 3,
        Print = 4,
        Preheating = 5,
        StopPrinting = 6,
        Offline = 7,
        Error = 9,
        NotAvailable = 11
    }
}
