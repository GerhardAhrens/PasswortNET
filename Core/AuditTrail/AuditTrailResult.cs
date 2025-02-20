//-----------------------------------------------------------------------
// <copyright file="AuditTrailResult.cs" company="Lifeprojects.de">
//     Class: AuditTrailResult
//     Copyright © Lifeprojects.de 2021
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>22.01.2021</date>
//
// <summary>
// Die Klasse dient als Result Container für einen AuditTrail Eintrag
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.AuditTrail
{
    using System.Diagnostics;

    using PasswortNET.Model;

    [DebuggerDisplay("Activity={this.Activity}")]
    public class AuditTrailResult
    {
        public int TrackChanges { get; set; }

        public ActionAuditTrail Activity { get; set; }

        public string SqlStatement { get; set; }

        public ChangeTracking TrackingObject { get; set; }
    }
}
