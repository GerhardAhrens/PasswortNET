﻿namespace PasswortNET.Views.ContentControls
{
    using System.Windows;
    using System.Windows.Controls;
    using ModernBaseLibrary.Core;
    using ModernBaseLibrary.Extension;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;

    /// <summary>
    /// Interaktionslogik für AppSettingsUC.xaml
    /// </summary>
    public partial class AppSettingsUC : UserControlBase
    {
        public AppSettingsUC() : base(typeof(AppSettingsUC))
        {
            this.InitializeComponent();

            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);

            this.InitCommands();
            this.DataContext = this;
        }

        public string Titel
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public bool ExitQuestion
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value);
        }

        public bool ApplicationPosition
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value);
        }

        public Dictionary<int,string> RunEnvironmentSelectionSource
        {
            get => base.GetValue<Dictionary<int, string>>();
            set => base.SetValue(value);
        }

        public string RunEnvironmentSelectionChanged
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public bool IsUCLoaded { get; set; } = false;

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("BackSettingsCommand", new RelayCommand(p1 => this.BackHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("SelectionChangedCommand", new RelayCommand(p1 => this.OnSelectionChanged(p1), p2 => true));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Titel = "Einstellungen zur Anwendung ändern";
            StatusbarMain.Statusbar.SetNotification("Ändern Sie verschieden Einstellungen.");

            using (ApplicationSettings settings = new ApplicationSettings())
            {
                if (settings.IsExitSettings() == true)
                {
                    settings.Load();
                    settings.LastAccess = DateTime.Now;
                    settings.LastUser = UserInfo.TS().CurrentDomainUser;
                    this.RunEnvironmentSelectionChanged = settings.RunEnvironment;
                    this.ExitQuestion = settings.ExitApplicationQuestion;
                    this.ApplicationPosition = settings.SaveLastWindowsPosition;
                }
            }

            this.RunEnvironmentSelectionSource = RunEnvironments.None.ToDictionary<RunEnvironments>();

            this.IsUCLoaded = true;
        }

        private void OnSelectionChanged(object e)
        {
            if (this.IsUCLoaded == true)
            {
                this.SaveSettings();
            }
        }

        private void BackHandler(object p1)
        {
            this.SaveSettings();

            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = MainButton.Home,
            });
        }

        private void SaveSettings()
        {
            using (ApplicationSettings settings = new ApplicationSettings())
            {
                if (settings.IsExitSettings() == true)
                {
                    settings.Load();
                    settings.LastAccess = DateTime.Now;
                    settings.LastUser = UserInfo.TS().CurrentDomainUser;
                    settings.RunEnvironment = this.RunEnvironmentSelectionChanged;
                    settings.ExitApplicationQuestion = this.ExitQuestion;
                    settings.SaveLastWindowsPosition = this.ApplicationPosition;
                    settings.Save();
                }

                settings.Load();

                App.ExitApplicationQuestion = settings.ExitApplicationQuestion;
                App.SaveLastWindowsPosition = settings.SaveLastWindowsPosition;
                App.RunEnvironment = settings.RunEnvironment;
            }
        }
    }
}
