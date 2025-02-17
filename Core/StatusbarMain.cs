namespace PasswortNET.Core
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;

    public static class StatusbarMain
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
            this.CurrentUser = $"{Environment.UserDomainName}\\{Environment.UserName}";
            this.CurrentDate = $"{DateTime.Now.ToShortDateString()}";
            this.RunEnvironment = "dev";
        }

        private string currentUser = string.Empty;
        public string CurrentUser
        {
            get { return currentUser; }
            set
            {
                currentUser = value;
                this.OnPropertyChanged();
            }
        }

        private string currentDate = string.Empty;
        public string CurrentDate
        {
            get { return currentDate; }
            set
            {
                currentDate = value;
                this.OnPropertyChanged();
            }
        }

        private string currentDatabase = string.Empty;
        public string CurrentDatabase
        {
            get { return this.currentDatabase; }
            set
            {
                this.currentDatabase = value;
                this.OnPropertyChanged();
            }
        }

        private string notification = string.Empty;
        public string Notification
        {
            get { return this.notification; }
            set
            {
                this.notification = value;
                this.OnPropertyChanged();
            }
        }

        private string databaseInfo = string.Empty;
        public string DatabaseInfo
        {
            get { return this.databaseInfo; }
            set
            {
                this.databaseInfo = value;
                this.OnPropertyChanged();
            }
        }

        private string databaseInfoTooltip = string.Empty;
        public string DatabaseInfoTooltip
        {
            get { return this.databaseInfoTooltip; }
            set
            {
                this.databaseInfoTooltip = value;
                this.OnPropertyChanged();
            }
        }

        private string runEnvironment = string.Empty;
        public string RunEnvironment
        {
            get { return this.runEnvironment; }
            set
            {
                this.runEnvironment = value;
                this.OnPropertyChanged();
            }
        }

        public void SetNotification(string notification = null)
        {
            if (string.IsNullOrEmpty(notification) == true)
            {
                this.Notification = string.Empty;
            }
            else
            {
                this.Notification = notification;
            }
        }

        public void SetDatabaeInfo(string databaseInfo = null, RunEnvironments runEnvironment = RunEnvironments.None)
        {
            if (string.IsNullOrEmpty(databaseInfo) == true)
            {
                this.DatabaseInfo = "Keine";
                this.DatabaseInfoTooltip = string.Empty;
            }
            else
            {
                this.DatabaseInfo = $"{Path.GetFileName(databaseInfo)} ({runEnvironment})";
                this.DatabaseInfoTooltip = $"{databaseInfo} ({runEnvironment})";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
