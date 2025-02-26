//-----------------------------------------------------------------------
// <copyright file="PasswordPin.cs" company="www.lifeprojects.de">
//     Class: PasswordPin
//     Copyright � www.lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - www.lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>27.04.2022 14:17:30</date>
//
// <summary>
// Klasse f�r 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Model
{
    using System.Text.Json.Serialization;

    using ModernBaseLibrary.Core;

    public partial class PasswordPin
    {
        [JsonIgnore]
        public string FullName
        {
            get
            {
                return $"{this.Id}|{this.Title}|{this.Description}";
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
