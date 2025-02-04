namespace PasswortNET.Views.ContentControls
{
    using System.Windows.Controls;

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
            this.InitCommands();
            this.DataContext = this;
        }

        public override void InitCommands()
        {
            base.CmdAgg.AddOrSetCommand("BackCommand", new RelayCommand(p1 => this.BackHandler(p1), p2 => true));
        }

        private void BackHandler(object p1)
        {
            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = MainButton.Home,
            });
        }
    }
}
