//-----------------------------------------------------------------------
// <copyright file="ActionAuditTrail.cs" company="Lifeprojects.de">
//     Class: ActionAuditTrail
//     Copyright © Lifeprojects.de 2021
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>12.04.2021</date>
//
// <summary>
// Enum Class for ActionAuditTrail (Audit Trail Function)
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.AuditTrail
{
    using System.ComponentModel;

    public enum ActionAuditTrail : int
    {
        [Description("Unbekannt")]
        None = 0,
        [Description("New Object")]
        New = 1,
        [Description("Edit Object")]
        Change = 2,
        [Description("Delete Object")]
        Delete = 3,
    }
}
