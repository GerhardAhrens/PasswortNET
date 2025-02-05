namespace PasswortNET.Views.ContentControls
{
    using System.Windows.Controls;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;

    /// <summary>
    /// Interaktionslogik für AboutUC.xaml
    /// </summary>
    public partial class AboutUC : UserControlBase
    {
        public AboutUC() : base(typeof(AboutUC))
        {
            this.InitializeComponent();
            this.InitCommands();
            this.DataContext = this;
        }

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("BackAboutCommand", new RelayCommand(p1 => this.BackHandler(p1), p2 => true));
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
