namespace PasswortNET.Core
{
    using System;

    using ModernBaseLibrary.CoreBase;

    public class ApplicationSettings : SmartSettingsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSettings"/> class.
        /// </summary>
        public ApplicationSettings() : base()
        {
        }

        public string LastUser { get; set; }

        public DateTime LastAccess { get; set; }

        public bool ExitApplicationQuestion { get; set; } = true;

        public bool SaveLastWindowsPosition { get; set; } = true;

        public string RunEnvironment { get; set; } = "dev";

        public string Hash { get; set; }

        public string ControlHash { get; set; }

        public string DatabaseFullname { get; set; }

        public bool DatabaseBackup { get; set; } = false;

        public int MaxBackupFile { get; set; } = 5;
    }
}
