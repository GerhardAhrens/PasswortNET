//-----------------------------------------------------------------------
// <copyright file="ChangeTrackingRepository.cs" company="Lifeprojects.de">
//     Class: ChangeTrackingRepository
//     Copyright © Lifeprojects.de 2021
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>12.04.2021</date>
//
// <summary>
// Viewmodel Klasse für Change Tracking
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.DataRepository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LiteDB;

    using PasswortNET.Model;

    public class ChangeTrackingRepository : RepositoryBase<ChangeTracking>
    {
        public ChangeTrackingRepository(string databaseFile = "") : base(databaseFile)
        {
            this.Database = base.DatabaseIntern;
            this.Collection = base.CollectionIntern;
            this.Collection.EnsureIndex(e => e.ObjectId);
        }

        public LiteDatabase Database { get; private set; }

        public ILiteCollection<ChangeTracking> Collection { get; private set; }

        public override int Count()
        {
            return base.Count();
        }

        public int CountById(Guid id)
        {
            int result = 0;

            if (id != Guid.Empty)
            {
                try
                {
                    ILiteCollection<ChangeTracking> collection = this.DatabaseIntern.GetCollection<ChangeTracking>(typeof(ChangeTracking).Name);
                    result = collection.Count(f => f.ObjectId == id.ToString());
                }
                catch (Exception ex)
                {
                    string errorText = ex.Message;
                    throw;
                }
            }

            return result;
        }

        new public IEnumerable<ChangeTracking> GetById(Guid id)
        {
            IEnumerable<ChangeTracking> result = null;

            if (id != Guid.Empty)
            {
                ILiteCollection<ChangeTracking> collection = this.DatabaseIntern.GetCollection<ChangeTracking>(typeof(ChangeTracking).Name);
                result = collection.Find(f => f.ObjectId == id.ToString()).OrderByDescending(o => o.ModifiedOn).ToList();
            }

            return result;
        }

        public override void Add(ChangeTracking entity)
        {
            try
            {
                ILiteCollection<ChangeTracking> entityCollection = this.DatabaseIntern.GetCollection<ChangeTracking>(typeof(ChangeTracking).Name);

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
