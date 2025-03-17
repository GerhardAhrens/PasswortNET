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
    using System;

    using ModernBaseLibrary.Core;
    using PasswortNET.Core;
    using PasswortNET.Core.Enums;

    public sealed partial class Attachment 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffortProject"/> class.
        /// </summary>
        public Attachment()
        {
            this.Id = Guid.NewGuid();
            this.CreatedBy = UserInfo.TS().CurrentUser;
            this.CreatedOn = UserInfo.TS().CurrentTime;
        }

        public Guid Id { get; set; }

        public Guid ObjectId { get; set; }

        public string ObjectName { get; set; }

        public byte[] Content { get; set; }

        public string Filename { get; set; }

        public string FileExtension { get; set; }

        public DateTime FileDateTime { get; set; }

        public long FileSize { get; set; }

        public SyncItemStatus SyncItemStatus { get; set; }

        public string SyncHash { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
