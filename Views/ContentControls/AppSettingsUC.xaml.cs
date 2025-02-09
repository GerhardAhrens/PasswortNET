namespace PasswortNET.Views.ContentControls
{
    using System.Windows;
    using System.Windows.Controls;
    using ModernBaseLibrary.Core;

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

        public string RunEnvironmentSelectionChanged
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("BackSettingsCommand", new RelayCommand(p1 => this.BackHandler(p1), p2 => true));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            WeakEventManager<TabControl, SelectionChangedEventArgs>.AddHandler(this.tcSettings, "SelectionChanged", this.OnSelectionChanged);
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
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveSettings();
        }

        private void BackHandler(object p1)
        {
            SaveSettings();

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
