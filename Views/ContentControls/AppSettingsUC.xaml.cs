namespace PasswortNET.Views.ContentControls
{
    using System.Windows;
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
            this.CmdAgg.AddOrSetCommand("BackSettingsCommand", new RelayCommand(p1 => this.BackHandler(p1), p2 => true));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Titel = "Einstellungen zur Anwendung ändern";
            StatusbarMain.Statusbar.SetNotification("Ändern Sie verschieden Einstellungen.");
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
