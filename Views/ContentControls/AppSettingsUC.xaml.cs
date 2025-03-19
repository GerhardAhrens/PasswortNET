namespace PasswortNET.Views.ContentControls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    using ModernBaseLibrary.Core;
    using ModernBaseLibrary.Extension;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.Core.Enums;

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

        public int RunEnvironmentSelectionChanged
        {
            get => base.GetValue<int>();
            set => base.SetValue(value);
        }

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("BackSettingsCommand", new RelayCommand(p1 => this.BackHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("SelectionChangedCommand", new RelayCommand(p1 => this.OnSelectionChanged(p1), p2 => true));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
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

                int index = ((Selector)(((FrameworkElement)e).Parent)).SelectedIndex;
                if (index == 0)
                {
                    StatusbarMain.Statusbar.SetNotification("Ändern Sie verschieden Einstellungen.");
                }
                else if (index == 1)
                {
                    StatusbarMain.Statusbar.SetNotification("Einstellungen zur Datenbank, Backup, Verzeichnisse.");
                }
                else if (index == 2)
                {
                    StatusbarMain.Statusbar.SetNotification("Tag zum markieren und zusammenfassen von Gruppen.");
                }
            }
        }

        private void BackHandler(object p1)
        {
            this.SaveSettings();

            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = MainButton.MainOverview,
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
