namespace PasswortNET.Views.ContentControls
{
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using ModernBaseLibrary.Collection;
    using ModernBaseLibrary.Core;
    using ModernBaseLibrary.Core.IO;
    using ModernBaseLibrary.Extension;

    using ModernUI.MVVM.Base;

    using PasswortNET.AuditTrail;
    using PasswortNET.Core;
    using PasswortNET.Core.Enums;
    using PasswortNET.DataRepository;
    using PasswortNET.Model;
    using PasswortNET.Resources;

    /// <summary>
    /// Interaktionslogik für LicenseDetailUC.xaml
    /// </summary>
    public partial class LicenseDetailUC : UserControlBase
    {
        public LicenseDetailUC(ChangeViewEventArgs args) : base(typeof(LicenseDetailUC))
        {
            this.InitializeComponent();

            this.Id = args.EntityId;
            this.RowPosition = args.RowPosition;
            this.IsNew = args.IsNew;
            this.IsCopy = args.IsCopy;

            this.ValidationErrors = new ObservableDictionary<string, string>();
            this.InitCommands();

            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Unloaded", this.OnUcUnloaded);
            WeakEventManager<UserControl, MouseWheelEventArgs>.AddHandler(this, "PreviewMouseWheel", this.OnPreviewMouseWheel);

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

        public IEnumerable<string> LicenseNameSource
        {
            get => base.GetValue<IEnumerable<string>>();
            set => base.SetValue(value);
        }

        public IEnumerable<string> CompanySource
        {
            get => base.GetValue<IEnumerable<string>>();
            set => base.SetValue(value);
        }

        public string Company
        {
            get => base.GetValue<string>();
            set => base.SetValue(value, this.CheckContent);
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

        public string LicenseName
        {
            get => base.GetValue<string>();
            set => base.SetValue(value, this.CheckContent);
        }

        public string LicenseKey
        {
            get => base.GetValue<string>();
            set => base.SetValue(value, this.CheckContent);
        }

        public Brush SelectedBackgroundColor
        {
            get => base.GetValue<Brush>();
            set => base.SetValue(value);
        }

        public byte[] Photo
        {
            get => base.GetValue<byte[]>();
            set => base.SetValue(value);
        }

        public int ChangeTrackingCount
        {
            get => base.GetValue<int>();
            set => base.SetValue(value);
        }

        /* Prüfen von Eingaben */
        public ObservableDictionary<string, string> ValidationErrors
        {
            get => base.GetValue<ObservableDictionary<string, string>>();
            set => base.SetValue(value);
        }

        public string ValidationErrorsSelected
        {
            get => base.GetValue<string>();
            set => base.SetValue(value, this.NavigationProperty);
        }
        private Guid Id { get; set; }
        private bool IsPhotoFound { get; set; }
        private long PhotoSize { get; set; } = 0;
        private string PhotoFileName { get; set; } = string.Empty;
        private bool IsNew { get; set; }
        private bool IsCopy { get; set; }
        #endregion Properties

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("BackCommand", new RelayCommand(p1 => this.BackHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("SaveDetailCommand", new RelayCommand(this.SaveDetailHandler));
            this.CmdAgg.AddOrSetCommand("AddAttachmentCommand", new RelayCommand(this.AddAttachmentHandler));
            this.CmdAgg.AddOrSetCommand("DeleteAttachmentCommand", new RelayCommand(this.DeleteAttachmentHandler, this.CanDeleteAttachmentHandler));
            this.CmdAgg.AddOrSetCommand("TrackingCommand", new RelayCommand(this.TrackingHandler));
            this.CmdAgg.AddOrSetCommand("CallWebPageCommand", new RelayCommand(this.CallWebPageHandler, this.CanCallWebPageHandler));
            this.CmdAgg.AddOrSetCommand("FromClipboardAttachmentCommand", new RelayCommand(this.FromClipboardAttachmentHandler));
        }

        #region UserControl Events
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this);
            this.RegisterValidations();
            this.LoadDataHandler();
        }

        private void OnUcUnloaded(object sender, RoutedEventArgs e)
        {
            if (this.CurrentSelectedItem != null)
            {
                PasswordPin original = PasswordPin.ToClone<PasswordPin>(this.CurrentSelectedItem);
                using (PasswordPinRepository repository = new PasswordPinRepository())
                {
                    original.ShowLast = DateTime.Now;
                    original.IsShowLast = true;
                    repository.Update(original);
                }
            }
        }

        #endregion UserControl Events

        #region Load Data
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

                    using (ChangeTrackingRepository repository = new ChangeTrackingRepository())
                    {
                        this.ChangeTrackingCount = repository.CountById(this.Id);
                    }

                    if (this.IsNew == true)
                    {
                        using (PasswordPinRepository repository = new PasswordPinRepository())
                        {
                            this.RegionSource = repository.ListByRegion();
                            this.SelectedRegion = this.RegionSource.FirstOrDefault(f => f.Name == "Alle");

                            var licenseNames = repository.List().Where(w => w.AccessTyp == AccessTyp.License).OrderBy(o => o.LicenseName).DistinctBy(d => d.LicenseName).Select(s => s.LicenseName).ToList();
                            if (licenseNames != null)
                            {
                                this.LicenseNameSource = licenseNames;
                            }

                            var companyNames = repository.List().Where(w => w.AccessTyp == AccessTyp.License).OrderBy(o => o.Company).DistinctBy(d => d.Company).Select(s => s.Company).ToList();
                            if (companyNames != null)
                            {
                                this.CompanySource = companyNames;
                            }
                        }

                        this.CurrentSelectedItem = new PasswordPin();
                        this.CurrentSelectedItem.Id = this.Id;
                        this.CurrentSelectedItem.CreatedBy = UserInfo.TS().CurrentDomainUser;
                        this.CurrentSelectedItem.CreatedOn = UserInfo.TS().CurrentTime;
                        this.CurrentSelectedItem.Id = Guid.Empty;
                        this.AccessTyp = AccessTyp.License;
                        this.SelectedBackgroundColor = ColorConverters.ConvertNameToBrush("Transparent");
                        this.Photo = EmbeddedResource.Extract("NoPicture256x226.png");
                    }
                    else
                    {
                        using (PasswordPinRepository repository = new PasswordPinRepository())
                        {
                            this.RegionSource = repository.ListByRegion();
                            this.SelectedRegion = this.RegionSource.FirstOrDefault(f => f.Name == "Alle");

                            var licenseName = repository.List().Where(w => w.AccessTyp == AccessTyp.License).OrderBy(o => o.LicenseName).DistinctBy(d => d.LicenseName).Select(s => s.LicenseName).ToList();
                            if (licenseName != null)
                            {
                                this.LicenseNameSource = licenseName;
                            }

                            var companyNames = repository.List().Where(w => w.AccessTyp == AccessTyp.License).OrderBy(o => o.Company).DistinctBy(d => d.Company).Select(s => s.Company).ToList();
                            if (companyNames != null)
                            {
                                this.CompanySource = companyNames;
                            }

                            this.CurrentSelectedItem = repository.GetById(this.Id);
                            if (this.CurrentSelectedItem != null)
                            {
                                this.AccessTyp = this.CurrentSelectedItem.AccessTyp;
                                this.Title = this.CurrentSelectedItem.Title;
                                this.Description = this.CurrentSelectedItem.Description;
                                this.ShowDescription = this.CurrentSelectedItem.ShowDescription;
                                this.Website = this.CurrentSelectedItem.Website;
                                this.SelectedSymbol = this.CurrentSelectedItem.Symbol;
                                this.Company = this.CurrentSelectedItem.Company;
                                this.LicenseName = this.CurrentSelectedItem.LicenseName;
                                this.LicenseKey = this.CurrentSelectedItem.LicenseKey;
                                this.SelectedBackgroundColor = ColorConverters.ConvertNameToBrush(this.CurrentSelectedItem.Background);
                                if (string.IsNullOrEmpty(this.CurrentSelectedItem.Region) == false)
                                {
                                    this.SelectedRegion = this.RegionSource.FirstOrDefault(f => f.Name == this.CurrentSelectedItem.Region);
                                    if (this.SelectedRegion.Background != "Transparent")
                                    {
                                        this.SelectedBackgroundColor = ColorConverters.ConvertNameToBrush(this.SelectedRegion.Background);
                                        if (this.SelectedRegion.Background != "Transparent")
                                        {
                                            this.SelectedBackgroundColor = ColorConverters.ConvertNameToBrush(this.SelectedRegion.Background);
                                        }
                                    }
                                }

                                this.CurrentSelectedItem.ModifiedBy = UserInfo.TS().CurrentDomainUser;
                                this.CurrentSelectedItem.ModifiedOn = UserInfo.TS().CurrentTime;
                            }
                        }

                        using (AttachmentRepository repository = new AttachmentRepository())
                        {
                            if (repository.ExistAttachment(this.CurrentSelectedItem.Id) == true)
                            {
                                this.Photo = repository.GetAttachmentById(this.CurrentSelectedItem.Id);
                                this.IsPhotoFound = true;
                                this.PhotoSize = this.Photo.Length;
                                if (this.PhotoSize == 0)
                                {
                                    this.Photo = EmbeddedResource.Extract("NoPicture256x226.png");
                                }
                            }
                            else
                            {
                                this.Photo = EmbeddedResource.Extract("NoPicture256x226.png");
                                this.IsPhotoFound = false;
                                this.PhotoSize = this.Photo.Length;
                            }
                        }
                        base.IsPropertyChanged = false;
                    }

                    StatusbarMain.Statusbar.SetNotification($"Bereit: {objectRuntime.ResultMilliseconds()}ms");
                }
            }
            catch (FileLockException ex)
            {
                App.ErrorMessage(ex);
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }
        #endregion Load Data

