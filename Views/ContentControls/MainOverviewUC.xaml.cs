﻿namespace PasswortNET.Views.ContentControls
{
    using System.Windows;
    using System.Windows.Controls;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;

    /// <summary>
    /// Interaktionslogik für MainOverviewUC.xaml
    /// </summary>
    public partial class MainOverviewUC : UserControlBase
    {
        public MainOverviewUC()
        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            this.InitCommands();
            this.DataContext = this;
        }

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("LogoffCommand", new RelayCommand(p1 => this.LogoffHandler(p1), p2 => true));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.IsUCLoaded = true;
        }

        private void LogoffHandler(object p1)
        {
            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = MainButton.Login,
            });
        }
    }
}
