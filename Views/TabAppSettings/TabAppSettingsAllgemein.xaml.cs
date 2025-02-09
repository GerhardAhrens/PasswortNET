namespace PasswortNET.Views.TabAppSettings
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaktionslogik für TabAppSettingsAllgemein.xaml
    /// </summary>
    public partial class TabAppSettingsAllgemein : UserControl
    {
        public TabAppSettingsAllgemein()
        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
