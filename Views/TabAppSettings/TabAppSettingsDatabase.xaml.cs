namespace PasswortNET.Views.TabAppSettings
{
    using System.Windows;
    using System.Windows.Controls;

    using ModernUI.MVVM.Base;

    /// <summary>
    /// Interaktionslogik für TabAppSettingsDatabase.xaml
    /// </summary>
    public partial class TabAppSettingsDatabase : UserControlBase
    {
        public TabAppSettingsDatabase()
        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
