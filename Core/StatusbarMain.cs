//-----------------------------------------------------------------------
// <copyright file="StatusbarContent.cs" company="NRM Netzdienste Rhein-Main GmbH">
//     Class: StatusbarContent
//     Copyright © NRM Netzdienste Rhein-Main GmbH 2023
// </copyright>
//
// <author>DeveloperName - NRM Netzdienste Rhein-Main GmbH</author>
// <email>DeveloperName@nrm-netzdienste.de</email>
// <date>18.08.2023 07:03:18</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Core
{
    using System;

    public class StatusbarMain
    {
        private static string currentUser = string.Empty;
        private static string notification = string.Empty;
        private static string databaseInfo = string.Empty;
        private static string databaseInfoTooltip = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusbarMain"/> class.
        /// </summary>
        static StatusbarMain()
        {
            UserChanged += (sender, e) => { return; };
            NotificationChanged += (sender, e) => { return; };
            DatabaseInfoChanged += (sender, e) => { return; };
            DatabaseInfoTooltipChanged += (sender, e) => { return; };

            CurrentUser = $"{Environment.UserDomainName}\\{Environment.UserName}";
            CurrentDate = $"{DateTime.Now.ToShortDateString()}";
        }

        public static string CurrentUser
        {
            get { return currentUser; }
            set
            {
                currentUser = value;
                OnUserChanged(EventArgs.Empty);
            }
        }

        public static string DatabaseInfo
        {
            get { return databaseInfo; }
            set
            {
                databaseInfo = value;
                OnDatabaseInfoChanged(EventArgs.Empty);
            }
        }

        public static string DatabaseInfoTooltip
        {
            get { return databaseInfoTooltip; }
            set
            {
                databaseInfoTooltip = value;
                OnDatabaseInfoTooltipChanged(EventArgs.Empty);
            }
        }

        public static string CurrentDate { get; set; }

        public static string CurrentHost { get; set; }

        public static string Notification
        {
            get { return notification; }
            set
            {
                notification = value;
                OnNotificationChanged(EventArgs.Empty);
            }
        }

        // Declare a static event representing changes to your static property
        public static event EventHandler UserChanged;
        public static event EventHandler NotificationChanged;
        public static event EventHandler DatabaseInfoChanged;
        public static event EventHandler DatabaseInfoTooltipChanged;

        // Raise the change event through this static method
        protected static void OnUserChanged(EventArgs e)
        {
            EventHandler handler = UserChanged;

            if (handler != null)
            {
                handler(null, e);
            }
        }

        protected static void OnNotificationChanged(EventArgs e)
        {
            EventHandler handler = NotificationChanged;

            if (handler != null)
            {
                handler(null, e);
            }
        }

        protected static void OnDatabaseInfoChanged(EventArgs e)
        {
            EventHandler handler = DatabaseInfoChanged;

            if (handler != null)
            {
                handler(null, e);
            }
        }

        protected static void OnDatabaseInfoTooltipChanged(EventArgs e)
        {
            EventHandler handler = DatabaseInfoTooltipChanged;

            if (handler != null)
            {
                handler(null, e);
            }
        }
    }
}
