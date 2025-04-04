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
    using System;
    using System.Diagnostics;
    using System.Windows.Media;

    using ModernBaseLibrary.Extension;
    using ModernUILibrary.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.Core.Enums;

    [DebuggerDisplay("Title={this.Title};Username={this.Username};Symbol={this.Symbol};Background={this.Background}")]
    public partial class PasswordPin : ModelBase<PasswordPin>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordPin"/> class.
        /// </summary>
        public PasswordPin()
        {
            this.LastExport = DateTime.Today.DefaultDate();
        }

        public Guid Id { get; set; }

        public AccessTyp AccessTyp { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsAttachment { get; set; }

        public bool ShowDescription { get; set; }

        public string Username { get; set; }

        public string Passwort { get; set; }

        public string Pin { get; set; }

        public string Website { get; set; }

        public int Symbol { get; set; }

        public string Background { get; set; }

        public Guid CompanyId { get; set; }

        public string Company { get; set; }

        public string CompanyInfoMail { get; set; }

        public string LicenseName { get; set; }

        public string LicenseKey { get; set; }

        public bool IsLicenseAbo { get; set; }

        public DateTime LicenseValid { get; set; }

        public string Region { get; set; }

        public DateTime LastExport { get; set; }

        public DateTime ShowLast { get; set; }

        public bool IsShowLast { get; set; }

        public SyncItemStatus SyncItemStatus { get; set; }

        public string SyncHash { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
