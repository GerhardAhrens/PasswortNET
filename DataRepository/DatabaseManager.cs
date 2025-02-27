
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
        public DatabaseManager(string databaseFile)
        {
            this.DatabaseFile = databaseFile;
            this.ConnectionDB = this.Connection(databaseFile, "222e5937065d2151f760731fef54b0f6");
        }

        private string DatabaseFile { get; set; }

        private ConnectionString ConnectionDB { get; set; }

        private LiteDatabase DatabaseIntern { get; set; }

        public Result<bool> ExistDatabase()
        {
            bool result = false;
            long milliSeconds = 0;

            try
            {
                using (ObjectRuntime objectRuntime = new ObjectRuntime())
                {
                    FileInfo fileInfoe = new FileInfo(this.DatabaseFile);
                    if (fileInfoe.Exists == true)
                    {
                        result = true;
                    }

                    milliSeconds = objectRuntime.ResultMilliseconds();
                }
            }
            catch (Exception ex)
            {
                return Result<bool>.FailureResult(ex);
            }

            return Result<bool>.SuccessResult(result, true, milliSeconds);
        }

        public Result<DatabaseInfo> CheckDatabase()
        {
            DatabaseInfo result = null;
            long milliSeconds = 0;

            try
            {
                using (ObjectRuntime objectRuntime = new ObjectRuntime())
                {
                    this.ConnectionDB.Password = null;
                    this.DatabaseIntern = new LiteDatabase(this.ConnectionDB, (BsonMapper)null);

                    if (this.DatabaseIntern != null)
                    {
                        ILiteCollection<DatabaseInfo> entityCollection = this.DatabaseIntern.GetCollection<DatabaseInfo>(typeof(DatabaseInfo).Name);
                        if (entityCollection.Count() == 0)
                        {
                            DatabaseInfo entity = new();
                            entity.Name = "PasswortNET";
                            entity.Description = "verwaltung von Passworten, Zugänge, PIN, Lizenzen";
                            entity.Version = 1;
                            entity.CreatedBy = UserInfo.TS().CurrentDomainUser;
                            entity.CreatedOn = UserInfo.TS().CurrentTime;
                            BsonValue resultEntity = entityCollection.Insert(entity);
                            result = entityCollection.FindAll().FirstOrDefault();
                        }
                        else
                        {
                            ILiteCollection<DatabaseInfo> entityCollectionX = this.DatabaseIntern.GetCollection<DatabaseInfo>(typeof(DatabaseInfo).Name);
                            result = entityCollectionX.FindAll().FirstOrDefault();
                        }

                        milliSeconds = objectRuntime.ResultMilliseconds();
                    }
                }
            }
            catch (Exception ex)
            {
                return Result<DatabaseInfo>.FailureResult(ex);
            }

            return Result<DatabaseInfo>.SuccessResult(result, true, milliSeconds);
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
            conn.Connection = ConnectionType.Direct;
            conn.Password = password;

            return conn;
        }

        private void LiteDBChangePassword(string password, ConnectionString connectionString)
        {
            LiteDatabase val = new LiteDatabase(connectionString, (BsonMapper)null);
            try
            {
                RebuildOptions val2 = ((!(password == "")) ? new RebuildOptions
                {
                    Password = password
                } : new RebuildOptions());
                val.Rebuild(val2);
            }
            finally
            {
                ((IDisposable)val)?.Dispose();
            }
        }
    }
}
