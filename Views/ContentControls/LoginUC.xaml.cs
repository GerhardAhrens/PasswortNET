namespace PasswortNET.Views.ContentControls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Ribbon;
    using System.Windows.Input;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;

    /// <summary>
    /// Interaktionslogik für LoginUC.xaml
    /// </summary>
    public partial class LoginUC : UserControlBase
    {
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
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Titel = "An PasswortNET anmelden";
        }

        private void LoginHandler(object p1)
        {
            string userName = this.LoginUser;
            string passwort = this.passwordBox.Password;

            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = MainButton.Home,
            });
        }
    }
}
