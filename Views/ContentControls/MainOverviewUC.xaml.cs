﻿namespace PasswortNET.Views.ContentControls
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
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            this.InitCommands();
            this.DataContext = this;
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

        private AccessTyp AccessTypState { get; set; }

        #endregion Properties

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("LogoffCommand", new RelayCommand(p1 => this.LogoffHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("ListViewCommand", new RelayCommand(p1 => ListViewContextMenu(p1.ToString()), p2 => true));
            this.CmdAgg.AddOrSetCommand("EditEntryCommand", new RelayCommand(p1 => this.EditEntryHandler(p1), p2 => this.CanEditEntryHandler()));
            this.CmdAgg.AddOrSetCommand("AddEntryCommand", new RelayCommand(p1 => this.AddEntryHandler(p1), p2 => true));
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
                                this.DisplayRowCount = this.DialogDataView.Count<PasswordPin>();

                                if (this.DisplayRowCount > 0)
                                {
                                    this.IsFilterContentFound = true;
                                }
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

        private bool CanEditEntryHandler()
        {
            return this.DisplayRowCount == 0 ? false : true;
        }

        private void SelectAccessStateHandler(object args)
        {
            this.AccessTypState = (AccessTyp)args;
        }

        #region Command Handler Methodes
        private void EditEntryHandler(object commandArgs)
        {

        }

        private void AddEntryHandler(object p1)
        {
        }

        private bool CanDeleteEntryHandler()
        {
            return this.DisplayRowCount == 0 ? false : true;
        }

        private void DeleteEntryHandler()
        {
        }

        private bool CanCopyEntryHandler()
        {
            return this.DisplayRowCount == 0 ? false : true;
        }


        private void CopyEntryHandler()
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
