/*
 * <copyright file="DatabaseBackup.cs" company="Lifeprojects.de">
 *     Class: DatabaseBackup
 *     Copyright © Lifeprojects.de 2025
 * </copyright>
 *
 * <author>Gerhard Ahrens - Lifeprojects.de</author>
 * <email>gerhard.ahrens@lifeprojects.de</email>
 * <date>16.02.2025 19:01:17</date>
 * <Project>CurrentProject</Project>
 *
 * <summary>
 * Beschreibung zur Klasse
 * </summary>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by the Free Software Foundation, 
 * either version 3 of the License, or (at your option) any later version.
 * This program is distributed in the hope that it will be useful,but WITHOUT ANY WARRANTY; 
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.You should have received a copy of the GNU General Public License along with this program. 
 * If not, see <http://www.gnu.org/licenses/>.
*/

namespace PasswortNET.DataRepository
{
    using System.IO;
    using System.Linq;

    using ModernBaseLibrary.Core;
    using ModernBaseLibrary.Extension;

    using PasswortNET.Core;
    using PasswortNET.Core.Enums;

    public class DatabaseBackup : DisposableCoreBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseBackup"/> class.
        /// </summary>
        public DatabaseBackup()
        {
            this.BackupFolder = this.CreateBackupFolder();

            using (ApplicationSettings settings = new ApplicationSettings())
            {
                if (settings.IsExitSettings() == true)
                {
                    settings.Load();
                    this.Fullname = settings.DatabaseFullname;
                    this.DatabaseFileBackup = settings.IsDatabaseBackup;
                    this.MaxBackupFile = settings.MaxBackupFile;
                    this.BackupFrequency = settings.BackupFrequency.ToEnum<BackupFrequency>();
                }
            }
        }

        private string Fullname { get; set; }

        private string BackupFolder { get; set; }

        private bool DatabaseFileBackup { get; set; }

        private int MaxBackupFile { get; set; }

        private BackupFrequency BackupFrequency { get; set; }

        public void CheckAndRun()
        {
            if (this.DatabaseFileBackup == false)
            {
                return;
            }

            if (this.BackupFrequency == BackupFrequency.OnceDaily)
            {
                if (LastBackup() == DateTime.Now.Date)
                {
                    return;
                }
            }

            try
            {
                DirectoryInfo directory = new DirectoryInfo(this.BackupFolder);
                if (directory.EqualsAny() == false)
                {
                    IEnumerable<FileInfo> files = directory.EnumerateFiles("*.db");
                    if (files.IsNullOrEmpty() == false)
                    {
                        if (files.Count() > this.MaxBackupFile)
                        {
                            this.CopyDatabase(this.Fullname, directory);
                            this.DeleteDatabase(files);
                        }
                        else
                        {
                            this.CopyDatabase(this.Fullname, directory);
                        }
                    }
                    else
                    {
                        this.CopyDatabase(this.Fullname, directory);
                    }
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        public DateTime LastBackup()
        {
            DateTime result = DateTime.Now.DefaultDate();

            if (string.IsNullOrEmpty(this.BackupFolder) == true)
            {
                return result; ;
            }

            if (Path.HasExtension(this.BackupFolder) == true)
            {
                this.BackupFolder = Path.GetDirectoryName(this.BackupFolder);
            }

            try
            {
                DirectoryInfo directory = new DirectoryInfo(this.BackupFolder);
                if (directory.IsNullOrEmpty() == false)
                {
                    IEnumerable<FileInfo> files = directory.EnumerateFiles("*.db");
                    if (files.IsNullOrEmpty() == false)
                    {
                        DateTime lastFile = files.Max(f => f.LastAccessTime);
                        result = lastFile;
                    }
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }

            return result.Date;
        }

        public string BackupInfo()
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(this.BackupFolder) == true)
            {
                return string.Empty;
            }

            if (Path.HasExtension(this.BackupFolder) == true)
            {
                this.BackupFolder = Path.GetDirectoryName(this.BackupFolder);
            }

            try
            {
                DirectoryInfo directory = new DirectoryInfo(this.BackupFolder);
                if (directory.IsNullOrEmpty() == false)
                {
                    IEnumerable<FileInfo> files = directory.EnumerateFiles("*.db");
                    if (files.IsNullOrEmpty() == false)
                    {
                        IEnumerable<FileInfo> filesSort = files.OrderByDescending(f => f.LastAccessTime);
                        int CountBackup = files.Count();
                        FileInfo lastFile = files.MaxBy(f => f.LastAccessTime);
                        string sizeInfo = string.Format(new FileSizeFormatTo(), "Dateigröße: {0:fs}", lastFile.Length);
                        result = $"{lastFile.LastAccessTime}, {lastFile.FullName}; Anzahl: {CountBackup}; {sizeInfo}";
                    }
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                result = "Keine Backupdatei gefunden.";
            }

            return result;
        }

        private void CopyDatabase(string databaseName, DirectoryInfo backupDirectory)
        {
            if (File.Exists(databaseName) == true)
            {
                string tempBackupName = this.CreateBackupFile(databaseName, backupDirectory);
                File.Copy(databaseName, tempBackupName);
            }
        }

        private void DeleteDatabase(IEnumerable<FileInfo> files)
        {
            IEnumerable<FileInfo> filesSort = files.OrderBy(f => f.LastWriteTime);
            IEnumerable<FileInfo> filesMax = filesSort.Take(files.Count() - this.MaxBackupFile);
            foreach (FileInfo file in filesMax)
            {
                File.Delete(file.FullName);
            }
        }

        private string CreateBackupFile(string databaseName, DirectoryInfo backupDirectory)
        {
            this.BackupFolder = $"{this.BackupFolder}\\{Path.GetFileName(databaseName)}";
            DecomposedFilePath testPath = new DecomposedFilePath(this.BackupFolder);
            IEnumerable<FileInfo> files = backupDirectory.EnumerateFiles("*.db");
            DecomposedFilePath nextFile = testPath.GetFirstFreeFilePath(files);

            return nextFile.FullFilePath;
        }


        private string CreateBackupFolder()
        {
            string backupPath = string.Empty;

            using (ApplicationSettings settings = new ApplicationSettings())
            {
                if (settings.IsExitSettings() == true)
                {
                    settings.Load();
                    backupPath = settings.DatabaseBackupFullname;
                }
            }

            if (string.IsNullOrEmpty(Path.GetDirectoryName(backupPath)) == false)
            {
                try
                {
                    if (Directory.Exists(Path.GetDirectoryName(backupPath)) == false)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(backupPath));
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return Path.GetDirectoryName(backupPath);
        }
    }
}
