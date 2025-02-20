//-----------------------------------------------------------------------
// <copyright file="PasswordPinRepository.cs" company="Lifeprojects.de">
//     Class: PasswordPinRepository
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;

    using LiteDB;

    using ModernBaseLibrary.Core;

    using PasswortNET.AuditTrail;
    using PasswortNET.Model;

    public class PasswordPinRepository : RepositoryBase<PasswordPin>
    {
        public PasswordPinRepository(string databaseFile = "") : base(databaseFile)
        {
            this.Database = base.DatabaseIntern;
            this.Collection = base.CollectionIntern;
            this.Collection.EnsureIndex(e => e.Id);
        }

        public LiteDatabase Database { get; private set; }

        public ILiteCollection<PasswordPin> Collection { get; private set; }

        public override int Count()
        {
            return base.Count();
        }

        public List<PasswordPin> ListByAttachmentId(Guid id)
        {
            List<PasswordPin> result = null;

            if (id != Guid.Empty)
            {
                try
                {
                    ILiteCollection<PasswordPin> attachmentCollection = this.DatabaseIntern.GetCollection<PasswordPin>(typeof(PasswordPin).Name);
                    result = attachmentCollection.Find(f => f.Id == id).ToList();
                }
                catch (Exception ex)
                {
                    string errotrText = ex.Message;
                    throw;
                }
            }

            return result;
        }

        public List<Region> ListByRegion()
        {
            List<Region> result = null;

            try
            {
                ILiteCollection<Region> regionCollection = this.DatabaseIntern.GetCollection<Region>(typeof(Region).Name);
                result = regionCollection.FindAll().ToList();
            }
            catch (Exception ex)
            {
                string errotrText = ex.Message;
                throw;
            }

            return result;
        }

        public override void Add(PasswordPin entity)
        {
            try
            {
                ILiteCollection<PasswordPin> entityCollection = this.DatabaseIntern.GetCollection<PasswordPin>(typeof(PasswordPin).Name);

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

        public void Add(PasswordPin entity, OperationResult<AuditTrailResult> auditTrailresult)
        {
            bool transaction = this.DatabaseIntern.BeginTrans();

            try
            {
                ILiteCollection<ChangeTracking> trackingCollection = this.DatabaseIntern.GetCollection<ChangeTracking>(typeof(ChangeTracking).Name);
                ILiteCollection<PasswordPin> entityCollection = this.DatabaseIntern.GetCollection<PasswordPin>(typeof(PasswordPin).Name);

                if (transaction == true)
                {
                    if (trackingCollection != null && auditTrailresult.Success == true)
                    {
                        AuditTrailResult auditTrail = auditTrailresult.Result;
                        BsonValue resultEntity = trackingCollection.Insert(auditTrail.TrackingObject);
                    }

                    if (entityCollection != null)
                    {
                        BsonValue resultEntity = entityCollection.Insert(entity);
                    }

                    this.DatabaseIntern.Commit();
                }
            }
            catch (Exception ex)
            {
                if (transaction == true)
                {
                    this.DatabaseIntern.Rollback();
                }

                string errotrText = ex.Message;
                throw;
            }
        }

        public void Add(PasswordPin entity, OperationResult<AuditTrailResult> auditTrailresult, byte[] photo)
        {
            bool transaction = this.DatabaseIntern.BeginTrans();

            try
            {
                ILiteCollection<ChangeTracking> trackingCollection = this.DatabaseIntern.GetCollection<ChangeTracking>(typeof(ChangeTracking).Name);
                ILiteCollection<PasswordPin> entityCollection = this.DatabaseIntern.GetCollection<PasswordPin>(typeof(PasswordPin).Name);

                if (transaction == true)
                {
                    if (trackingCollection != null && auditTrailresult.Success == true)
                    {
                        AuditTrailResult auditTrail = auditTrailresult.Result;
                        BsonValue resultEntity = trackingCollection.Insert(auditTrail.TrackingObject);
                    }

                    if (entityCollection != null)
                    {
                        BsonValue resultEntity = entityCollection.Insert(entity);
                    }

                    if (entity != null && photo?.Length > 0)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            stream.Write(photo, 0, photo.Length);
                            stream.Position = 0;
                            this.DatabaseIntern.FileStorage.Delete(entity.Id.ToString());
                            LiteFileInfo<string> fileInfo = this.DatabaseIntern.FileStorage.Upload(entity.Id.ToString(), "Unbekannt", stream);
                        }
                    }

                    this.DatabaseIntern.Commit();
                }
            }
            catch (Exception ex)
            {
                if (transaction == true)
                {
                    this.DatabaseIntern.Rollback();
                }

                string errotrText = ex.Message;
                throw;
            }
        }

        public void Update(PasswordPin entity, OperationResult<AuditTrailResult> auditTrailresult, byte[] photo)
        {
            bool transaction = this.DatabaseIntern.BeginTrans();

            try
            {
                ILiteCollection<ChangeTracking> trackingCollection = this.DatabaseIntern.GetCollection<ChangeTracking>(typeof(ChangeTracking).Name);
                ILiteCollection<PasswordPin> entityCollection = this.DatabaseIntern.GetCollection<PasswordPin>(typeof(PasswordPin).Name);

                if (transaction == true)
                {
                    if (trackingCollection != null && auditTrailresult.Success == true && auditTrailresult.Result != null)
                    {
                        AuditTrailResult auditTrail = auditTrailresult.Result;
                        BsonValue resultEntity = trackingCollection.Insert(auditTrail.TrackingObject);
                    }

                    if (entityCollection != null)
                    {
                        BsonValue resultEntity = entityCollection.Update(entity);
                    }

                    if (entity != null && photo.Length > 0)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            stream.Write(photo, 0, photo.Length);
                            stream.Position = 0;
                            this.DatabaseIntern.FileStorage.Delete(entity.Id.ToString());
                            LiteFileInfo<string> fileInfo = this.DatabaseIntern.FileStorage.Upload(entity.Id.ToString(), "Unbekannt", stream);
                        }
                    }

                    this.DatabaseIntern.Commit();
                }
            }
            catch (Exception ex)
            {
                if (transaction == true)
                {
                    this.DatabaseIntern.Rollback();
                }

                string errotrText = ex.Message;
                throw;
            }
        }

        public void Delete(Guid id, OperationResult<AuditTrailResult> auditTrailresult)
        {
            bool transaction = this.DatabaseIntern.BeginTrans();

            try
            {
                ILiteCollection<ChangeTracking> trackingCollection = this.DatabaseIntern.GetCollection<ChangeTracking>(typeof(ChangeTracking).Name);
                ILiteCollection<PasswordPin> entityCollection = this.DatabaseIntern.GetCollection<PasswordPin>(typeof(PasswordPin).Name);

                if (transaction == true)
                {
                    if (trackingCollection != null && auditTrailresult.Success == true)
                    {
                        AuditTrailResult auditTrail = auditTrailresult.Result;
                        BsonValue resultEntity = trackingCollection.Insert(auditTrail.TrackingObject);
                    }

                    if (id != Guid.Empty)
                    {
                        entityCollection.Delete(id);
                    }

                    this.DatabaseIntern.Commit();
                }
            }
            catch (Exception ex)
            {
                if (transaction == true)
                {
                    this.DatabaseIntern.Rollback();
                }

                string errotrText = ex.Message;
                throw;
            }
        }

        public void AddImage()
        {
            this.DatabaseIntern.FileStorage.Upload("","");
        }

    }
}
