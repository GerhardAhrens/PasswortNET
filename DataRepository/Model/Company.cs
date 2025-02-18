//-----------------------------------------------------------------------
// <copyright file="Company.cs" company="www.lifeprojects.de">
//     Class: Company
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
    using System;
    using System.Diagnostics;
    using System.Windows.Media;

    [DebuggerDisplay("Name={this.Name};ObjectId={this.ObjectId};ObjectTyp={this.ObjectTyp}")]
    public partial class Company
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Company"/> class.
        /// </summary>
        public Company()
        {
        }

        public Guid Id { get; set; }

        public Guid ObjectId { get; set; }

        public string ObjectTyp { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Website { get; set; }

        public byte[] Logo { get; set; }

        public string CompanyInfoMail { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
