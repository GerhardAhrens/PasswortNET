namespace PasswortNET.Views.ContentControls
{
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    using ModernBaseLibrary.Core;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.DataRepository;

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

        public int CountAll
        {
            get => base.GetValue<int>();
            set => base.SetValue(value);
        }

        public int CountWebsite
        {
            get => base.GetValue<int>();
            set => base.SetValue(value);
        }

        public int CountPasswort
        {
            get => base.GetValue<int>();
            set => base.SetValue(value);
        }

        public int CountPin
        {
            get => base.GetValue<int>();
            set => base.SetValue(value);
        }

        public int CountLicense
        {
            get => base.GetValue<int>();
            set => base.SetValue(value);
        }

        public DateTime LastAccess
        {
            get => base.GetValue<DateTime>();
            set => base.SetValue(value);
        }

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("BackAboutCommand", new RelayCommand(p1 => this.BackHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("SelectionChangedCommand", new RelayCommand(p1 => this.OnSelectionChanged(p1), p2 => true));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.IsUCLoaded = true;
            AssemblyMetaInfo ami = new AssemblyMetaInfo();
        }

        private void OnSelectionChanged(object e)
        {
            if (this.IsUCLoaded == true)
            {
                int index = ((Selector)(((FrameworkElement)e).Parent)).SelectedIndex;
                if (index == 0)
                {
                    StatusbarMain.Statusbar.SetNotification("Informationen zur Anwendung.");
                }
                else if (index == 1)
                {
                    using (StatistikRepository repository = new StatistikRepository())
                    {
                        Result<bool> result = repository.GetStatistic();
                        if (result.Success == true)
                        {
                            this.CountAll = repository.CountAll;
                            this.CountWebsite = repository.CountWebsite;
                            this.CountPasswort = repository.CountPasswort;
                            this.CountPin = repository.CountPin;
                            this.CountLicense = repository.CountLicense;
                            this.LastAccess = repository.LastAccess;
                        }

                        StatusbarMain.Statusbar.SetNotification($"Bereit: {result.ElapsedMilliseconds}ms");
                    }
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
