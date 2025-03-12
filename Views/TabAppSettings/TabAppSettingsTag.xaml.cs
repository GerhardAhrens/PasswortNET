namespace PasswortNET.Views.TabAppSettings
{
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using ModernBaseLibrary.Core;
    using ModernBaseLibrary.Extension;

    using ModernIU.Controls;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.DataRepository;
    using PasswortNET.Model;

    /// <summary>
    /// Interaktionslogik für TabAppSettingsTag.xaml
    /// </summary>
    public partial class TabAppSettingsTag : UserControlBase
    {
        private INotificationService notificationService = new NotificationService();

        public TabAppSettingsTag()
        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            this.InitCommands();
            this.DataContext = this;
        }

        #region Get/Set Properties
        public IEnumerable<Region> RegionSource
        {
            get => base.GetValue<IEnumerable<Region>>();
            set => base.SetValue(value);
        }

        public int RegionCount
        {
            get => base.GetValue<int>();
            set => base.SetValue(value);
        }

        public Region CurrentSelectedItem
        {
            get => base.GetValue<Region>();
            set => base.SetValue(value, this.CurrentSelected);
        }

        public string TagName
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public int SymbolSelected
        {
            get => base.GetValue<int>();
            set => base.SetValue(value);
        }

        public int BackgroundColorSelected
        {
            get => base.GetValue<int>();
            set => base.SetValue(value);
        }
        #endregion Get/Set Properties

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("AddCommand", new RelayCommand<Region>(p1 => this.AddHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("UpdateCommand", new RelayCommand<Region>(p1 => this.UpdateHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("DeleteTagCommand", new RelayCommand<Region>(p1 => this.DeleteHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("SelectedItemChangedCommand", new RelayCommand<Region>(p1 => this.SelectedItemChangedHandler(p1), p2 => true));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.IsUCLoaded = true;

            if ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue) == false)
            {
                this.LoadDataHandler();
            }
        }

        #region Load and Filter Data
        private void LoadDataHandler(bool isRefresh = false)
        {
            if (this.IsUCLoaded == false)
            {
                return;
            }

            try
            {
                using (RegionRepository repository = new RegionRepository())
                {
                    if (repository == null)
                    {
                        return;
                    }

                    this.RegionCount = repository.Count();
                    this.RegionSource = repository.List().ToList().OrderBy(o => o.Name);
                }

                if (this.lbRegion.HasItems == true)
                {
                    this.lbRegion.Focus();
                    this.lbRegion.SelectedIndex = 0;
                    this.lbRegion.ScrollIntoView(this.lbRegion.Items[this.lbRegion.SelectedIndex]);
                    this.lbRegion.Focus();
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }
        #endregion Load and Filter Data

        private void AddHandler(Region commandParam)
        {
            Region tagName = null;

            try
            {
                if (string.IsNullOrEmpty(this.TagName) == false)
                {
                    if (this.RegionSource.Count() == 0 || this.RegionSource.Any(a => a.Name != this.TagName))
                    {
                        using (RegionRepository repository = new RegionRepository())
                        {
                            Region r = new Region();
                            r.Name = this.TagName;
                            r.CreatedBy = UserInfo.TS().CurrentUser;
                            r.CreatedOn = UserInfo.TS().CurrentTime;
                            r.Background = this.ConvertIndexToColorName(this.BackgroundColorSelected);
                            r.Id = Guid.NewGuid();
                            repository.Add(r);
                            tagName = r;
                        }

                        this.LoadDataHandler(true);
                        int tagIndex = -1;
                        foreach (Region r in this.lbRegion.Items)
                        {
                            tagIndex++;
                            if (r.Name == tagName.Name)
                            {
                                break;
                            }
                        }

                        this.lbRegion.SelectedIndex = tagIndex;
                        this.lbRegion.ScrollIntoView(this.lbRegion.Items[tagIndex]);
                    }
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private void UpdateHandler(Region commandParam)
        {
            Region tagName = null;

            try
            {
                if (string.IsNullOrEmpty(this.TagName) == false)
                {
                    using (RegionRepository repository = new RegionRepository())
                    {
                        if (this.CurrentSelectedItem != null && this.RegionSource.Any(a => a.Id == this.CurrentSelectedItem.Id))
                        {
                            Region r = commandParam;
                            r.Name = this.TagName;
                            r.Background = this.ConvertIndexToColorName(this.BackgroundColorSelected);
                            r.ModifiedBy = UserInfo.TS().CurrentUser;
                            r.ModifiedOn = UserInfo.TS().CurrentTime;
                            repository.Update(r);
                            tagName = r;
                        }
                        else
                        {
                            Region r = new Region();
                            r.Name = this.TagName;
                            r.Background = this.ConvertIndexToColorName(this.BackgroundColorSelected);
                            r.CreatedBy = UserInfo.TS().CurrentUser;
                            r.CreatedOn = UserInfo.TS().CurrentTime;
                            r.Id = Guid.NewGuid();
                            repository.Add(r);
                            tagName = r;
                        }
                    }

                    this.LoadDataHandler(true);

                    int tagIndex = -1;
                    foreach (Region r in this.lbRegion.Items)
                    {
                        tagIndex++;
                        if (r.Name == tagName.Name)
                        {
                            break;
                        }
                    }

                    this.lbRegion.SelectedIndex = tagIndex;
                    this.lbRegion.ScrollIntoView(this.lbRegion.Items[tagIndex]);
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private void DeleteHandler(Region commandParam)
        {
            try
            {
                NotificationBoxButton deleteYN = this.notificationService.DeleteRegionItem($"{commandParam.Name}");
                if (deleteYN == NotificationBoxButton.Yes)
                {
                    string name = commandParam.Name.ToUpper();
                    if (name.In("ALLE", "ZULETZT GESEHEN") == true)
                    {
                        this.notificationService.DefaultValueNotDelete();
                        return;
                    }

                    using (RegionRepository repository = new RegionRepository())
                    {
                        repository.Delete(commandParam.Id);
                    }

                    this.LoadDataHandler(true);

                    this.lbRegion.SelectedIndex = 0;
                    this.lbRegion.ScrollIntoView(this.lbRegion.Items[0]);
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private void SelectedItemChangedHandler(Region commandParam)
        {
            try
            {
                this.CurrentSelectedItem = commandParam;
                this.TagName = commandParam.Name;
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private void CurrentSelected(Region region, string arg2)
        {
            this.BackgroundColorSelected = this.ConvertColorNameToIndex(region.Background);
        }

        private int ConvertColorNameToIndex(string colorName)
        {
            int indexColor = -1;
            PropertyInfo[] colors = typeof(Brushes).GetProperties();

            if (string.IsNullOrEmpty(colorName) == true)
            {
                indexColor = Array.FindIndex(colors, x => x.Name.ToUpper() == "TRANSPARENT");
            }
            else
            {
                indexColor = Array.FindIndex(colors, x => x.Name.ToUpper() == colorName.ToUpper());
            }

            if (indexColor == -1)
            {
                indexColor = Array.FindIndex(colors, x => x.Name.ToUpper() == "TRANSPARENT");
            }

            return indexColor;
        }

        private string ConvertIndexToColorName(int colorIndex)
        {
            PropertyInfo[] colors = typeof(Brushes).GetProperties();
            string colorName = colors[colorIndex].Name;
            return colorName;
        }
    }
}
