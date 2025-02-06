namespace PasswortNET.Views.ContentControls
{
    using System;
    using System.Security;
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

        public string Titel
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
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

        public override void InitCommands()
        {
            base.CmdAgg.AddOrSetCommand("LoginCommand", new RelayCommand(p1 => this.LoginHandler(p1), p2 => true));
            base.CmdAgg.AddOrSetCommand("InputLoginCommand", new RelayCommand(p1 => this.InputLoginHandler(p1), p2 => true));
            base.CmdAgg.AddOrSetCommand("CloseWindowCommand", new RelayCommand(p1 => this.CloseWindowHandler(p1), p2 => true));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Titel = "An PasswortNET anmelden";

            using (ApplicationSettings settings = new ApplicationSettings())
            {
                if (settings.IsExitSettings() == false)
                {
                    settings.LastAccess = DateTime.Now;
                    settings.LastUser = UserInfo.TS().CurrentDomainUser;
                    settings.RunEnvironment = "dev";
                    settings.ExitApplicationQuestion = false;
                    settings.SaveLastWindowsPosition = false;
                    settings.Save();
                }

                settings.Load();

                App.ExitApplicationQuestion = settings.ExitApplicationQuestion;
                App.SaveLastWindowsPosition = settings.SaveLastWindowsPosition;
                App.RunEnvironment = settings.RunEnvironment;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => { this.TxtBenutzername.Focus(); }));
            }
        }

        private void LoginHandler(object p1)
        {
            string userName = this.LoginUser;
            string passwort = this.passwordBox.Password;

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
                        settings.Hash = encryptHash;
                        settings.Save();
                    }
                    else
                    {
                        compareHash = settings.Hash.Decrypt();
                    }
                }
            }

            if (string.Equals(hash,compareHash) == false)
            {
                NotificationBoxButton result = this.notificationService.BenutzerPasswortFalsch();
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

            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = MainButton.Home,
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
            this.LoginUser = "lifeprojects";
            this.passwordBox.Text = "beate.2019";
        }
    }
}
