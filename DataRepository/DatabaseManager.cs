
//-----------------------------------------------------------------------
// <copyright file="DatabaseManager.cs" company="Lifeprojects.de">
//     Class: DatabaseManager
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>01.07.2022</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.DataRepository
{
    using System;
    using System.IO;

    using LiteDB;
    using LiteDB.Engine;

    using ModernBaseLibrary.Core;

    using PasswortNET.Model;

    public sealed class DatabaseManager : DisposableCoreBase
    {
        public DatabaseManager(string databaseFile, string password)
        {
            this.DatabaseFile = databaseFile;
            this.ConnectionDB = this.Connection(databaseFile, password);
        }

        private string DatabaseFile { get; set; }

        private ConnectionString ConnectionDB { get; set; }

        private LiteDatabase DatabaseIntern { get; set; }

        public Result<bool> ExistDatabase()
        {
            bool result = false;
            long ticks = 0;

            try
            {
                using (ObjectRuntime objectRuntime = new ObjectRuntime())
                {
                    FileInfo fileInfoe = new FileInfo(this.DatabaseFile);
                    if (fileInfoe.Exists == true)
                    {
                        result = true;
                    }

                    ticks = objectRuntime.ResultMilliseconds();
                }
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult(ex);
            }

            return Result<bool>.SuccessResult(result, true, ticks);
        }

        public void CheckDatabase()
        {
            try
            {
                this.DatabaseIntern = new LiteDatabase(this.ConnectionDB);

                if (this.DatabaseIntern != null)
                {
                    ILiteCollection<Region> entityCollection = this.DatabaseIntern.GetCollection<Region>(typeof(Region).Name);
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        public void ChangePassword(string password)
        {
            long rebuildResult = 0;

            try
            {
                this.DatabaseIntern = new LiteDatabase(this.ConnectionDB);

                if (this.DatabaseIntern != null)
                {
                    RebuildOptions options = new RebuildOptions();
                    options.Password = password;
                    rebuildResult = this.DatabaseIntern.Rebuild(options);
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private ConnectionString Connection(string databaseFile, string password = null)
        {
            ConnectionString conn = new ConnectionString(databaseFile);
            conn.Connection = ConnectionType.Shared;
            conn.Password = password;

            return conn;
        }
    }
}
