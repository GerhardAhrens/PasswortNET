//-----------------------------------------------------------------------
// <copyright file="RegionPart.cs" company="www.lifeprojects.de">
//     Class: Region
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
    using System.Text.Json.Serialization;

    using ModernBaseLibrary.Core;

    [DebuggerDisplay("Name={this.Name}")]
    public partial class Region 
    {
        [JsonIgnore]
        public string FullName
        {
            get
            {
                return $"{this.Id}|{this.Name}|{this.Background}|{this.Symbol}";
            }
        }

        [JsonIgnore]
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
