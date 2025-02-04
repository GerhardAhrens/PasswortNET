namespace PasswortNET.Core
{
    using System;
    using ModernBaseLibrary.Core;

    public class ChangeViewEventArgs : EventArgs, IEventAggregatorArgs
    {
        public string Sender { get; set; }
        public string Description { get; set; }
        public MainButton MenuButton { get; set; }
    }
}
