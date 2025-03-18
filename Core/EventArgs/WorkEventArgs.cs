namespace PasswortNET.Core
{
    using System;
    using ModernBaseLibrary.Core;
    using PasswortNET.Core.Enums;

    public class WorkEventArgs : EventArgs, IEventAggregatorArgs
    {
        public string Sender { get; set; }
        public AccessTyp AccessTyp { get; set; }
    }
}
