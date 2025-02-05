namespace PasswortNET.Views
{
    using System;
    using System.ComponentModel;
    using System.Runtime.Versioning;
    using System.Windows;
    using System.Windows.Controls;

    using ModernBaseLibrary.Core;
    using ModernBaseLibrary.Extension;

    using ModernIU.Controls;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.Views.ContentControls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class MainWindow : WindowBase, IDialogClosing
    {
        private INotificationService _notificationService = new NotificationService();

        public MainWindow() : base(typeof(MainWindow))
        {
            this.InitializeComponent();

            this.DialogDescription = "PasswortNET";

            base.EventAgg.Subscribe<ChangeViewEventArgs>(this.ChangeControl);
            WeakEventManager<Window, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            this.InitCommands();
            this.DataContext = this;
        }

        public string DialogDescription
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public bool IsDropDownOpen
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value);
        }

        public UserControl WorkContent
        {
            get { return base.GetValue<UserControl>(); }
            set { base.SetValue(value); }
        }

        public bool IsAppSettings
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value);
        }

        public bool IsLogoff
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value);
        }

        public bool IsWorkPassword
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value);
        }

        public override void InitCommands()
        {
            base.CmdAgg.AddOrSetCommand("CloseWindowCommand", new RelayCommand(p1 => this.CloseWindowHandler(p1), p2 => true));
            base.CmdAgg.AddOrSetCommand("LogoffCommand", new RelayCommand(p1 => this.LogoffHandler(p1), p2 => true));
            base.CmdAgg.AddOrSetCommand("AppSettingsCommand", new RelayCommand(p1 => this.AppSettingsHandler(p1), p2 => true));
            base.CmdAgg.AddOrSetCommand("AboutCommand", new RelayCommand(p1 => this.AboutHandler(p1), p2 => true));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            NotificationService.RegisterDialog<QuestionYesNo>();
            NotificationService.RegisterDialog<QuestionHtmlYesNo>();

            /* Letzte Windows Positionn landen*/
            using (UserPreferences userPrefs = new UserPreferences(this))
            {
                userPrefs.Load();
            }

            ChangeViewEventArgs arg = new ChangeViewEventArgs();
            arg.MenuButton = MainButton.Login;
            this.ChangeControl(arg);
        }


        private void CloseWindowHandler(object p1)
        {
            this.Close();
        }

        private void LogoffHandler(object p1)
        {
            ChangeViewEventArgs arg = new ChangeViewEventArgs();
            arg.MenuButton = MainButton.Login;
            this.ChangeControl(arg);
        }

        private void AppSettingsHandler(object p1)
        {
            ChangeViewEventArgs arg = new ChangeViewEventArgs();
            arg.MenuButton = MainButton.AppSettings;
            this.ChangeControl(arg);
        }

        private void AboutHandler(object p1)
        {
            ChangeViewEventArgs arg = new ChangeViewEventArgs();
            arg.MenuButton = MainButton.About;
            this.ChangeControl(arg);
        }

        public override void OnViewIsClosing(CancelEventArgs e)
        {
            Window window = Application.Current.MainWindow;
            if (window != null)
            {
                NotificationBoxButton result = this._notificationService.ApplicationExit();
                if (result == NotificationBoxButton.Yes)
                {
                    using (UserPreferences userPrefs = new UserPreferences(this))
                    {
                        userPrefs.Save();
                    }

                    e.Cancel = false;
                    Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                    Application.Current.Shutdown();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void ChangeControl(ChangeViewEventArgs e)
        {
            MenuWorkArea view = DialogFactory.Get(e.MenuButton);
            if (view is MenuWorkArea menuWorkArea)
            {
                string name = e.MenuButton.ToDescription();
                this.WorkContent = menuWorkArea.WorkContent;
                this.WorkContent.VerticalAlignment = VerticalAlignment.Stretch;
                if (this.WorkContent.GetType() == typeof(LoginUC))
                {
                    this.IsAppSettings = false;
                    this.IsLogoff = false;
                    this.IsWorkPassword = false;
                }
                else if (this.WorkContent.GetType() == typeof(AppSettingsUC))
                {
                    this.IsAppSettings = false;
                    this.IsWorkPassword = false;
                    this.IsLogoff = true;
                }
                else if (this.WorkContent.GetType() == typeof(AboutUC))
                {
                    this.IsAppSettings = false;
                    this.IsWorkPassword = false;
                    this.IsLogoff = true;
                }
                else if (this.WorkContent.GetType() == typeof(HomeUC))
                {
                    this.IsAppSettings = true;
                    this.IsLogoff = true;
                    this.IsWorkPassword = true;
                }
            }

            this.IsDropDownOpen = false;
        }
    }
}