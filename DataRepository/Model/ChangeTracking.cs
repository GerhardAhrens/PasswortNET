//-----------------------------------------------------------------------
// <copyright file="ChangeTracking.cs" company="Lifeprojects.de">
//     Class: ChangeTracking
//     Copyright © Lifeprojects.de 2021
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>12.04.2021</date>
//
// <summary>
// Modelklasse für Tabelle 'SYS_ChangeTracking' mit 10 Columns
// (generiert aus T4-Template)
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Model
{
    using System;

    using ModernBaseLibrary.Core;

    /// <summary>
    /// Model Class mit Properties zur Tabelle E_SYS_ChangeTracking (generiert aus T4-Template)
    /// </summary>
    public sealed partial class ChangeTracking
    {
        public ChangeTracking()
        {
            this.CreatedBy = UserInfo.TS().CurrentUser;
            this.CreatedOn = UserInfo.TS().CurrentTime;
        }

        public Guid Id { get; set; }

        public string ActivityTyp { get; set; }

        public string ObjectId { get; set; }

        public string ObjectName { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}