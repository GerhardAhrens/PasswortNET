//-----------------------------------------------------------------------
// <copyright file="IgnorAuditTrailWords.cs" company="Lifeprojects.de">
//     Class: IgnorAuditTrailWords
//     Copyright © Lifeprojects.de 2021
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>12.04.2021</date>
//
// <summary>
// Die Klasse gibt eine Liste von Properties die für das Audit Trail ingnoriert werden sollen
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.AuditTrail
{

    using System.Collections.Generic;

    using ModernBaseLibrary.Core;

    public class IgnorAuditTrailWords : DisposableCoreBase
    {

        public IgnorAuditTrailWords()
        {
            if (this.IgnorProperties == null)
            {
                this.IgnorProperties = new List<string>();
                this.IgnorProperties.Add("CreatedBy");
                this.IgnorProperties.Add("CreatedOn");
                this.IgnorProperties.Add("ModifiedBy");
                this.IgnorProperties.Add("ModifiedOn");
                this.IgnorProperties.Add("Timestamp");
                this.IgnorProperties.Add("FullName");
                this.IgnorProperties.Add("IsModified");
                this.IgnorProperties.Add("Id");
            }
        }

        public List<string> IgnorProperties { get; private set; }
    }
}
