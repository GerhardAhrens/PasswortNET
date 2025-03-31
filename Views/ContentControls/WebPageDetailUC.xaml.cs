namespace PasswortNET.Views.ContentControls
{
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    using ModernBaseLibrary.Collection;
    using ModernBaseLibrary.Core;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.Core.Enums;
    using PasswortNET.DataRepository;
    using PasswortNET.Model;

    /// <summary>
    /// Interaktionslogik für WebPageDetailUC.xaml
    /// </summary>
    public partial class WebPageDetailUC : UserControlBase
    {
        public WebPageDetailUC(ChangeViewEventArgs args) : base(typeof(WebPageDetailUC))
        {
            this.InitializeComponent();

            this.Id = args.EntityId;
            this.RowPosition = args.RowPosition;
            this.IsNew = args.IsNew;
            this.IsCopy = args.IsCopy;

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

        public string Passwort
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
        }

        #region UserControl Events
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this);
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
        #endregion Command Handler

        #region Register Validations
        private void RegisterValidations()
        {
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

        private void NavigationProperty<T>(T value, string propertyName)
        {
            if (value is string txt)
            {
                if (txt.ToLower() == "title")
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                    {
                        this.TxtTitel.Focus();
                        this.TxtTitel.Background = Brushes.Coral;
                    }));
                }
                else if (txt.ToLower() == "password")
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                    {
                        //this.TxtPassword.Focus();
                        //this.TxtPassword.Background = Brushes.Coral;
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
