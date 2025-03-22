namespace PasswortNET.Views.ContentControls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using ModernIU.Controls;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;

    /// <summary>
    /// Interaktionslogik für PasswordDetailUC.xaml
    /// </summary>
    public partial class PasswordDetailUC : UserControlBase
    {
        private INotificationService notificationService = new NotificationService();

        public PasswordDetailUC(ChangeViewEventArgs e) : base(typeof(PasswordDetailUC))
        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            this.InitCommands();
            this.DataContext = this;
        }

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
        }

        private void BackHandler(object p1)
        {
            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = MainButton.MainOverview,
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
    }
}
