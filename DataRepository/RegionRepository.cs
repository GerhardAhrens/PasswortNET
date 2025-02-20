//-----------------------------------------------------------------------
// <copyright file="RegionRepository.cs" company="Lifeprojects.de">
//     Class: RegionRepository
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>31.12.2022</date>
//
// <summary>
// Viewmodel Klasse für Regionen
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.DataRepository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LiteDB;

    using PasswortNET.Model;

    public class RegionRepository : RepositoryBase<Region>
    {
        public RegionRepository(string databaseFile = "") : base(databaseFile)
        {
            if (base.DatabaseIntern != null)
            {
                this.Database = base.DatabaseIntern;
                this.Collection = base.CollectionIntern;
                this.Collection.EnsureIndex(e => e.Id);
            }
        }

        public LiteDatabase Database { get; private set; }

        public ILiteCollection<Region> Collection { get; private set; }

        public override int Count()
        {
            return base.Count();
        }

        public override void Add(Region entity)
        {
            try
            {
                ILiteCollection<Region> entityCollection = this.DatabaseIntern.GetCollection<Region>(typeof(Region).Name);

                if (entityCollection != null)
                {
                    BsonValue resultEntity = entityCollection.Insert(entity);
                }
            }
            catch (Exception ex)
            {
                string errotrText = ex.Message;
                throw;
            }
        }
    }
}
