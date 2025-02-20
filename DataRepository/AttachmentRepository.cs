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
    using System.IO;

    using LiteDB;

    using PasswortNET.Model;

    public class AttachmentRepository : RepositoryBase<Attachment>
    {
        public AttachmentRepository(string databaseFile = "") : base(databaseFile)
        {
            this.Database = base.DatabaseIntern;
            this.Collection = base.CollectionIntern;
            this.Collection.EnsureIndex(e => e.Id);
        }

        public LiteDatabase Database { get; private set; }

        public ILiteCollection<Attachment> Collection { get; private set; }

        public override int Count()
        {
            return base.Count();
        }

        public override void Add(Attachment entity)
        {
            try
            {
            }
            catch (Exception ex)
            {
                string errotrText = ex.Message;
                throw;
            }
        }

        public void Add(Guid id,string entity)
        {
            try
            {
                using (FileStream stream = new FileStream(entity, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    stream.Position = 0;
                    if (this.DatabaseIntern.FileStorage.Exists(id.ToString()) == false)
                    {
                        LiteFileInfo<string> fileInfo = this.DatabaseIntern.FileStorage.Upload(id.ToString(), entity, stream);
                    }
                    else
                    {
                        this.DatabaseIntern.FileStorage.Delete(id.ToString());
                        LiteFileInfo<string> fileInfo = this.DatabaseIntern.FileStorage.Upload(id.ToString(), entity, stream);
                    }
                }
            }
            catch (Exception ex)
            {
                string errotrText = ex.Message;
                throw;
            }
        }

        public void Add(Guid id, byte[] entity)
        {
            try
            {
                if (entity != null && entity.Length > 0)
                {
                    MemoryStream stream = new MemoryStream();
                    stream.Write(entity, 0, entity.Length);
                    this.DatabaseIntern.FileStorage.Delete(id.ToString());
                    stream.Position = 0;
                    this.DatabaseIntern.FileStorage.Delete(id.ToString());
                    LiteFileInfo<string> fileInfo = this.DatabaseIntern.FileStorage.Upload(id.ToString(), "FromClipboard", stream);
                }
            }
            catch (Exception ex)
            {
                string errotrText = ex.Message;
                throw;
            }
        }

        public byte[] GetAttachmentById(Guid id)
        {
            try
            {
                if (this.DatabaseIntern.FileStorage.Exists(id.ToString()) == true)
                {
                    MemoryStream stream = new MemoryStream();
                    LiteFileInfo<string> file = this.DatabaseIntern.FileStorage.FindById(id.ToString());
                    file.CopyTo(stream);
                    return stream.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteAttachment (Guid id)
        {
            try
            {
                if (this.DatabaseIntern.FileStorage.Exists(id.ToString()) == true)
                {
                    this.DatabaseIntern.FileStorage.Delete(id.ToString());
                }
            }
            catch (Exception ex)
            {
                string errotrText = ex.Message;
                throw;
            }
        }

        public bool ExistAttachment(Guid id)
        {
            bool result = false;
            try
            {
                result = this.DatabaseIntern.FileStorage.Exists(id.ToString());

            }
            catch (Exception ex)
            {
                string errotrText = ex.Message;
                throw;
            }

            return result;
        }
    }
}
