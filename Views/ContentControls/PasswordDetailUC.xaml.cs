namespace PasswortNET.Views.ContentControls
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    using ModernBaseLibrary.Core;

    using ModernIU.Controls;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.Core.Enums;
    using PasswortNET.DataRepository;
    using PasswortNET.Model;

    /// <summary>
    /// Interaktionslogik für PasswordDetailUC.xaml
    /// </summary>
    public partial class PasswordDetailUC : UserControlBase
    {
        private INotificationService notificationService = new NotificationService();

        public PasswordDetailUC(ChangeViewEventArgs args) : base(typeof(PasswordDetailUC))
        {
            this.InitializeComponent();

            this.Id = args.EntityId;
            this.RowPosition = args.RowPosition;
            this.IsEntryNew = args.IsNew;

            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            this.InitCommands();
            this.DataContext = this;
        }

        #region Properties
        public IEnumerable<Region> RegionSource
        {
            get => base.GetValue<IEnumerable<Region>>();
            set => base.SetValue(value);
        }

        public Region SelectedRegion
        {
            get => base.GetValue<Region>();
            set => base.SetValue(value);
        }

        public Dictionary<int, string> SymbolSource
        {
            get => base.GetValue<Dictionary<int, string>>();
            set => base.SetValue(value);
        }

        public int SelectedSymbol
        {
            get => base.GetValue<int>();
            set => base.SetValue(value);
        }

        public PasswordPin CurrentSelectedItem
        {
            get => base.GetValue<PasswordPin>();
            set => base.SetValue(value);
        }

        public AccessTyp AccessTyp
        {
            get => base.GetValue<AccessTyp>();
            set => base.SetValue(value);
        }

        public string Title
        {
            get => base.GetValue<string>();
            set => base.SetValue(value, this.CheckContent);
        }

        public string Description
        {
            get => base.GetValue<string>();
            set => base.SetValue(value, this.CheckContent);
        }

        public bool ShowDescription
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value, this.CheckContent);
        }

        public string Website
        {
            get => base.GetValue<string>();
            set => base.SetValue(value, this.CheckContent);
        }

        public string Password
        {
            get => base.GetValue<string>();
            set => base.SetValue(value, this.CheckContent);
        }

        public Brush BackgroundColorSelected
        {
            get => base.GetValue<Brush>();
            set => base.SetValue(value);
        }

        private Guid Id { get; set; }
        private int RowPosition { get; set; }

        public bool IsEntryNew { get; set; }

        #endregion Properties

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("BackCommand", new RelayCommand(p1 => this.BackHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("SaveDetailCommand", new RelayCommand(p1 => this.SaveDetailHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("AddAttachmentCommand", new RelayCommand(p1 => this.AddAttachmentHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("TrackingCommand", new RelayCommand(p1 => this.TrackingHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("PasswordGeneratorCommand", new RelayCommand(p1 => this.PasswordGeneratorHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("CallWebPageCommand", new RelayCommand(p1 => this.CallWebPageHandler(p1), p2 => true));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this);
            this.LoadDataHandler();
        }

        private void LoadDataHandler()
        {
            try
            {
                using (ObjectRuntime objectRuntime = new ObjectRuntime())
                {
                    using (SymbolsList sl = new SymbolsList())
                    {
                        this.SymbolSource = sl.GetSymbols();
                    }

                    if (this.IsEntryNew == true)
                    {
                        this.CurrentSelectedItem = new PasswordPin();
                        this.CurrentSelectedItem.Id = this.Id;
                        this.CurrentSelectedItem.CreatedBy = UserInfo.TS().CurrentDomainUser;
                        this.CurrentSelectedItem.CreatedOn = UserInfo.TS().CurrentTime;
                    }
                    else
                    {
                        using (PasswordPinRepository repository = new PasswordPinRepository())
                        {
                            this.RegionSource = repository.ListByRegion();
                            this.SelectedRegion = this.RegionSource.FirstOrDefault(f => f.Name == "Alle");

                            this.CurrentSelectedItem = repository.GetById(this.Id);
                            if (this.CurrentSelectedItem != null)
                            {
                                this.AccessTyp = this.CurrentSelectedItem.AccessTyp;
                                this.Title = this.CurrentSelectedItem.Title;
                                this.Description = this.CurrentSelectedItem.Description;
                                this.ShowDescription = this.CurrentSelectedItem.ShowDescription;
                                this.Website = this.CurrentSelectedItem.Website;
                                this.SelectedSymbol = this.CurrentSelectedItem.Symbol;
                                this.Password = this.CurrentSelectedItem.Passwort;
                                this.BackgroundColorSelected = this.ConvertNameToBrush(this.CurrentSelectedItem.Background);
                                if (string.IsNullOrEmpty(this.CurrentSelectedItem.Region) == false)
                                {
                                    this.SelectedRegion = this.RegionSource.FirstOrDefault(f => f.Name == this.CurrentSelectedItem.Region);
                                }
                            }
                        }
                    }

                    StatusbarMain.Statusbar.SetNotification($"Bereit: {objectRuntime.ResultMilliseconds()}ms");
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

        private void BackHandler(object p1)
        {
            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = FunctionButtons.MainOverview,
                RowPosition = this.RowPosition,
            });
        }

        private void SaveDetailHandler(object p1)
        {
            this.notificationService.FeaturesNotFound2("Speichern");
        }

        private void AddAttachmentHandler(object p1)
        {
            this.notificationService.FeaturesNotFound2("Anhang");
        }

        private void TrackingHandler(object p1)
        {
            this.notificationService.FeaturesNotFound2("Tracking");
        }

        private void CallWebPageHandler(object p1)
        {
            this.notificationService.FeaturesNotFound2("Webseite");
        }

        private void PasswordGeneratorHandler(object p1)
        {
            this.notificationService.FeaturesNotFound2("Passwortgenerator");
        }

        private int ConvertColorNameToIndex(string colorName)
        {
            PropertyInfo[] colors = typeof(Brushes).GetProperties();
            int indexColor = Array.FindIndex(colors, x => x.Name.ToUpper() == colorName.ToUpper());
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

        private Brush ConvertIndexToBrush(int colorIndex)
        {
            PropertyInfo[] colors = typeof(Brushes).GetProperties();
            string colorName = colors[colorIndex].Name;
            Color col = (Color)ColorConverter.ConvertFromString(colorName);
            Brush brushColor = new SolidColorBrush(col);
            return brushColor;
        }

        private Brush ConvertNameToBrush(string colorName)
        {
            Color col = (Color)ColorConverter.ConvertFromString(colorName);
            Brush brushColor = new SolidColorBrush(col);
            return brushColor;
        }

        private void CheckContent(string value, string propertyName)
        {
            if (this.CurrentSelectedItem == null)
            {
                return;
            }

            PropertyInfo propInfo = this.CurrentSelectedItem.GetType().GetProperties().FirstOrDefault(p => p.Name == propertyName);
            if (propInfo == null)
            {
                base.IsPropertyChanged = false;
                return;
            }

            var propValue = propInfo.GetValue(this.CurrentSelectedItem);
            if (propValue == null)
            {
                base.IsPropertyChanged = true;
                return;
            }

            if (propValue.Equals(value) == false)
            {
                base.IsPropertyChanged = true;
            }
        }

        private void CheckContent(bool value, string propertyName)
        {
            if (this.CurrentSelectedItem == null)
            {
                return;
            }

            PropertyInfo propInfo = this.CurrentSelectedItem.GetType().GetProperties().FirstOrDefault(p => p.Name == propertyName);
            if (propInfo == null)
            {
                base.IsPropertyChanged = false;
                return;
            }

            var propValue = propInfo.GetValue(this.CurrentSelectedItem);
            if (propValue == null)
            {
                base.IsPropertyChanged = true;
                return;
            }

            if (propValue.Equals(value) == false)
            {
                base.IsPropertyChanged = true;
            }
        }
    }
}
