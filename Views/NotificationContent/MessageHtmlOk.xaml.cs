namespace PasswortNET.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    using ModernIU.Controls;

    /// <summary>
    /// Interaktionslogik für MessageHtmlOk.xaml
    /// </summary>
    public partial class MessageHtmlOk : UserControl, INotificationServiceMessage
    {
        public MessageHtmlOk()
        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnOk, "Click", this.OnOkClick);
        }

        public int CountDown { get; set; }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            (string InfoText, string CustomText, double FontSize) textOption = ((string InfoText, string CustomText, double FontSize))this.Tag;

            this.TbHeader.Text = textOption.Item1;
            this.ControlContent.NavigateToString(textOption.Item2);
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            Window window = this.Parent as Window;
            window.Tag = new Tuple<NotificationBoxButton, object>(NotificationBoxButton.Ok, null);
            window.DialogResult = true;
            window.Close();
        }
    }
}
