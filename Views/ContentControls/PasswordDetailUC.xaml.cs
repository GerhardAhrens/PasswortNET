namespace PasswortNET.Views.ContentControls
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

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
            this.IsNew = args.IsNew;
            this.IsCopy = args.IsCopy;

            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Unloaded", this.OnUcUnloaded);
            this.InitCommands();
            this.DataContext = this;
        }

        private void OnUcUnloaded(object sender, RoutedEventArgs e)
        {
            PasswordPin original = PasswordPin.ToClone<PasswordPin>(this.CurrentSelectedItem);
            using (PasswordPinRepository repository = new PasswordPinRepository())
            {
                original.ShowLast = DateTime.Now;
                original.IsShowLast = true;
                repository.Update(original);
            }
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
            this.CmdAgg.AddOrSetCommand("SaveDetailCommand", new RelayCommand(p1 => this.SaveDetailHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("AddAttachmentCommand", new RelayCommand(p1 => this.AddAttachmentHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("TrackingCommand", new RelayCommand(p1 => this.TrackingHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("PasswordGeneratorCommand", new RelayCommand(p1 => this.PasswordGeneratorHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("CallWebPageCommand", new RelayCommand(p1 => this.CallWebPageHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("DeleteAttachmentCommand", new RelayCommand(p1 => this.DeleteAttachmentHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("FromClipboardAttachmentCommand", new RelayCommand(p1 => this.FromClipboardAttachmentHandler(p1), p2 => true));
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

                    if (this.IsNew == true)
                    {
                        using (PasswordPinRepository repository = new PasswordPinRepository())
                        {
                            this.RegionSource = repository.ListByRegion();
                            this.SelectedRegion = this.RegionSource.FirstOrDefault(f => f.Name == "Alle");
                        }

                        this.CurrentSelectedItem = new PasswordPin();
                        this.CurrentSelectedItem.Id = this.Id;
                        this.CurrentSelectedItem.CreatedBy = UserInfo.TS().CurrentDomainUser;
                        this.CurrentSelectedItem.CreatedOn = UserInfo.TS().CurrentTime;
                        this.CurrentSelectedItem.Id = Guid.Empty;
                        this.AccessTyp = AccessTyp.Passwort;
                        this.SelectedBackgroundColor = this.ConvertNameToBrush("Transparent");
                        this.Photo = this.ExtractResource("NoPicture256x226.png");
                        base.IsPropertyChanged = false;
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
                                this.TxtPassword.Text = this.Password;
                                this.SelectedBackgroundColor = this.ConvertNameToBrush(this.CurrentSelectedItem.Background);
                                if (string.IsNullOrEmpty(this.CurrentSelectedItem.Region) == false)
                                {
                                    this.SelectedRegion = this.RegionSource.FirstOrDefault(f => f.Name == this.CurrentSelectedItem.Region);
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
                                    this.Photo = this.ExtractResource("NoPicture256x226.png");
                                }
                            }
                            else
                            {
                                this.Photo = this.ExtractResource("NoPicture256x226.png");
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

        #region Command Handler
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
            try
            {
                PasswordPin original = PasswordPin.ToClone<PasswordPin>(this.CurrentSelectedItem);
                this.CurrentSelectedItem.AccessTyp = this.AccessTyp;
                this.CurrentSelectedItem.Title = this.Title;
                this.CurrentSelectedItem.Description = this.Description;
                this.CurrentSelectedItem.ShowDescription = this.ShowDescription;
                this.CurrentSelectedItem.Website = this.Website;
                this.CurrentSelectedItem.Symbol = this.SelectedSymbol;
                this.CurrentSelectedItem.Passwort = this.TxtPassword.Text;
                this.CurrentSelectedItem.Background = this.ConvertBrushToName(this.CBColor.SelectedColor);
                this.CurrentSelectedItem.Region = this.SelectedRegion?.Name;
                using (PasswordPinRepository repository = new PasswordPinRepository())
                {
                    if (this.CurrentSelectedItem.Id == Guid.Empty && this.IsCopy == false)
                    {
                        this.CurrentSelectedItem.CreatedBy = UserInfo.TS().CurrentUser;
                        this.CurrentSelectedItem.CreatedOn = UserInfo.TS().CurrentTime;
                        this.CurrentSelectedItem.ModifiedBy = UserInfo.TS().CurrentUser;
                        this.CurrentSelectedItem.ModifiedOn = UserInfo.TS().CurrentTime;
                        this.CurrentSelectedItem.Id = Guid.NewGuid();
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

        private void DeleteAttachmentHandler(object p1)
        {
            using (AttachmentRepository repository = new AttachmentRepository())
            {
                repository.DeleteAttachment(this.CurrentSelectedItem.Id);
                this.Photo = this.ExtractResource("NoPicture256x226.png");
                this.IsPhotoFound = false;
                this.PhotoSize = this.Photo.Length;
            }
        }

        private void FromClipboardAttachmentHandler(object p1)
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

        private string ConvertBrushToName(Brush colorName)
        {
            string result = string.Empty;
            string cname = new BrushConverter().ConvertToString(colorName);
            PropertyInfo[] brushes = typeof(Brushes).GetProperties();
            foreach (PropertyInfo item in brushes)
            {
                Brush brush = item.GetValue(brushes) as Brush;
                if (brush.ToString() == cname)
                {
                    result = item.Name;
                    break;
                }

            }
            return result;
        }

        public byte[] ExtractResource(string filename)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            using (Stream resFilestream = executingAssembly.GetManifestResourceStream($"{executingAssembly.GetName().Name}.Resources.Picture.{filename}"))
            {
                if (resFilestream == null)
                {
                    return null;
                }

                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
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
                this.ChangedContent(true);
                return;
            }

            if (propValue.Equals(value) == false)
            {
                this.ChangedContent(true);
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
                this.ChangedContent(true);
                return;
            }

            if (propValue.Equals(value) == false)
            {
                this.ChangedContent(true);
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
    }
}
