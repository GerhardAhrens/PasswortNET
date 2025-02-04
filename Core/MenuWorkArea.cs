namespace PasswortNET.Core
{
    using System;
    using System.Windows.Controls;

    public class MenuWorkArea : Tuple<UserControl, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuWorkArea"/> class.
        /// </summary>
        public MenuWorkArea(UserControl workContent, string usedTime = "") : base(workContent, usedTime)
        {
            this.WorkContent = workContent;
            this.UsedTime = usedTime;
        }

        public UserControl WorkContent { get; private set; }

        public string UsedTime { get; set; }
    }
}
