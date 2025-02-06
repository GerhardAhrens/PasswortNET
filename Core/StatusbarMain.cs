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
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using ModernBaseLibrary.Core;

    using ModernIU.WPF.Base;

    public static class StatusbarMain 
    {
        private static string currentUser = string.Empty;
        private static string currentDate = string.Empty;
        private static string notification = string.Empty;
        private static string databaseInfo = string.Empty;
        private static string databaseInfoTooltip = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusbarMain"/> class.
        /// </summary>
        static StatusbarMain()
        {
            CurrentUser = $"{Environment.UserDomainName}\\{Environment.UserName}";
            CurrentDate = $"{DateTime.Now.ToShortDateString()}";
        }

        public static string CurrentUser
        {
            get { return currentUser; }
            set
            {
                currentUser = value;
                OnGlobalPropertyChanged();
            }
        }

        public static string CurrentDate
        {
            get { return currentDate; }
            set
            {
                currentDate = value;
                OnGlobalPropertyChanged();
            }
        }

        public static string DatabaseInfo
        {
            get { return databaseInfo; }
            set
            {
                databaseInfo = value;
                OnGlobalPropertyChanged();
            }
        }

        public static string DatabaseInfoTooltip
        {
            get { return databaseInfoTooltip; }
            set
            {
                databaseInfoTooltip = value;
                OnGlobalPropertyChanged();
            }
        }

        public static string CurrentHost { get; set; }

        public static string Notification
        {
            get { return notification; }
            set
            {
                notification = value;
                OnGlobalPropertyChanged();
            }
        }

        // Declare a static event representing changes to your static property
        public static event EventHandler GlobalPropertyChanged;

        // Raise the change event through this static method
        public static void OnGlobalPropertyChanged()
        {
            if (GlobalPropertyChanged != null)
            {
                GlobalPropertyChanged(typeof(StatusbarMain), EventArgs.Empty);
            }
        }
    }

    public static class StatusbarContent
    {
        private static StatusbarModel _StatusbarModel = new StatusbarModel();
        public static StatusbarModel Statusbar
        {
            get { return _StatusbarModel; }
            set { _StatusbarModel = value; }
        }
    }

    public class StatusbarModel : INotifyPropertyChanged
    {
        public StatusbarModel()
        {
            CurrentUser = $"{Environment.UserDomainName}\\{Environment.UserName}";
            CurrentDate = $"{DateTime.Now.ToShortDateString()}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string currentUser = string.Empty;
        public string CurrentUser
        {
            get { return currentUser; }
            set
            {
                currentUser = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentUser"));
            }
        }

        private static string currentDate = string.Empty;
        public string CurrentDate
        {
            get { return currentDate; }
            set
            {
                currentDate = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentDate"));
            }
        }

        private string notification = string.Empty;
        public string Notification
        {
            get { return this.notification; }
            set
            {
                this.notification = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Notification"));
            }
        }
    }
}
