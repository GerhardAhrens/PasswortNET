//-----------------------------------------------------------------------
// <copyright file="ChangeTrackingPart.cs" company="Lifeprojects.de">
//     Class: ChangeTracking
//     Copyright © Lifeprojects.de 2021
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>12.04.2021</date>
//
// <summary>
// Modelklasse für Tabelle 'E_SYS_ChangeTracking' mit 10 Columns
// (generiert aus T4-Template)
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Model
{
    using ModernBaseLibrary.Core;

    /// <summary>
    /// Model Class mit zusätzlichen Properties zum ChangeTracking.
    /// Hier werden nur Get-Methoden geschrieben
    /// </summary>
    public sealed partial class ChangeTracking 
    {
        public string FullName
        {
            get
            {
                return $"{this.ObjectId}-{this.ObjectName} [{this.ActivityTyp}]";
            }
        }

        public string Timestamp
        {
            get
            {
                string result = string.Empty;

                TimeStamp ts = new TimeStamp();
                result = ts.MaxEntry(this.CreatedOn, this.CreatedBy, this.ModifiedOn, this.ModifiedBy);

                return result;
            }
        }
    }
}