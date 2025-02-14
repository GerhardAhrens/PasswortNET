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
            if (this.lbRegion.HasItems == true)
            {
                this.lbRegion.Focus();
                this.lbRegion.SelectedIndex = 0;
                this.lbRegion.ScrollIntoView(this.lbRegion.Items[this.lbRegion.SelectedIndex]);
                this.lbRegion.Focus();
            }
        }
    }
}
