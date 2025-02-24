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
            this.CmdAgg.AddOrSetCommand("ImportSyncCommand", new RelayCommand(p1 => this.ImportSyncHandler(p1), p2 => true));
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
                this.notificationService.NoDataForSync();
                return;
            }

            try
            {
                exportSyncFile = $"{this.ExportFolder}\\PasswortSync.Tag";
                using (RegionRepository repository = new RegionRepository())
                {
                    if (repository == null || repository.List().Count() == 0)
                    {
                        return;
                    }

                    DataTable dtRegion = repository.List().ToDataTable<Region>();
                    dtRegion.TableName = nameof(Region);
                    dtRegion.Columns.Remove("Timestamp");
                    dtRegion.Columns.Remove("Fullname");
                    dtRegion.Columns.Add("SyncItemStatus", typeof(int));
                    dtRegion.Columns.Add("Hash", typeof(string));
                    foreach (DataRow row in dtRegion.Rows)
                    {
                        row.SetField<int>("SyncItemStatus", 0);
                        string fieldHash = this.RegionExportHash(row);
                        row.SetField<string>("Hash", fieldHash);
                    }

                    dtRegion.WriteXml(exportSyncFile);
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private void ImportSyncHandler(object p1)
        {
            if (string.IsNullOrEmpty(this.ImportFolder) == true)
            {
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
    }
}
