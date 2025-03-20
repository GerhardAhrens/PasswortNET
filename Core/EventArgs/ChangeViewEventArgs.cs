namespace PasswortNET.Core
{
    using System;
    using ModernBaseLibrary.Core;

    public class ChangeViewEventArgs : EventArgs, IEventAggregatorArgs
    {
        public string Sender { get; set; }
        public string Description { get; set; }
        public MainButton MenuButton { get; set; }
        public string LoginHash { get; set; }

        /// <summary>
        /// Id des Entity Objektes
        /// </summary>
        public Guid EntityId { get; set; }

        public bool IsNew { get; set; } = false;

        public int RowPosition { get; set; }

        /// <summary>
        /// Wechsel von Dialog
        /// </summary>
        public MainButton FromPage { get; set; }

        /// <summary>
        /// Wechsel zu Dialog
        /// </summary>
        public MainButton TargetPage { get; set; }
    }
}
