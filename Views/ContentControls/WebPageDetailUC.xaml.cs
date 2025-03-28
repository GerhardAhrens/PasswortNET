namespace PasswortNET.Views.ContentControls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;

    /// <summary>
    /// Interaktionslogik für WebPageDetailUC.xaml
    /// </summary>
    public partial class WebPageDetailUC : UserControlBase
    {
        public WebPageDetailUC(ChangeViewEventArgs args) : base(typeof(AuditTrailDetailUC))
        {
            this.InitializeComponent();

            this.Id = args.EntityId;
            this.InitCommands();

            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            this.DataContext = this;
        }

        #region Properties
        private Guid Id { get; set; }
        #endregion Properties

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("BackCommand", new RelayCommand(p1 => this.BackHandler(p1), p2 => true));
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this);
        }

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
    }
}
