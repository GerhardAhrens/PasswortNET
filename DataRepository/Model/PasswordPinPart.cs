//-----------------------------------------------------------------------
// <copyright file="PasswordPin.cs" company="www.lifeprojects.de">
//     Class: PasswordPin
//     Copyright © www.lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - www.lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>27.04.2022 14:17:30</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Model
{
    using ModernBaseLibrary.Core;

    public partial class PasswordPin
    {
        public string FullName
        {
            get
            {
                return $"{this.Id}|{this.Title}|{this.Description}";
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
