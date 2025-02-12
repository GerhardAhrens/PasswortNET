namespace PasswortNET.Views.ContentControls
{
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    using ModernBaseLibrary.Core;
    using ModernBaseLibrary.Extension;

    using ModernIU.Controls;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.DataRepository;
    using PasswortNET.Model;

    /// <summary>
    /// Interaktionslogik für ChangePasswordUC.xaml
    /// </summary>
    public partial class ChangePasswordUC : UserControlBase
    {
        private INotificationService notificationService = new NotificationService();

        public ChangePasswordUC() : base(typeof(ChangePasswordUC))
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
            base.CmdAgg.AddOrSetCommand("ChangeLoginCommand", new RelayCommand(p1 => this.ChangeLoginHandler(p1), p2 => true));
            base.CmdAgg.AddOrSetCommand("CloseWindowCommand", new RelayCommand(p1 => this.CloseWindowHandler(p1), p2 => true));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            StatusbarMain.Statusbar.SetNotification("Geben Sie einen neuen Benutzer und/oder ein neues Passwort an.");

        }

        private void ChangeLoginHandler(object p1)
        {
            string databaseFile = string.Empty;
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
                    compareHash = settings.ControlHash.Decrypt().Replace("|",string.Empty).ToMD5(false);
                    if (string.IsNullOrEmpty(settings.DatabaseFullname) == true)
                    {
                        settings.DatabaseFullname = DatabaseName.FullDatabaseName;
                        settings.Save();
                        databaseFile = settings.DatabaseFullname;
                    }
                    else
                    {
                        databaseFile = settings.DatabaseFullname;
                    }
                }
            }

            if (File.Exists(databaseFile) == false)
            {
                this.notificationService.DatebaseNotExist(databaseFile);
                this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => { this.TxtPassword.Focus(); }));
                return;
            }

            if (string.Equals(passwort, passwortRepeat) == false)
            {
                this.notificationService.PasswortRepeatWrong();
                this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => { this.TxtPassword.Focus(); }));
                return;
            }

            if (string.Equals(compareHash, hash) == true)
            {
                this.notificationService.PasswortIsEquals();
                this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => { this.TxtPassword.Focus(); }));
                return;
            }

            NotificationBoxButton saveYN = this.notificationService.SaveNewPasswortYN();
            if (saveYN == NotificationBoxButton.Yes)
            {
                using (ApplicationSettings settings = new ApplicationSettings())
                {
                    if (settings.IsExitSettings() == true)
                    {
                        settings.Load();
                        settings.Hash = encryptHash;
                        string ctrlHash = $"{userName}|{passwort}";
                        settings.ControlHash = ctrlHash.Encrypt();
                        settings.Save();
                    }
                }

                using (DatabaseManager dm = new DatabaseManager(databaseFile))
                {
                    Result<DatabaseInfo> dbi = dm.CheckDatabase();
                    StatusbarMain.Statusbar.Notification = $"Bereit: {dbi.ElapsedTime}ms";
                }
            }

            this.notificationService.NoteForLogoff();

            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = MainButton.Login,
                LoginHash = hash,
            });
        }

        private void CloseWindowHandler(object p1)
        {
            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = MainButton.Home,
            });
        }
    }
}
