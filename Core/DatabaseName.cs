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

    using ModernBaseLibrary.CoreBase;

    public static class DatabaseName
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseName"/> class.
        /// </summary>
        static DatabaseName()
        {
        }

        public static string PathName => CurrentSettingsPath() + "\\";

        public static string FullDatabaseName
        {
            get
            {
                string path = CurrentSettingsPath();
                string name = "PasswortNET.db";
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

        private static SettingsLocation SettingsLocation { get; set; } = SettingsLocation.ProgramData;

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