        #region Command Handler
        private void BackHandler(object p1)
        {
            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                EntityId = this.Id,
                MenuButton = FunctionButtons.MainOverview,
            });
        }

        private void SaveDetailHandler(object commandParam)
        {
            try
            {
                PasswordPin original = PasswordPin.ToClone<PasswordPin>(this.CurrentSelectedItem);
                this.CurrentSelectedItem.AccessTyp = this.AccessTyp;
                this.CurrentSelectedItem.Title = this.Title;
                this.CurrentSelectedItem.Description = this.Description;
                this.CurrentSelectedItem.ShowDescription = this.ShowDescription;
                this.CurrentSelectedItem.Website = this.Website;
                this.CurrentSelectedItem.Symbol = this.SelectedSymbol;
                this.CurrentSelectedItem.Company = this.Company;
                this.CurrentSelectedItem.LicenseName = this.LicenseName;
                this.CurrentSelectedItem.LicenseKey = this.LicenseKey;
                this.CurrentSelectedItem.Background = ColorConverters.ConvertBrushToName(this.CBColor.SelectedColor);
                this.CurrentSelectedItem.Region = this.SelectedRegion?.Name;
                using (PasswordPinRepository repository = new PasswordPinRepository())
                {
                    if (this.CurrentSelectedItem.Id == Guid.Empty && this.IsCopy == false)
                    {
                        this.CurrentSelectedItem.Id = Guid.NewGuid();
                        this.CurrentSelectedItem.CreatedBy = UserInfo.TS().CurrentUser;
                        this.CurrentSelectedItem.CreatedOn = UserInfo.TS().CurrentTime;
                        this.CurrentSelectedItem.ModifiedBy = UserInfo.TS().CurrentUser;
                        this.CurrentSelectedItem.ModifiedOn = UserInfo.TS().CurrentTime;
                        OperationResult<AuditTrailResult> auditTrailresult = AuditTrail.Create(this.CurrentSelectedItem, null);

                        repository.Add(this.CurrentSelectedItem, auditTrailresult, this.Photo);

                        this.ChangedContent(false);
                    }
                    else if (this.CurrentSelectedItem.Id != Guid.Empty && this.IsCopy == false)
                    {
                        this.CurrentSelectedItem.CreatedBy = UserInfo.TS().CurrentUser;
                        this.CurrentSelectedItem.CreatedOn = UserInfo.TS().CurrentTime;
                        this.CurrentSelectedItem.ModifiedBy = UserInfo.TS().CurrentUser;
                        this.CurrentSelectedItem.ModifiedOn = UserInfo.TS().CurrentTime;
                        OperationResult<AuditTrailResult> auditTrailresult = AuditTrail.Create(original, this.CurrentSelectedItem);

                        repository.Update(this.CurrentSelectedItem, auditTrailresult, this.Photo);
                        this.ChangedContent(false);
                    }
                    else if (this.CurrentSelectedItem.Id != Guid.Empty && this.IsCopy == true)
                    {
                        this.CurrentSelectedItem.Id = Guid.NewGuid();
                        this.CurrentSelectedItem.CreatedBy = UserInfo.TS().CurrentUser;
                        this.CurrentSelectedItem.CreatedOn = UserInfo.TS().CurrentTime;
                        this.CurrentSelectedItem.ModifiedBy = UserInfo.TS().CurrentUser;
                        this.CurrentSelectedItem.ModifiedOn = UserInfo.TS().CurrentTime;
                        OperationResult<AuditTrailResult> auditTrailresult = AuditTrail.Create(this.CurrentSelectedItem, null);

                        repository.Add(this.CurrentSelectedItem, auditTrailresult, this.Photo);
                        this.ChangedContent(false);
                    }
                }

                this.BackHandler(null);
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private void AddAttachmentHandler(object commandParam)
        {
            FileFilter fileFilter = new FileFilter();
            fileFilter.AddFilter("Alle Dateien", "*", true);
            fileFilter.AddFilter("Images", "*.png;*.jpg", false);


            using (OpenFileDialogEx openFile = new OpenFileDialogEx())
            {
                openFile.Title = "Bild einfügen";
                openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
                openFile.FileFilter = fileFilter;
                openFile.OpenDialog();
                if (string.IsNullOrEmpty(openFile.FileName) == false)
                {
                    this.PhotoFileName = openFile.FileName;
                }
            }

            if (string.IsNullOrEmpty(this.PhotoFileName) == true)
            {
                return;
            }

            this.Photo = System.IO.File.ReadAllBytes(this.PhotoFileName);
        }

        private void TrackingHandler(object commandParam)
        {
            try
            {
                base.EventAgg.Publish<ChangeViewEventArgs>(
                    new ChangeViewEventArgs
                    {
                        Sender = this.GetType().Name,
                        EntityId = this.Id,
                        FromPage = FunctionButtons.MainOverview,
                        MenuButton = FunctionButtons.AuditTrail,
                    });
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private bool CanCallWebPageHandler(object commandParam)
        {
            if (this.CurrentSelectedItem == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(this.Website?.Trim()) == true)
            {
                return false;
            }
            else
            {
                return this.Website.Trim().IsValidUrl();
            }
        }

        private void CallWebPageHandler(object commandParam)
        {
            string url = this.Website?.Trim();
            try
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        private bool CanDeleteAttachmentHandler(object commandParam)
        {
            return this.IsPhotoFound;
        }

        private void DeleteAttachmentHandler(object commandParam)
        {
            using (AttachmentRepository repository = new AttachmentRepository())
            {
                repository.DeleteAttachment(this.CurrentSelectedItem.Id);
                this.Photo = EmbeddedResource.Extract("NoPicture256x226.png");
                this.IsPhotoFound = false;
                this.PhotoSize = this.Photo.Length;
            }
        }

        private void FromClipboardAttachmentHandler(object commandParam)
        {
            BitmapSource clipboardImage = Clipboard.GetImage();
            if (clipboardImage != null)
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.QualityLevel = 100;
                using (MemoryStream stream = new MemoryStream())
                {
                    encoder.Frames.Add(BitmapFrame.Create(clipboardImage));
                    encoder.Save(stream);
                    this.Photo = stream.ToArray();
                    this.IsPhotoFound = true;
                    this.PhotoSize = this.Photo.Length;
                    stream.Close();
                }
            }
        }
        #endregion Command Handler

        #region Register Validations
        private void RegisterValidations()
        {
            this.ValidationRules.Add(nameof(this.Title), () =>
            {
                return InputValidation<LicenseDetailUC>.This(this).NotEmpty(x => x.Title, "Titel");
            });

            this.ValidationRules.Add(nameof(this.Company), () =>
            {
                return InputValidation<LicenseDetailUC>.This(this).NotEmpty(x => x.Company, "Unternehmen");
            });

            this.ValidationRules.Add(nameof(this.LicenseName), () =>
            {
                return InputValidation<LicenseDetailUC>.This(this).NotEmpty(x => x.LicenseName, "Lizenz für ...");
            });

            this.ValidationRules.Add(nameof(this.LicenseKey), () =>
            {
                return InputValidation<LicenseDetailUC>.This(this).NotEmpty(x => x.LicenseKey, "Lizenzschlüssel");
            });
        }
        #endregion Register Validations

        #region Helper Functions
        private void CheckContent<T>(T value, string propertyName)
        {
            if (this.CurrentSelectedItem != null)
            {
                PropertyInfo propInfo = this.CurrentSelectedItem.GetType().GetProperties().FirstOrDefault(p => p.Name == propertyName);
                if (propInfo == null)
                {
                    this.ChangedContent(true);
                    return;
                }
                else
                {
                    var propValue = propInfo.GetValue(this.CurrentSelectedItem);
                    if (propValue != null)
                    {
                        if (propValue.Equals(value) == false)
                        {
                            this.ChangedContent(true);
                        }
                    }
                    else
                    {
                        this.ChangedContent(true);
                    }
                }
            }

            this.ValidationErrors.Clear();
            foreach (string property in this.GetProperties(this))
            {
                Func<Result<string>> function = null;
                if (this.ValidationRules.TryGetValue(property, out function) == true)
                {
                    Result<string> ruleText = this.DoValidation(function, property);
                    if (string.IsNullOrEmpty(ruleText.Value) == false)
                    {
                        this.ValidationErrors.Add(property, ruleText.Value);
                    }
                }
            }
        }

        public override void ChangedContent(bool isPropertyChanged = false)
        {
            this.IsPropertyChanged = isPropertyChanged;
            if (isPropertyChanged == true)
            {
                StatusbarMain.Statusbar.SetNotification($"Geändert");
            }
            else
            {
                StatusbarMain.Statusbar.SetNotification($"Bereit");
            }
        }

        private void NavigationProperty<T>(T value, string propertyName)
        {
            if (value is string txt)
            {
                this.TxtTitel.Background = Brushes.Transparent;
                this.TxtLicenseName.Background = Brushes.Transparent;
                this.TxtLicenseKey.Background = Brushes.Transparent;

                if (txt == nameof(this.Title))
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                    {
                        this.TxtTitel.Focus();
                        this.TxtTitel.Background = Brushes.Coral;
                    }));
                }
                else if (txt == nameof(this.LicenseName))
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                    {
                        this.TxtLicenseName.Focus();
                        this.TxtLicenseName.Background = Brushes.Coral;
                    }));
                }
                else if (txt == nameof(this.LicenseKey))
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                    {
                        this.TxtLicenseKey.Focus();
                        this.TxtLicenseKey.Background = Brushes.Coral;
                    }));
                }
                else
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                    {
                        this.TxtTitel.Focus();
                        this.TxtTitel.Background = Brushes.Transparent;
                    }));
                }
            }
        }
        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
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
                    if (this.Scalefactor.ScaleX > 1.35)
                    {
                        this.Scalefactor.ScaleX = this.Scalefactor.ScaleX - 0.25;
                        this.Scalefactor.ScaleY = this.Scalefactor.ScaleY - 0.25;
                    }
                }
            }
        }
        #endregion Helper Functions
    }
}
