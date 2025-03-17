namespace PasswortNET.Views.ContentControls
{
    using System;
    using System.Security;
    using System.Security.Policy;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Ribbon;
    using System.Windows.Input;
    using System.Windows.Threading;

    using ModernBaseLibrary.Core;
    using ModernBaseLibrary.Extension;

    using ModernIU.Controls;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.Core.Enums;
    using PasswortNET.DataRepository;
    using PasswortNET.Model;

    /// <summary>
    /// Interaktionslogik für LoginUC.xaml
    /// </summary>
    public partial class LoginUC : UserControlBase
    {
        private INotificationService notificationService = new NotificationService();

        public LoginUC() : base(typeof(LoginUC))
        {
            this.InitializeComponent();

            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);

            this.InitCommands();
            this.DataContext = this;
        }

        public string LoginUser
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public string LoginPasswort
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public bool IsPasswordRepeat
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value);
        }

        private int MaxTryLogin { get; set; } = 3;


        public override void InitCommands()
        {
            base.CmdAgg.AddOrSetCommand("LoginCommand", new RelayCommand(p1 => this.LoginHandler(p1), p2 => true));
            base.CmdAgg.AddOrSetCommand("InputLoginCommand", new RelayCommand(p1 => this.InputLoginHandler(p1), p2 => true));
            base.CmdAgg.AddOrSetCommand("CloseWindowCommand", new RelayCommand(p1 => this.CloseWindowHandler(p1), p2 => true));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            WeakEventManager<IconTextBox, KeyEventArgs>.AddHandler(this.TxtBenutzername, "KeyDown", this.OnKeyDown);
            WeakEventManager<MPasswordBox, KeyEventArgs>.AddHandler(this.TxtPassword, "KeyDown", this.OnKeyDown);
            WeakEventManager<MPasswordBox, KeyEventArgs>.AddHandler(this.TxtPasswordRepeat, "KeyDown", this.OnKeyDown);

            StatusbarMain.Statusbar.SetNotification("Geben Sie einen Benutzer und ein  Passwort an.");

            using (ApplicationSettings settings = new ApplicationSettings())
            {
                if (settings.IsExitSettings() == false)
                {
                    settings.LastAccess = DateTime.Now;
                    settings.LastUser = UserInfo.TS().CurrentDomainUser;
                    settings.RunEnvironment = 0;
                    settings.ExitApplicationQuestion = false;
                    settings.SaveLastWindowsPosition = false;
                    settings.Hash = string.Empty;
                    settings.DatabaseBackupFullname = DatabaseName.FullBackupName;
                    settings.DatabaseFullname = DatabaseName.FullDatabaseName;
                    settings.Save();
                }

                settings.Load();

                App.ExitApplicationQuestion = settings.ExitApplicationQuestion;
                App.SaveLastWindowsPosition = settings.SaveLastWindowsPosition;
                App.RunEnvironment = settings.RunEnvironment;
            }

            this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => { this.TxtBenutzername.Focus(); }));

            using (ApplicationSettings settings = new ApplicationSettings())
            {
                if (settings.IsExitSettings() == true)
                {
                    settings.Load();

                    if (string.IsNullOrEmpty(settings.Hash) == true)
                    {
                        this.IsPasswordRepeat = true;
                    }
                    else
                    {
                        this.IsPasswordRepeat = false;
                    }
                }
            }
        }

        private void LoginHandler(object p1)
        {
            const int MAXLOGIN = 3;
            string databaseFile = string.Empty;
            RunEnvironments runEnvironment = RunEnvironments.None;
            bool isFirstStart = false;
            string userName = this.LoginUser;
            string passwort = this.TxtPassword.Password;
            string passwortRepeat = this.TxtPasswordRepeat.Password;
            string hash = $"{userName}{passwort}".ToMD5(false);
            string encryptHash = hash.Encrypt();
            string compareHash = string.Empty;

            using (ApplicationSettings settings = new ApplicationSettings())
            {
                if (settings.IsExitSettings() == true)
                {
                    settings.Load();

                    if (string.IsNullOrEmpty(settings.Hash) == true)
                    {
                        isFirstStart = true;
                        settings.Hash = encryptHash;
                        string ctrlHash = $"{userName}|{passwort}";
                        settings.ControlHash = ctrlHash.Encrypt();
                        settings.DatabaseFullname = DatabaseName.FullDatabaseName;
                        settings.RunEnvironment = (int)runEnvironment;
                        settings.Save();
                        compareHash = settings.Hash.Decrypt();
                        databaseFile = settings.DatabaseFullname;
                        runEnvironment = settings.RunEnvironment.ToEnum<RunEnvironments>();
                    }
                    else
                    {
                        isFirstStart = false;
                        compareHash = settings.Hash.Decrypt();
                        if (string.IsNullOrEmpty(settings.DatabaseFullname) == false)
                        {
                            databaseFile = settings.DatabaseFullname;
                            runEnvironment = settings.RunEnvironment.ToEnum<RunEnvironments>();
                        }
                        else
                        {
                            settings.DatabaseFullname = DatabaseName.FullDatabaseName;
                            settings.RunEnvironment = (int)runEnvironment;
                            settings.Save();

                            databaseFile = DatabaseName.FullDatabaseName;
                            runEnvironment = settings.RunEnvironment.ToEnum<RunEnvironments>();
                        }
                    }
                }
            }

            if (isFirstStart == true)
            {
                if (string.Equals(passwort, passwortRepeat) == false)
                {
                    this.notificationService.PasswortRepeatWrong();
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => { this.TxtPassword.Focus(); }));
                    return;
                }
            }

            if (string.Equals(hash,compareHash) == false)
            {
                this.MaxTryLogin--;
                if (this.MaxTryLogin == 0)
                {
                    this.notificationService.MaxTryLogin(MAXLOGIN);
                    this.CloseWindowHandler(null);
                }

                NotificationBoxButton result = this.notificationService.BenutzerPasswortFalsch(this.MaxTryLogin);
                if (result == NotificationBoxButton.Yes)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => { this.TxtBenutzername.Focus(); }));
                    return;
                }
                else
                {
                    this.CloseWindowHandler(null);
                }
            }

            using (DatabaseManager dm = new DatabaseManager(databaseFile))
            {
                Result<DatabaseInfo> dbi = dm.CheckDatabase();
                StatusbarMain.Statusbar.Notification = $"Bereit: {dbi.ElapsedTime}ms";
                StatusbarMain.Statusbar.SetDatabaeInfo(databaseFile, runEnvironment);
            }

            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = MainButton.MainOverview,
                LoginHash = hash,
            });
        }

        private void CloseWindowHandler(object p1)
        {
            Window mainWindow = this.Parent.TryFindParent<Window>();
            if (mainWindow != null)
            {
                mainWindow.Close();
            }
        }

        private void InputLoginHandler(object p1)
        {
            using (ApplicationSettings settings = new ApplicationSettings())
            {
                settings.Load();
                string ctrlHash = settings.ControlHash.Decrypt();
                this.LoginUser = ctrlHash.Split('|').FirstOrDefault();
                this.TxtPassword.Text = ctrlHash.Split('|').LastOrDefault();
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            IconTextBox it = sender as IconTextBox;
            if (it != null)
            {
                if (it.Name == "TxtBenutzername" && e.Key == Key.Tab)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => { this.TxtPassword.Focus(); }));
                }
            }

            MPasswordBox mp = sender as MPasswordBox;
            if (mp != null)
            {
                if (mp.Name == "TxtPassword" && e.Key == Key.Tab && IsPasswordRepeat == true)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => { this.TxtPasswordRepeat.Focus(); }));
                }
                else if (mp.Name == "TxtPassword" && e.Key == Key.Tab && IsPasswordRepeat == false)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => { this.TxtBenutzername.Focus(); }));
                }
                else if (mp.Name == "TxtPasswordRepeat" && e.Key == Key.Tab)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => { this.TxtBenutzername.Focus(); }));
                }
            }
        }
    }
}
