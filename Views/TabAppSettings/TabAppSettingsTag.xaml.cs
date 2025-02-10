namespace PasswortNET.Views.TabAppSettings
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaktionslogik für TabAppSettingsTag.xaml
    /// </summary>
    public partial class TabAppSettingsTag : UserControl
    {
        public TabAppSettingsTag()
        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
