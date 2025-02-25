namespace PasswortNET.Views.ContentControls
{
    using System;
    using System.Data;
    using System.Windows;
    using System.Windows.Controls;

    using ModernBaseLibrary.Extension;

    using ModernIU.Controls;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.DataRepository;
    using PasswortNET.Model;

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

        public double ProgressBarValue
        {
            get => base.GetValue<double>();
            set => base.SetValue(value);
        }

        public string ProgressBarText
        {
            get => base.GetValue<string>();
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
        }

        private bool CanExportSyncHandler(object args)
        {
            if (string.IsNullOrEmpty(this.ExportFolder) == false)
            {
                return true;
            }

            return false;
        }

        private void ExportSyncHandler(object args)
        {
            string exportSyncFile = string.Empty;
            if (string.IsNullOrEmpty(this.ExportFolder) == true)
            {
                this.notificationService.NoFolderForSync(SyncDirection.SyncExport);
                return;
            }

            try
            {
                exportSyncFile = $"{this.ExportFolder}\\PasswortSync.Tag";
                using (RegionRepository repository = new RegionRepository())
                {
                    if (repository == null || repository.List().Count() == 0)
                    {
                        this.notificationService.NoDataForSync();
                        return;
                    }

                    DataTable dtRegion = repository.List().ToDataTable<Region>();
                    dtRegion.TableName = nameof(Region);
                    dtRegion.Columns.Remove("Timestamp");
                    dtRegion.Columns.Remove("Fullname");
                    dtRegion.Columns.Add("Hash", typeof(string));
                    foreach (DataRow row in dtRegion.Rows)
                    {
                        row.SetField<int>("SyncItemStatus", 0);
                        string fieldHash = this.RegionExportHash(row);
                        row.SetField<string>("Hash", fieldHash);
                    }

                    dtRegion = this.GetNullFilledDataTableForXML(dtRegion);
                    dtRegion.WriteXml(exportSyncFile);
                }

                exportSyncFile = $"{this.ExportFolder}\\PasswortSync.Passwort";
                using (PasswordPinRepository repository = new PasswordPinRepository())
                {
                    if (repository == null || repository.List().Count() == 0)
                    {
                        this.notificationService.NoDataForSync();
                        return;
                    }

                    DataTable dtPasswort = repository.List().ToDataTable<PasswordPin>();
                    dtPasswort.TableName = nameof(PasswordPin);
                    dtPasswort.Columns.Remove("Timestamp");
                    dtPasswort.Columns.Remove("Fullname");
                    dtPasswort.Columns.Add("Hash", typeof(string));
                    foreach (DataRow row in dtPasswort.Rows)
                    {
                        row.SetField<int>("SyncItemStatus", 0);
                        string fieldHash = this.PasswordPinExportHash(row);
                        row.SetField<string>("Hash", fieldHash);
                    }

                    dtPasswort = this.GetNullFilledDataTableForXML(dtPasswort);
                    dtPasswort.WriteXml(exportSyncFile);
                }

                exportSyncFile = $"{this.ExportFolder}\\PasswortSync.Attachment";
                using (AttachmentRepository repository = new AttachmentRepository())
                {
                    if (repository == null || repository.List().Count() == 0)
                    {
                        return;
                    }

                    DataTable dtAttachment = repository.List().ToDataTable<Attachment>();
                    dtAttachment.TableName = nameof(Attachment);
                    dtAttachment.Columns.Remove("Timestamp");
                    dtAttachment.Columns.Remove("Fullname");
                    dtAttachment.Columns.Add("Hash", typeof(string));
                    foreach (DataRow row in dtAttachment.Rows)
                    {
                        row.SetField<int>("SyncItemStatus", 0);
                        string fieldHash = this.PasswordPinExportHash(row);
                        row.SetField<string>("Hash", fieldHash);
                    }

                    dtAttachment = this.GetNullFilledDataTableForXML(dtAttachment);
                    dtAttachment.WriteXml(exportSyncFile);
                }
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
                return true;
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
        }

        private void LogoffHandler(object p1)
        {
            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = MainButton.Login,
            });
        }

        private void BackHandler(object p1)
        {
            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = MainButton.Home,
            });
        }

        private string RegionExportHash(DataRow row)
        {
            string fieldHash = $"{row.Field<string>("Name")}|{row.Field<string>("Description")}|{row.Field<string>("Background")}|{row.Field<int>("Symbol")}";
            return fieldHash.RemoveWhitespace().ToMD5();
        }

        private string PasswordPinExportHash(DataRow row)
        {
            string fieldHash = $"{row.Field<string>("Title")}|{row.Field<string>("Description")}|{row.Field<string>("Username")}|{row.Field<string>("Passwort")}|{row.Field<string>("Pin")}|{row.Field<string>("Background")}|{row.Field<int>("Symbol")}|{row.Field<string>("Region")}";
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
                        targetRow[ctr] = String.Empty;
                    }
                }

                dtTarget.Rows.Add(targetRow);
            }

            return dtTarget;
        }
    }
}
