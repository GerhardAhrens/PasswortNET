namespace PasswortNET.Views.TabAppSettings
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;

    using ModernBaseLibrary.Core;
    using ModernBaseLibrary.Extension;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.Core.Enums;
    using PasswortNET.DataRepository;
    using PasswortNET.Model;

    /// <summary>
    /// Interaktionslogik für TabAppSettingsDatabase.xaml
    /// </summary>
    public partial class TabAppSettingsDatabase : UserControlBase
    {
        public TabAppSettingsDatabase()
        {
            this.InitializeComponent();

            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Unloaded", this.OnUnloaded);

            this.InitCommands();
            this.DataContext = this;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            using (ApplicationSettings settings = new ApplicationSettings())
            {
                if (settings.IsExitSettings() == true)
                {
                    settings.Load();
                    settings.IsDatabaseBackup = this.IsDatabaseBackup;
                    settings.MaxBackupFile = this.MaxBackupFile;
                    settings.BackupFrequency = (int)this.BackupFrequencySelected;
                    settings.Save();
                }
            }
        }

        public bool IsDatabaseBackup
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value);
        }

        public Dictionary<BackupFrequency, string> BackupFrequencySource
        {
            get => base.GetValue<Dictionary<BackupFrequency, string>>();
            set => base.SetValue(value);
        }

        public BackupFrequency BackupFrequencySelected
        {
            get => base.GetValue<BackupFrequency>();
            set => base.SetValue(value);
        }

        public int MaxBackupFile
        {
            get => base.GetValue<int>();
            set => base.SetValue(value);
        }

        public IEnumerable<int> MaxBackupFileSource
        {
            get => base.GetValue<IEnumerable<int>>();
            set => base.SetValue(value);
        }

        public string LastBackupInfo
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public string DatabaseFolder
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public string DatabaseBackuFolder
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("CreateBackupCommand", new RelayCommand(p1 => this.CreateBackupHandler(), p2 => true));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.BuildDataSource();
            this.IsUCLoaded = true;
        }

        private void BuildDataSource()
        {
            if (this.IsUCLoaded == false)
            {
                return;
            }

            using (ApplicationSettings settings = new ApplicationSettings())
            {
                if (settings.IsExitSettings() == true)
                {
                    settings.Load();
                    this.IsDatabaseBackup = settings.IsDatabaseBackup;
                    this.MaxBackupFile = settings.MaxBackupFile;
                    this.BackupFrequencySelected = settings.BackupFrequency.ToEnum<BackupFrequency>();
                    this.DatabaseFolder = Path.GetDirectoryName(settings.DatabaseFullname);
                    this.DatabaseBackuFolder = Path.GetDirectoryName(settings.DatabaseBackupFullname);
                }
            }

            this.BackupFrequencySource = new EnumDescripionToDictionary<BackupFrequency>();
            this.MaxBackupFileSource = Enumerable.Range(1, 10).Select(x => (x - 1) + 1);

            this.LastBackupInfo = new DatabaseBackup().BackupInfo();
        }

        private void CreateBackupHandler()
        {
            using (DatabaseBackup db = new DatabaseBackup())
            {
                db.CheckAndRun();
                this.LastBackupInfo = db.BackupInfo();
            }
        }

    }
}
