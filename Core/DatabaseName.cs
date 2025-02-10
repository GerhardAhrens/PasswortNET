//-----------------------------------------------------------------------
// <copyright file="DatabaseName.cs" company="Lifeprojects.de">
//     Class: DatabaseName
//     Copyright © Lifeprojects.de 2025
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>Gerhard Ahrens@Lifeprojects.de</email>
// <date>10.02.2025 14:32:16</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Core
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using ModernBaseLibrary.CoreBase;

    public static class DatabaseName
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseName"/> class.
        /// </summary>
        static DatabaseName()
        {
        }

        public static string PathDatabaseName => CurrentSettingsPath() + "\\";
        public static string PathBackupName => CurrentSettingsPath(true) + "\\";

        public static string FullDatabaseName
        {
            get
            {
                string path = CurrentSettingsPath();
                string name = Filename;
                return Path.Combine(path, name);
            }
        }

        public static string FullBackupName
        {
            get
            {
                string path = CurrentSettingsPath(true);
                string name = BackupFilename;
                return Path.Combine(path, name);
            }
        }

        public static string Filename
        {
            get
            {
                return "PasswortNET.db";
            }
        }

        public static string BackupFilename
        {
            get
            {
                return "PasswortNET.db";
            }
        }

        public static string BuildBackupName()
        {
            return GetNextFileName(FullBackupName);
        }

        private static SettingsLocation SettingsLocation { get; set; } = SettingsLocation.ProgramData;

        private static string GetNextFileName(string fileName, int maxFiles = 5)
        {
            string extension = Path.GetExtension(fileName);

            int i = 0;
            while (File.Exists(fileName))
            {
                if (i == 0)
                {
                    fileName = fileName.Replace(extension, $"({++i}){extension}");
                }
                else
                {
                    fileName = fileName.Replace($"({i}){extension}", $"({++i}){extension}");
                }
            }

            return fileName;
        }

        private static string CurrentSettingsPath(bool isbackupPath = false)
        {
            string pathDatabase = string.Empty;
            if (SettingsLocation == SettingsLocation.ProgramData)
            {
                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                if (isbackupPath == false)
                {
                    pathDatabase = folderPath + "\\" + ApplicationName() + "\\Database";
                }
                else
                {
                    pathDatabase = folderPath + "\\" + ApplicationName() + "\\Backup";
                }
            }
            else
            {
                string directoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                if (isbackupPath == false)
                {
                    pathDatabase = directoryName + "\\" + ApplicationName() + "\\Database";
                }
                else
                {
                    pathDatabase = directoryName + "\\" + ApplicationName() + "\\Backup";
                }
            }

            if (!string.IsNullOrEmpty(pathDatabase))
            {
                try
                {
                    if (!Directory.Exists(pathDatabase))
                    {
                        Directory.CreateDirectory(pathDatabase);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return pathDatabase;
        }

        private static string ApplicationName()
        {
            string empty = string.Empty;
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            return entryAssembly.GetName().Name;
        }
    }
}
