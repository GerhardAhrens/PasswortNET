namespace PasswortNET.Views.ContentControls
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.Model;

    /// <summary>
    /// Interaktionslogik für MainOverviewUC.xaml
    /// </summary>
    public partial class MainOverviewUC : UserControlBase
    {
        public MainOverviewUC()
        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            this.InitCommands();
            this.DataContext = this;
        }

        #region Properties
        public int DisplayRowCount
        {
            get => base.GetValue<int>();
            set => base.SetValue(value);
        }

        public string FilterDefaultSearch
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public ICollectionView DialogDataView
        {
            get => base.GetValue<ICollectionView>();
            set => base.SetValue(value);
        }

        public PasswordPin CurrentSelectedItem
        {
            get => base.GetValue<PasswordPin>();
            set => base.SetValue(value);
        }

        public ViewBase CustomView
        {
            get => base.GetValue<ViewBase>();
            set => base.SetValue(value);
        }

        public IEnumerable<Region> RegionSource
        {
            get => base.GetValue<IEnumerable<Region>>();
            set => base.SetValue(value);
        }

        public Region RegionCurrent
        {
            get => base.GetValue<Region>();
            set => base.SetValue(value);
        }

        public bool IsFilterContentFound
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value);
        }

        private AccessTyp AccessTypState { get; set; }

        #endregion Properties

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("LogoffCommand", new RelayCommand(p1 => this.LogoffHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("ListViewCommand", new RelayCommand(p1 => ListViewContextMenu(p1.ToString()), p2 => true));
            this.CmdAgg.AddOrSetCommand("EditEntryCommand", new RelayCommand(p1 => this.EditEntryHandler(p1), p2 => this.CanEditEntryHandler()));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.IsUCLoaded = true;
            this.IsFilterContentFound = false;
            this.ChangeView("TileView");
            this.AccessTypState = AccessTyp.All;
            this.LoadDataHandler();
        }

        #region Load and Filter Data
        private void LoadDataHandler()
        {

        }
        #endregion Load and Filter Data

        private void ListViewContextMenu(string menuItem)
        {
            this.ChangeView(menuItem);
        }

        private void ChangeView(string viewTyp = "")
        {
            if (viewTyp == "GridView")
            {
                this.CustomView = Application.Current.TryFindResource("gridView") as ViewBase;
            }
            else if (viewTyp == "TileView")
            {
                this.CustomView = Application.Current.TryFindResource("tileView") as ViewBase;
            }
            else
            {
                this.CustomView = Application.Current.TryFindResource("tileView") as ViewBase;
            }
        }

        private bool CanEditEntryHandler()
        {
            return this.DisplayRowCount == 0 ? false : true;
        }

        #region Command Handler Methodes
        private void EditEntryHandler(object commandArgs)
        {

        }

        private void LogoffHandler(object p1)
        {
            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = MainButton.Login,
            });
        }
        #endregion Command Handler Methodes
    }
}
