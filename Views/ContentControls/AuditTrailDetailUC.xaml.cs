namespace PasswortNET.Views.ContentControls
{
    using System.Windows.Controls;
    using System.Windows;

    using ModernUI.MVVM.Base;
    using System.Windows.Input;
    using PasswortNET.Core;

    /// <summary>
    /// Interaktionslogik für AuditTrailDetailUC.xaml
    /// </summary>
    public partial class AuditTrailDetailUC : UserControlBase
    {
        public AuditTrailDetailUC(ChangeViewEventArgs args) : base(typeof(AuditTrailDetailUC))
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
                MenuButton = FunctionButtons.PasswordDetail,
            });
        }
        #endregion Command Handler
    }
}
