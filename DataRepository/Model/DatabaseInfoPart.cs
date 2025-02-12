//-----------------------------------------------------------------------
// <copyright file="DatabaseInfoPart.cs" company="www.lifeprojects.de">
//     Class: DatabaseInfo
//     Copyright © www.lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - www.lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>30.05.2022 15:02:30</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Model
{
    using System.Diagnostics;

    using ModernBaseLibrary.Core;

    [DebuggerDisplay("Name={this.Name}")]
    public partial class DatabaseInfo
    {
        public string FullName
        {
            get
            {
                return $"{this.Id}|{this.Name}";
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
