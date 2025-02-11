namespace PasswortNET.Views.ContentControls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

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

        private bool IsUCLoaded { get; set; } = false;

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("BackAboutCommand", new RelayCommand(p1 => this.BackHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("SelectionChangedCommand", new RelayCommand(p1 => this.OnSelectionChanged(p1), p2 => true));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Titel = "Anwendungs Information und Statistik";
            this.Focus();
            this.IsUCLoaded = true;
        }

        private void OnSelectionChanged(object e)
        {
            StatusbarMain.Statusbar.SetNotification("Informationen zur Anwendung.");

            if (this.IsUCLoaded == true)
            {
                int index = ((Selector)(((FrameworkElement)e).Parent)).SelectedIndex;
                if (index == 0)
                {
                    StatusbarMain.Statusbar.SetNotification("Informationen zur Anwendung.");
                }
                else if (index == 1)
                {
                    StatusbarMain.Statusbar.SetNotification("Informationen zur Statistik der Datenbank Einträge.");
                }
            }
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
