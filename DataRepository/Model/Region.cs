//-----------------------------------------------------------------------
// <copyright file="Region.cs" company="www.lifeprojects.de">
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
    using System;
    using System.Diagnostics;

    using ModernBaseLibrary.Extension;

    [DebuggerDisplay("Name={this.Name}")]
    public partial class Region 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Region"/> class.
        /// </summary>
        public Region()
        {
            this.LastExport = DateTime.Today.DefaultDate();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Background { get; set; }

        public int Symbol { get; set; }

        public DateTime LastExport { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
