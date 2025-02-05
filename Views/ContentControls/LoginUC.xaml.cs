namespace PasswortNET.Views.ContentControls
{
    using System;
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

        public override void InitCommands()
        {
            base.CmdAgg.AddOrSetCommand("LoginCommand", new RelayCommand(p1 => this.LoginHandler(p1), p2 => true));
        }

        private void LoginHandler(object p1)
        {
            var aa = this.passwordBox.Password;
            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = MainButton.Home,
            });
        }
    }
}
