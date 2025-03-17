//-----------------------------------------------------------------------
// <copyright file="StatistikRepository.cs" company="Lifeprojects.de">
//     Class: StatistikRepository
//     Copyright © Lifeprojects.de 2021
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>12.04.2021</date>
//
// <summary>
// Viewmodel Klasse für Passwörter und PIns
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.DataRepository
{
    using LiteDB;
    using ModernBaseLibrary.Core;

    using PasswortNET.Core;
    using PasswortNET.Core.Enums;
    using PasswortNET.Model;

    public class StatistikRepository : RepositoryBase<StatistikRepository>
    {
        public StatistikRepository(string databaseFile = "") : base(databaseFile)
        {
            this.Database = base.DatabaseIntern;
        }

        public LiteDatabase Database { get; private set; }

        public ILiteCollection<PasswordPin> Collection { get; private set; }

        public int CountAll { get; set; }

        public int CountWebsite { get; set; }

        public int CountPasswort { get; set; }

        public int CountPin { get; set; }

        public int CountLicense { get; set; }

        public DateTime LastAccess { get; set; }

        public override int Count()
        {
            return base.Count();
        }

        public Result<bool> GetStatistic()
        {
            bool result = false;
            long milliSeconds = 0;

            try
            {
                using (ObjectRuntime objectRuntime = new ObjectRuntime())
                {
                    ILiteCollection<PasswordPin> entityCollection = this.DatabaseIntern.GetCollection<PasswordPin>(typeof(PasswordPin).Name);
                    if (entityCollection.Count() > 0)
                    {
                        this.CountAll = entityCollection.Count();
                        this.CountWebsite = entityCollection.Count(c => c.AccessTyp == AccessTyp.Website);
                        this.CountPasswort = entityCollection.Count(c => c.AccessTyp == AccessTyp.Passwort);
                        this.CountPin = entityCollection.Count(c => c.AccessTyp == AccessTyp.Pin);
                        this.CountLicense = entityCollection.Count(c => c.AccessTyp == AccessTyp.License);
                        this.LastAccess = entityCollection.Max(m => m.ModifiedOn);
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
    }
}
