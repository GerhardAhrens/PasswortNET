namespace PasswortNET.Views.ContentControls
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Drawing.Imaging;
    using LiteDB;

    using ModernBaseLibrary.Core;
    using ModernBaseLibrary.Extension;
    using ModernBaseLibrary.Graphics;

    using ModernIU.Controls;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.DataRepository;
    using PasswortNET.Model;
    using System.Linq;
    using static System.Net.Mime.MediaTypeNames;
    using PasswortNET.Core.Enums;

    /// <summary>
    /// Interaktionslogik für DataSyncUC.xaml
    /// </summary>
    public partial class DataSyncUC : UserControlBase
    {
        private INotificationService notificationService = new NotificationService();

        public DataSyncUC() : base(typeof(DataSyncUC))
        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            this.InitCommands();
            this.DataContext = this;
        }

        public bool IsBusyIndicator
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value);
        }

        public double ProgressBarValueExport
        {
            get => base.GetValue<double>();
            set => base.SetValue(value);
        }

        public string ProgressBarTextExport
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public double ProgressBarValueImport
        {
            get => base.GetValue<double>();
            set => base.SetValue(value);
        }

        public string ProgressBarTextImport
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public bool IsImportAllRows
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value);
        }

        public string ExportFolder
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public string ImportFolder
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public string NeuCount
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public string GeaendertCount
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("LogoffCommand", new RelayCommand(p1 => this.LogoffHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("BackAboutCommand", new RelayCommand(p1 => this.BackHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("ExportSyncCommand", new RelayCommand(p1 => this.ExportSyncHandler(p1), p2 => this.CanExportSyncHandler(p2)));
            this.CmdAgg.AddOrSetCommand("ImportSyncCommand", new RelayCommand(p1 => this.ImportSyncHandler(p1), p2 => this.CanImportSyncHandler(p2)));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.IsUCLoaded = true;
            this.IsImportAllRows = false;
        }

        private bool CanExportSyncHandler(object args)
        {
            if (string.IsNullOrEmpty(this.ExportFolder) == false)
            {
                if (Directory.Exists(this.ExportFolder) == true)
                {
                    return true;
                }
            }

            return false;
        }

        private void ExportSyncHandler(object args)
        {
            string exportSyncFile = string.Empty;
            if (string.IsNullOrEmpty(this.ExportFolder) == true)
            {
                object value = this.notificationService.NoFolderForSync(SyncDirection.SyncExport);
                return;
            }

            try
            {
                this.IsBusyIndicator = true;
                IProgress<double> progress = new Progress<double>(this.UpdateProgressTextExport);
                progress.Report(0);

                exportSyncFile = $"{this.ExportFolder}\\PasswortSync.Tag";
                using (RegionRepository repository = new RegionRepository())
                {
                    if (repository == null || repository.List().Count() == 0)
                    {
                        this.notificationService.NoDataForSync();
                        progress.Report(0);
                        this.IsBusyIndicator = false;
                        return;
                    }

                    List<Region> source = repository.List().ToList();
                    foreach (Region row in source)
                    {
                        row.SyncItemStatus = 0;
                        row.SyncHash = this.RegionExportHash(row);
                    }

                    source.ToJson<Region>(exportSyncFile);
                }

                progress.Report(0.33);

                exportSyncFile = $"{this.ExportFolder}\\PasswortSync.Passwort";
                using (PasswordPinRepository repository = new PasswordPinRepository())
                {
                    if (repository == null || repository.List().Count() == 0)
                    {
                        this.notificationService.NoDataForSync();
                        progress.Report(0);
                        this.IsBusyIndicator = false;
                        return;
                    }

                    List<PasswordPin> source = repository.List().ToList();
                    foreach (PasswordPin row in source)
                    {
                        row.SyncItemStatus = 0;
                        row.SyncHash = this.PasswordPinExportHash(row);

                        if (repository.DatabaseIntern.FileStorage.Exists(row.Id.ToString()) == true)
                        {
                            string attachmentPath = Path.Combine(this.ExportFolder, "Attachments");
                            if (Directory.Exists(attachmentPath) == false)
                            {
                                Directory.CreateDirectory(attachmentPath);
                            }

                            MemoryStream stream = new MemoryStream();
                            LiteFileInfo<string> file = repository.DatabaseIntern.FileStorage.FindById(row.Id.ToString());
                            file.CopyTo(stream);

                            if (stream.ToArray().Length > 1_000)
                            {
                                System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                                if (image != null)
                                {
                                    string imageTyp = image.RawFormat.ToString().ToLower();
                                    if (imageTyp == "png")
                                    {
                                        image.Save($"{attachmentPath}\\{row.Id.ToString()}.{imageTyp}", ImageFormat.Png);
                                    }
                                    else if (imageTyp == "jpeg")
                                    {
                                        image.Save($"{attachmentPath}\\{row.Id.ToString()}.jpg", ImageFormat.Jpeg);
                                    }
                                    else if (imageTyp == "bmp")
                                    {
                                        image.Save($"{attachmentPath}\\{row.Id.ToString()}.{imageTyp}", ImageFormat.Bmp);
                                    }
                                    else if (imageTyp == "gif")
                                    {
                                        image.Save($"{attachmentPath}\\{row.Id.ToString()}.{imageTyp}", ImageFormat.Gif);
                                    }
                                    else if (imageTyp == "tiff")
                                    {
                                        image.Save($"{attachmentPath}\\{row.Id.ToString()}.{imageTyp}", ImageFormat.Tiff);
                                    }
                                    else if (imageTyp == "ico")
                                    {
                                        image.Save($"{attachmentPath}\\{row.Id.ToString()}.{imageTyp}", ImageFormat.Icon);
                                    }
                                    else
                                    {
                                        File.WriteAllBytes($"{attachmentPath}\\{row.Id.ToString()}.jpg", stream.ToArray());
                                    }
                                }
                            }
                        }
                    }

                    source.ToJson<PasswordPin>(exportSyncFile);
                }

                progress.Report(0.66);

                exportSyncFile = $"{this.ExportFolder}\\PasswortSync.Attachment";
                using (AttachmentRepository repository = new AttachmentRepository())
                {
                    if (repository == null || repository.List().Count() == 0)
                    {
                        progress.Report(0);
                        this.IsBusyIndicator = false;
                        return;
                    }

                    List<Attachment> source = repository.List().ToList();
                    foreach (Attachment row in source)
                    {
                        row.SyncItemStatus = 0;
                        row.SyncHash = this.AttachmentExportHash(row);
                    }

                    source.ToJson<Attachment>(exportSyncFile);
                }

                progress.Report(1.00);

                this.IsBusyIndicator = false;
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private bool CanImportSyncHandler(object args)
        {
            if (string.IsNullOrEmpty(this.ImportFolder) == false)
            {
                if (Directory.Exists(this.ImportFolder) == true)
                {
                    return true;
                }
            }

            return false;
        }

        private void ImportSyncHandler(object p1)
        {
            if (string.IsNullOrEmpty(this.ImportFolder) == true)
            {
                this.notificationService.NoFolderForSync(SyncDirection.SyncImport);
                return;
            }

            if (this.IsImportAllRows == true)
            {
                this.ImportSyncAllRows();
            }
            else
            {
                this.ImportSyncChangedRows();
            }
        }

        private void ImportSyncAllRows()
        {
            string importSyncFile = string.Empty;

            try
            {
                importSyncFile = $"{this.ImportFolder}\\PasswortSync.Tag";
                if (File.Exists(importSyncFile) == false)
                {
                    return;
                }

                string jsonText = File.ReadAllText(importSyncFile);
                List<Region> importRegion = jsonText.JsonToList<Region>();
                using (RegionRepository repository = new RegionRepository())
                {
                    repository.DeleteAll();

                    int newRowsCount = 0;
                    foreach (Region row in importRegion.Cast<Region>().Where(w => w.SyncItemStatus == 0))
                    {
                        Region regionNew = new Region();
                        regionNew.Id = row.Id;
                        regionNew.Name = row.Name;
                        regionNew.Description = row.Description;
                        regionNew.Background = row.Background;
                        regionNew.Symbol = row.Symbol;
                        regionNew.LastExport = row.LastExport;
                        regionNew.CreatedBy = UserInfo.TS().CurrentUser;
                        regionNew.CreatedOn = UserInfo.TS().CurrentTime;
                        repository.Add(regionNew);

                        newRowsCount++;
                    }

                    this.NeuCount = $"Tag: {newRowsCount}";
                }

                importSyncFile = $"{this.ImportFolder}\\PasswortSync.Passwort";
                if (File.Exists(importSyncFile) == false)
                {
                    return;
                }

                jsonText = File.ReadAllText(importSyncFile);
                List<PasswordPin> importPasswotPin = jsonText.JsonToList<PasswordPin>();

                using (PasswordPinRepository repository = new PasswordPinRepository())
                {
                    repository.DeleteAll();

                    foreach (PasswordPin row in importPasswotPin.Cast<PasswordPin>().Where(w => w.SyncItemStatus == 0))
                    {
                        PasswordPin passwortPinNew = new PasswordPin();
                        passwortPinNew.Id = row.Id;
                        passwortPinNew.AccessTyp = row.AccessTyp;
                        passwortPinNew.Title = row.Title;
                        passwortPinNew.IsAttachment = false;
                        passwortPinNew.ShowDescription = row.ShowDescription;
                        passwortPinNew.Username = row.Username;
                        passwortPinNew.Passwort = row.Passwort;
                        passwortPinNew.Pin = row.Pin;
                        passwortPinNew.Website = row.Website;
                        passwortPinNew.Symbol = row.Symbol;
                        passwortPinNew.Background = row.Background;
                        passwortPinNew.CompanyId = row.CompanyId;
                        passwortPinNew.Company = row.Company;
                        passwortPinNew.CompanyInfoMail = row.CompanyInfoMail;
                        passwortPinNew.LicenseName = row.LicenseName;
                        passwortPinNew.LicenseKey = row.LicenseKey;
                        passwortPinNew.Region = row.Region;
                        passwortPinNew.LastExport = row.LastExport;
                        passwortPinNew.ShowLast = row.ShowLast;
                        passwortPinNew.IsShowLast = row.IsShowLast;
                        passwortPinNew.SyncItemStatus = row.SyncItemStatus;
                        passwortPinNew.SyncHash = string.Empty;
                        passwortPinNew.CreatedBy = UserInfo.TS().CurrentUser;
                        passwortPinNew.CreatedOn = UserInfo.TS().CurrentTime;
                        repository.Add(passwortPinNew);
                    }

                    string attachmentPath = Path.Combine(this.ImportFolder, "Attachments");
                    if (Directory.Exists(attachmentPath) == true)
                    {
                        List<string> attachmentFiles = Directory.GetFiles(attachmentPath).ToList();
                        foreach (string file in attachmentFiles)
                        {
                            Guid photoId = Guid.Parse(Path.GetFileNameWithoutExtension(file));
                            byte[] photo = File.ReadAllBytes(file);
                            if (photo?.Length > 0)
                            {
                                using (MemoryStream stream = new MemoryStream())
                                {
                                    stream.Write(photo, 0, photo.Length);
                                    stream.Position = 0;
                                    repository.DatabaseIntern.FileStorage.Delete(photoId.ToString());
                                    LiteFileInfo<string> fileInfo = repository.DatabaseIntern.FileStorage.Upload(photoId.ToString(), "Unbekannt", stream);
                                }
                            }
                        }
                    }
                }

                using (ChangeTrackingRepository repository = new ChangeTrackingRepository())
                {
                    repository.DeleteAll();
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private void ImportSyncChangedRows()
        {
            string importSyncFile = string.Empty;

            try
            {
                /* Import von Tag */
                importSyncFile = $"{this.ImportFolder}\\PasswortSync.Tag";
                if (File.Exists(importSyncFile) == false)
                {
                    return;
                }

                string jsonText = File.ReadAllText(importSyncFile);
                List<Region> importRegion = jsonText.JsonToList<Region>();
                using (RegionRepository repository = new RegionRepository())
                {
                    IEnumerable<Region> fromDB = repository.List();
                    foreach (Region row in fromDB)
                    {
                        row.SyncHash = this.RegionExportHash(row);
                    }

                    IEnumerable<Region> differentRows = importRegion.Except<Region>(fromDB, new HashRegionComparer());
                    foreach (Region row in differentRows)
                    {
                        if (fromDB.Any(a => a.Id == row.Id) == true)
                        {
                            repository.Update(row);
                        }
                        else
                        {
                            repository.Add(row);
                        }
                    }
                }

                /* Import von Passwörtern */
                importSyncFile = $"{this.ImportFolder}\\PasswortSync.Passwort";
                if (File.Exists(importSyncFile) == false)
                {
                    return;
                }

                jsonText = File.ReadAllText(importSyncFile);
                List<PasswordPin> importPassword = jsonText.JsonToList<PasswordPin>();
                using (PasswordPinRepository repository = new PasswordPinRepository())
                {
                    IEnumerable<PasswordPin> fromDB = repository.List();
                    foreach (PasswordPin row in fromDB)
                    {
                        row.SyncHash = this.PasswordPinExportHash(row);
                    }

                    IEnumerable<PasswordPin> differentRows = importPassword.Except<PasswordPin>(fromDB, new HashPasswordComparer());
                    foreach (PasswordPin row in differentRows)
                    {
                        if (fromDB.Any(a => a.Id == row.Id) == true)
                        {
                            repository.Update(row);
                        }
                        else
                        {
                            repository.Add(row);
                        }
                    }
                }

                /* Import von Attachments */
                importSyncFile = $"{this.ImportFolder}\\PasswortSync.Attachment";
                if (File.Exists(importSyncFile) == false)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private void LogoffHandler(object p1)
        {
            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = FunctionButtons.Login,
            });
        }

        private void BackHandler(object p1)
        {
            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = FunctionButtons.MainOverview,
            });
        }

        private string RegionExportHash(Region row)
        {
            string fieldHash = $"{row.Name}|{row.Description}|{row.Background}|{row.Symbol}";
            return fieldHash.RemoveWhitespace().ToMD5();
        }

        private string PasswordPinExportHash(PasswordPin row)
        {
            string fieldHash = $"{row.Title}|{row.Description}|{row.Username}|{row.Passwort}|{row.Pin}|{row.Background}|{row.Symbol}|{row.Region}";
            return fieldHash.RemoveWhitespace().ToMD5();
        }

        private string AttachmentExportHash(Attachment row)
        {
            string fieldHash = $"{row.Id}|{row.ObjectId}|{row.ObjectName}|{row.Filename}|{row.FileSize}";
            return fieldHash.RemoveWhitespace().ToMD5();
        }

        private DataTable GetNullFilledDataTableForXML(DataTable dtSource)
        {
            DataTable dtTarget = dtSource.Clone();
            foreach (DataColumn col in dtTarget.Columns)
            {
                col.DataType = typeof(string);
            }

            int colCountInTarget = dtTarget.Columns.Count;
            foreach (DataRow sourceRow in dtSource.Rows)
            {
                DataRow targetRow = dtTarget.NewRow();
                targetRow.ItemArray = sourceRow.ItemArray;

                for (int ctr = 0; ctr < colCountInTarget; ctr++)
                {
                    if (targetRow[ctr] == DBNull.Value)
                    {
                        targetRow[ctr] = string.Empty;
                    }
                }

                dtTarget.Rows.Add(targetRow);
            }

            return dtTarget;
        }

        private void UpdateProgressTextExport(double percentage)
        {
            this.ProgressBarValueExport = percentage * 100;
            this.ProgressBarTextExport = (percentage).ToString("0%");
        }

        private DataTable ReadXMLFromRegion(string file)
        {
            file.IsArgumentNullOrEmpty("Das Argument 'file' darf nicht leer sein.");

            DataTable table = null;

            try
            {
                if (File.Exists(file) == true)
                {
                    string jsonText = File.ReadAllText(file);
                    table = jsonText.JsonToDataTable<Region>(nameof(Region));
                    return table;

                    /*
                    using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        table.Columns.Add("Id", typeof(Guid));
                        table.Columns.Add("Name", typeof(string));
                        table.Columns.Add("Description", typeof(string));
                        table.Columns.Add("Background", typeof(string));
                        table.Columns.Add("Symbol", typeof(int));
                        table.Columns.Add("LastExport", typeof(DateTime));
                        table.Columns.Add("SyncItemStatus", typeof(int));
                        table.Columns.Add("Hash", typeof(string));
                        table.Columns.Add("CreatedBy", typeof(string));
                        table.Columns.Add("CreatedOn", typeof(DateTime));
                        table.Columns.Add("ModifiedBy", typeof(string));
                        table.Columns.Add("ModifiedOn", typeof(DateTime));

                        table.ReadXml(file);

                        return table;
                    }
                    */
                }
                else
                {
                    return table;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                throw;
            }
        }

        private DataTable ReadXMLFromPassword(string file)
        {
            file.IsArgumentNullOrEmpty("Das Argument 'file' darf nicht leer sein.");

            DataTable table = new DataTable(nameof(PasswordPin));

            try
            {
                if (File.Exists(file) == true)
                {
                    using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        table.Columns.Add("Id", typeof(Guid));
                        table.Columns.Add("AccessTyp", typeof(int));
                        table.Columns.Add("Title", typeof(string));
                        table.Columns.Add("Description", typeof(string));
                        table.Columns.Add("ShowDescription", typeof(bool));
                        table.Columns.Add("Username", typeof(string));
                        table.Columns.Add("Passwort", typeof(string));
                        table.Columns.Add("Pin", typeof(string));
                        table.Columns.Add("Symbol", typeof(string));
                        table.Columns.Add("Background", typeof(string));
                        table.Columns.Add("CompanyId", typeof(Guid));
                        table.Columns.Add("Background", typeof(string));
                        table.Columns.Add("Region", typeof(string));
                        table.Columns.Add("LicenseName", typeof(string));
                        table.Columns.Add("LicenseKey", typeof(string));
                        table.Columns.Add("SyncItemStatus", typeof(int));
                        table.Columns.Add("LastExport", typeof(DateTime));
                        table.Columns.Add("Hash", typeof(string));
                        table.Columns.Add("CreatedBy", typeof(string));
                        table.Columns.Add("CreatedOn", typeof(DateTime));
                        table.Columns.Add("ModifiedBy", typeof(string));
                        table.Columns.Add("ModifiedOn", typeof(DateTime));
                        table.ReadXml(stream);

                        return table;
                    }
                }
                else
                {
                    return table;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                throw;
            }
        }

        public bool IsPng(byte[] array)
        {
            return array != null
                && array.Length > 8
                && array[0] == 0x89
                && array[1] == 0x50
                && array[2] == 0x4e
                && array[3] == 0x47
                && array[4] == 0x0d
                && array[5] == 0x0a
                && array[6] == 0x1a
                && array[7] == 0x0a;
        }

        public static bool IsJpeg(byte[] array)
        {
            return array != null
                && array.Length > 2
                && array[0] == 0xff
            && array[1] == 0xd8;
        }
    }

    public sealed class HashRegionComparer : IEqualityComparer<Region>
    {
        public bool Equals(Region x, Region y)
        {
            if (x == null)
            {
                return y == null;
            }
            else if (y == null)
            {
                return false;
            }
            else
            {
                return x.SyncHash == y.SyncHash;
            }
        }

        public int GetHashCode(Region obj)
        {
            return obj.SyncHash.GetHashCode();
        }
    }

    public sealed class HashPasswordComparer : IEqualityComparer<PasswordPin>
    {
        public bool Equals(PasswordPin x, PasswordPin y)
        {
            if (x == null)
            {
                return y == null;
            }
            else if (y == null)
            {
                return false;
            }
            else
            {
                return x.SyncHash == y.SyncHash;
            }
        }

        public int GetHashCode(PasswordPin obj)
        {
            return obj.SyncHash.GetHashCode();
        }
    }
}
