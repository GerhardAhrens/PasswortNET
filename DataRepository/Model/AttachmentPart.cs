//-----------------------------------------------------------------------
// <copyright file="Attachment.cs" company="www.lifeprojects.de">
//     Class: Attachment
//     Copyright © www.lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - www.Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>30.06.2022 13:53:09</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Model
{
    using System.Text.Json.Serialization;

    using ModernBaseLibrary.Core;

    public sealed partial class Attachment 
    {
        [JsonIgnore]
        public string FullName
        {
            get
            {
                return $"{this.ObjectName}-{this.Filename}-{this.FileSize}";
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
