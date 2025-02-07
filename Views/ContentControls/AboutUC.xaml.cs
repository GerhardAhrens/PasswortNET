namespace PasswortNET.Views.ContentControls
{
    using System.Windows;
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

            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);

            this.InitCommands();
            this.DataContext = this;
        }

        public string Titel
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("BackAboutCommand", new RelayCommand(p1 => this.BackHandler(p1), p2 => true));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Titel = "Anwendungs Information";
            StatusbarMain.Statusbar.SetNotification("Informationen zur Anwendung.");
        }

        private void TabControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private static void item_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e, TabControl tabControl)
        {
        }

        private void tcSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
