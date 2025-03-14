namespace PasswortNET.Views.ContentControls
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using ModernBaseLibrary.Core;

    using ModernBaseLibrary.Extension;
    using ModernIU.Controls;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.DataRepository;
    using PasswortNET.Model;

    /// <summary>
    /// Interaktionslogik für MainOverviewUC.xaml
    /// </summary>
    public partial class MainOverviewUC : UserControlBase
    {
        private INotificationService notificationService = new NotificationService();

        public MainOverviewUC()
        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            this.InitCommands();
            this.DataContext = this;
        }

        #region Properties
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
            try
            {
                using (PasswordPinRepository repository = new PasswordPinRepository())
                {
                    this.RegionSource = repository.ListByRegion();

                    IEnumerable<PasswordPin> overviewSource = repository.List();
                    if (overviewSource != null)
                    {
                        this.DialogDataView = CollectionViewSource.GetDefaultView(overviewSource);
                        if (this.DialogDataView != null)
                        {

                            this.ChangeView("TileView");
                            this.DialogDataView.Filter = rowItem => this.DataDefaultFilter(rowItem as PasswordPin);
                            this.DialogDataView.SortDescriptions.Add(new SortDescription("AccessTyp", ListSortDirection.Ascending));
                            this.DialogDataView.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
                            this.DialogDataView.MoveCurrentToFirst();
                            this.DisplayRowCount = this.DialogDataView.Count<PasswordPin>();

                            if (this.DisplayRowCount > 0)
                            {
                                this.IsFilterContentFound = true;
                            }
                        }
                    }
                }
            }
            catch (FileLockException ex)
            {
                App.ErrorMessage(ex);
                App.Current.Shutdown(0);
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private bool DataDefaultFilter(PasswordPin rowItem)
        {
            bool found = false;

            if (rowItem == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(this.RegionCurrent?.Name) == true)
            {
                found = true;
            }

            return found;
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
