//-----------------------------------------------------------------------
// <copyright file="DatabaseInfo.cs" company="www.lifeprojects.de">
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
    using System;
    using System.Diagnostics;

    using ModernBaseLibrary.Extension;

    [DebuggerDisplay("Name={this.Name}")]
    public partial class DatabaseInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseInfo"/> class.
        /// </summary>
        public DatabaseInfo()
        {
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Version { get; set; }

        public string CreatedBy { get; set; }

        public DateTime LastExport { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
