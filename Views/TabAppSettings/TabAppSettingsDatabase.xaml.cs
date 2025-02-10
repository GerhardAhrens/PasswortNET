namespace PasswortNET.Views.TabAppSettings
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaktionslogik für TabAppSettingsDatabase.xaml
    /// </summary>
    public partial class TabAppSettingsDatabase : UserControl
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
