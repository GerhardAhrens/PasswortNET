namespace PasswortNET.Views.ContentControls
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

    using ModernBaseLibrary.Core;

    using ModernBaseLibrary.Extension;

    using ModernIU.Controls;

    using ModernUI.MVVM.Base;

    using PasswortNET.AuditTrail;
    using PasswortNET.Core;
    using PasswortNET.Core.Enums;
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
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            base.EventAgg.Subscribe<WorkEventArgs>(this.WorkEntryHandler);
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<ListView, MouseWheelEventArgs>.AddHandler(this.lvwMain, "PreviewMouseWheel", this.OnLvwPreviewMouseWheel);
            this.Unloaded += OnUcUnloaded;
            this.InitCommands();
            this.DataContext = this;
        }

        private void OnUcUnloaded(object sender, RoutedEventArgs e)
        {
            this.CustomView = null;
        }

        #region Properties
        public string FilterDefaultSearch
        {
            get => base.GetValue<string>();
            set => base.SetValue(value, this.RefreshDefaultFilter);
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
            set => base.SetValue(value, this.RefreshDefaultFilter);
        }

        public bool IsFilterContentFound
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value);
        }

        public bool IsCheckStateAll
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value);
        }

        public bool IsCheckStateWebsite
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value);
        }

        public bool IsCheckStatePin
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value);
        }

        public bool IsCheckStatePassword
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value);
        }

        public bool IsCheckStateLicense
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
            this.CmdAgg.AddOrSetCommand("DeleteEntryCommand", new RelayCommand(p1 => this.DeleteEntryHandler(), p2 => this.CanDeleteEntryHandler()));
            this.CmdAgg.AddOrSetCommand("CopyEntryCommand", new RelayCommand(p1 => this.CopyEntryHandler(), p2 => this.CanCopyEntryHandler()));
            this.CmdAgg.AddOrSetCommand("SelectAccessStateCommand", new RelayCommand(p1 => this.SelectAccessStateHandler(p1), p2 => true));
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
                using (ObjectRuntime objectRuntime = new ObjectRuntime())
                {
                    using (PasswordPinRepository repository = new PasswordPinRepository())
                    {
                        this.RegionSource = repository.ListByRegion().OrderBy(o => o.ItemSorting);

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
                                this.lvwMain.SelectedIndex = 0;
                                var item = this.DialogDataView.CurrentItem;
                                this.lvwMain.ScrollIntoView(item);
                                this.DisplayRowCount = this.DialogDataView.Count<PasswordPin>();

                                this.IsFilterContentFound = this.DisplayRowCount > 0 ? true : false;
                            }
                        }
                    }

                    StatusbarMain.Statusbar.SetNotification($"Bereit: {objectRuntime.ResultMilliseconds()}ms; Anzahl: {this.DisplayRowCount}");

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
            else
            {
                if (this.RegionCurrent?.Name.ToLower() != "alle")
                {
                    if (rowItem.Region?.ToLower() == this.RegionCurrent?.Name.ToLower())
                    {
                        found = true;
                    }
                    else
                    {
                        if (this.RegionCurrent?.Name.ToLower() == "zuletzt gesehen")
                        {
                            if (rowItem.IsShowLast == true)
                            {
                                found = true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else if (this.RegionCurrent?.Name.ToLower() == "zuletzt gesehen")
                {
                    if (rowItem.IsShowLast == true)
                    {
                        found = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    found = true;
                }
            }

            if (this.AccessTypState == AccessTyp.Website)
            {
                if (rowItem.AccessTyp == AccessTyp.Website)
                {
                    found = true;
                }
                else
                {
                    found = false;
                }
            }
            else if (this.AccessTypState == AccessTyp.All)
            {
                found = true;
            }
            else if (this.AccessTypState == AccessTyp.Pin)
            {
                if (rowItem.AccessTyp == AccessTyp.Pin)
                {
                    found = true;
                }
                else
                {
                    found = false;
                }
            }
            else if (this.AccessTypState == AccessTyp.Passwort)
            {
                if (rowItem.AccessTyp == AccessTyp.Passwort)
                {
                    found = true;
                }
                else
                {
                    found = false;
                }
            }
            else if (this.AccessTypState == AccessTyp.License)
            {
                if (rowItem.AccessTyp == AccessTyp.License)
                {
                    found = true;
                }
                else
                {
                    found = false;
                }
            }

            string textFilterString = (this.FilterDefaultSearch ?? string.Empty).ToUpper();
            if (string.IsNullOrEmpty(textFilterString) == false)
            {
                string fullRow = rowItem.ToSearchFilter.ToUpper();
                if (string.IsNullOrEmpty(fullRow) == true)
                {
                    return true;
                }

                string[] words = textFilterString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string word in words.AsParallel<string>())
                {
                    found = fullRow.Contains(word);

                    if (found == false)
                    {
                        return false;
                    }
                }
            }

            return found;
        }
        #endregion Load and Filter Data

        private void ListViewContextMenu(string menuItem)
        {
            this.ChangeView(menuItem);
        }

        private void RefreshDefaultFilter(string value, string propertyName)
        {
            if (value != null)
            {
                this.DialogDataView.Refresh();
                this.DisplayRowCount = this.DialogDataView.Cast<PasswordPin>().Count();
                this.DialogDataView.MoveCurrentToFirst();

                if (this.DisplayRowCount > 0)
                {
                    this.IsFilterContentFound = true;
                }
                else
                {
                    this.IsFilterContentFound = false;
                }

                StatusbarMain.Statusbar.SetNotification($"Bereit: Anzahl: {this.DisplayRowCount}");
            }
        }

        private void RefreshDefaultFilter(Region value, string propertyName)
        {
            if (value != null)
            {
                this.DialogDataView.Refresh();
                this.DisplayRowCount = this.DialogDataView.Cast<PasswordPin>().Count();
                this.DialogDataView.MoveCurrentToFirst();

                if (this.DisplayRowCount > 0)
                {
                    this.IsFilterContentFound = true;
                }
                else
                {
                    this.IsFilterContentFound = false;
                }

                StatusbarMain.Statusbar.SetNotification($"Bereit: Anzahl: {this.DisplayRowCount}");
            }
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

        private void SelectAccessStateHandler(object args)
        {
            this.AccessTypState = (AccessTyp)args;

            if (this.AccessTypState == AccessTyp.Pin)
            {
                this.IsCheckStatePin = true;
                this.IsCheckStatePassword = false;
                this.IsCheckStateWebsite = false;
                this.IsCheckStateLicense = false;
                this.IsCheckStateAll = false;
            }
            else if (this.AccessTypState == AccessTyp.All)
            {
                this.IsCheckStatePin = false;
                this.IsCheckStatePassword = false;
                this.IsCheckStateWebsite = false;
                this.IsCheckStateLicense = false;
                this.IsCheckStateAll = true;
            }
            else if (this.AccessTypState == AccessTyp.Passwort)
            {
                this.IsCheckStatePin = false;
                this.IsCheckStatePassword = true;
                this.IsCheckStateWebsite = false;
                this.IsCheckStateLicense = false;
                this.IsCheckStateAll = false;
            }
            else if (this.AccessTypState == AccessTyp.Website)
            {
                this.IsCheckStatePin = false;
                this.IsCheckStatePassword = false;
                this.IsCheckStateWebsite = true;
                this.IsCheckStateLicense = false;
                this.IsCheckStateAll = false;
            }
            else if (this.AccessTypState == AccessTyp.License)
            {
                this.IsCheckStatePin = false;
                this.IsCheckStatePassword = false;
                this.IsCheckStateWebsite = false;
                this.IsCheckStateLicense = true;
                this.IsCheckStateAll = false;
            }

            this.LoadDataHandler();
        }

        private void OnLvwPreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (sender != null)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) == true)
                {
                    if (e.Delta > 0)
                    {
                        if (this.Scalefactor.ScaleX <= 2.0)
                        {
                            this.Scalefactor.ScaleX = this.Scalefactor.ScaleX + 0.25;
                            this.Scalefactor.ScaleY = this.Scalefactor.ScaleY + 0.25;
                        }
                    }

                    if (e.Delta < 0)
                    {
                        if (this.Scalefactor.ScaleX > 0.75)
                        {
                            this.Scalefactor.ScaleX = this.Scalefactor.ScaleX - 0.25;
                            this.Scalefactor.ScaleY = this.Scalefactor.ScaleY - 0.25;
                        }
                    }
                }
                else
                {
                    e.Handled = true;

                    var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                    e2.RoutedEvent = ListView.MouseWheelEvent;
                    e2.Source = e.Source;

                    this.lvwMain.RaiseEvent(e2);
                }
            }
        }

        #region Command Handler Methodes
        private bool CanEditEntryHandler()
        {
            return this.DisplayRowCount == 0 ? false : true;
        }

        private void EditEntryHandler(object commandArgs)
        {
            if (this.CurrentSelectedItem == null)
            {
                notificationService.NoRowSelected();
                return;
            }

            AccessTyp accessTyp = ((PasswordPin)this.CurrentSelectedItem).AccessTyp;
            Guid id = this.CurrentSelectedItem.Id;

            if (accessTyp.In(AccessTyp.Website,AccessTyp.Passwort, AccessTyp.Pin) == true)
            {
            }
            else
            {
            }
        }

        private bool CanDeleteEntryHandler()
        {
            return this.DisplayRowCount == 0 ? false : true;
        }

        private void DeleteEntryHandler()
        {
            if (this.CurrentSelectedItem == null)
            {
                notificationService.NoRowSelected();
                return;
            }

            Guid id = this.CurrentSelectedItem.Id;

            NotificationBoxButton result = this.notificationService.DeleteSelectedRow(this.CurrentSelectedItem.Title);
            if (result == NotificationBoxButton.Yes)
            {
                using (PasswordPinRepository repository = new PasswordPinRepository())
                {
                    OperationResult<AuditTrailResult> auditTrailresult = AuditTrail.Create(null, this.CurrentSelectedItem);

                    repository.Delete(id, auditTrailresult);
                }

                this.LoadDataHandler();
            }
        }

        private bool CanCopyEntryHandler()
        {
            return this.DisplayRowCount == 0 ? false : true;
        }


        private void CopyEntryHandler()
        {
            if (this.CurrentSelectedItem == null)
            {
                notificationService.NoRowSelected();
                return;
            }

            AccessTyp accessTyp = ((PasswordPin)this.CurrentSelectedItem).AccessTyp;
            Guid id = this.CurrentSelectedItem.Id;

            NotificationBoxButton result = this.notificationService.CopySelectedRow(this.CurrentSelectedItem.Title);
            if (result == NotificationBoxButton.Yes)
            {
                if (accessTyp.In(AccessTyp.Website, AccessTyp.Passwort, AccessTyp.Pin) == true)
                {
                }
                else
                {
                }
            }
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

        #region Handler zum dem EventAggregator
        private void WorkEntryHandler(WorkEventArgs args)
        {
            this.notificationService.FeaturesNotFound(args.AccessTyp.ToString());

            this.LoadDataHandler();
        }
        #endregion Handler zum dem EventAggregator
    }
}
