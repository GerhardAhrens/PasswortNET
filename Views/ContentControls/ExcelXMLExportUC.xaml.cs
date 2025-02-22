namespace PasswortNET.Views.ContentControls
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Threading;

    using ModernBaseLibrary.Core;
    using ModernBaseLibrary.ExcelWriter;
    using ModernBaseLibrary.Extension;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.Core.Enums;
    using PasswortNET.DataRepository;
    using PasswortNET.Model;

    using static ModernBaseLibrary.ExcelWriter.Style;

    /// <summary>
    /// Interaktionslogik für ExcelXMLExportUC.xaml
    /// </summary>
    public partial class ExcelXMLExportUC : UserControlBase
    {
        public ExcelXMLExportUC() : base(typeof(ExcelXMLExportUC))
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

        public bool IsAllRows
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value);
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

        public string ExportFile
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public Dictionary<ExportImportFormat, string> FormatSource
        {
            get => base.GetValue<Dictionary<ExportImportFormat, string>>();
            set => base.SetValue(value);
        }

        public ExportImportFormat FormatSelected
        {
            get => base.GetValue<ExportImportFormat>();
            set => base.SetValue(value, this.FormatCallBack);
        }

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("BackAboutCommand", new RelayCommand(p1 => this.BackHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("LogoffCommand", new RelayCommand(p1 => this.LogoffHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("ExportCommand", new RelayCommand(p1 => this.ExportHandler(), p2 => this.CanExportHandler()));

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.IsUCLoaded = true;
            this.BuildDataSource();
        }

        private void BuildDataSource()
        {
            if (this.IsUCLoaded == false)
            {
                return;
            }

            this.IsAllRows = true;
            this.IsBusyIndicator = false;
            this.FormatSelected = ExportImportFormat.None;
            this.FormatSource = new EnumDescripionToDictionary<ExportImportFormat>();
        }

        private void FormatCallBack(ExportImportFormat format, string arg2)
        {
            if (format == ExportImportFormat.None)
            {
                this.ExportFile = string.Empty;
            }
            else
            {
                this.ExportFile = $"Export_{DateTime.Now.ToString("yyyyMMdd")}.{format.ToLowerString()}";
            }
        }

        private bool CanExportHandler()
        {
            if (string.IsNullOrEmpty(this.ExportFile) == false)
            {
                if (this.ExportFile.ToLower().ContainsAll(".xml", ".xlsx") == true)
                {
                    return true;
                }
            }

            return false;
        }

        private async void ExportHandler()
        {
            this.IsBusyIndicator = true;

            IProgress<double> progress = new Progress<double>(this.UpdateProgressText);
            _ = await GenerateItems((a) => this.BuildExport(progress), progress);

            this.IsBusyIndicator = false;
        }

        private void BuildExport(IProgress<double> progress)
        {
            try
            {
                progress.Report(0);
                if (this.IsAllRows == false)
                {

                }
                else
                {
                    this.CreateAllRows(progress);
                }

                progress.Report(1);
            }
            catch (FileLockException ex)
            {
                App.ErrorMessage(ex, ex.Message);
                App.Current.Shutdown(0);
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private void BuildExportXML(DataTable dtPassword, IProgress<double> progress)
        {
            int maxRecords = dtPassword.Rows.Count;
            for (int rowIndex = 0; rowIndex < maxRecords; rowIndex++)
            {
                double percentage = (double)rowIndex / maxRecords;
                progress.Report(percentage);

                AccessTyp accessTyp = dtPassword.Rows[rowIndex]["AccessTyp"].ToInt().ToEnum<AccessTyp>();
                dtPassword.Rows[rowIndex]["AccessTypText"] = accessTyp.ToDescription();
                Thread.Sleep(100);
            }

           dtPassword.Columns.Remove("AccessTyp");
           dtPassword.WriteXml(this.ExportFile);
            if (File.Exists(this.ExportFile) == true)
            {
                this.Run();
            }
        }

        private void BuildExportExcel(DataTable dtPassword, IProgress<double> progress)
        {
            if (IsFileLocked(this.ExportFile) == true)
            {
                return;
            }

            int maxRecords = dtPassword.Rows.Count;
            for (int rowIndex = 0; rowIndex < maxRecords; rowIndex++)
            {
                double percentage = (double)rowIndex / maxRecords;
                progress.Report(percentage);

                AccessTyp accessTyp = dtPassword.Rows[rowIndex]["AccessTyp"].ToInt().ToEnum<AccessTyp>();
                dtPassword.Rows[rowIndex]["AccessTypText"] = accessTyp.ToDescription();
                Thread.Sleep(100);
            }

            dtPassword.Columns.Remove("AccessTyp");
            progress.Report(0);

            using (ExcelWriter workbook = new ExcelWriter(this.ExportFile, nameof(PasswordPin)))
            {
                ModernBaseLibrary.ExcelWriter.Style s = new ModernBaseLibrary.ExcelWriter.Style();
                s.CurrentFill.SetColor(this.BrushToHex(Colors.Transparent), ModernBaseLibrary.ExcelWriter.Style.Fill.FillType.fillColor);

                int columnIndex = 0;
                foreach (DataColumn column in dtPassword.Columns)
                {
                    workbook.CurrentWorksheet.AddNextCell(column.ColumnName,s);
                    workbook.CurrentWorksheet.SetColumnWidth(columnIndex, 20f);
                    columnIndex++;
                }

                workbook.CurrentWorksheet.SetAutoFilter(0, dtPassword.Columns.Count-1);
                workbook.CurrentWorksheet.GoToNextRow();
                int rowIndex = 0;
                foreach (DataRow row in dtPassword.Rows)
                {
                    foreach (DataColumn column in dtPassword.Columns)
                    {
                        try
                        {
                            if (column.DataType == typeof(string))
                            {
                                string value = Convert.ToString(row[column.ColumnName]);
                                workbook.CurrentWorksheet.AddNextCell(value,s);
                            }
                            else if (column.DataType == typeof(DateTime))
                            {
                                DateTime value = Convert.ToDateTime(row[column.ColumnName]);
                                workbook.CurrentWorksheet.AddNextCell(value,s);
                            }
                            else if (column.DataType == typeof(double))
                            {
                                double value = Convert.ToDouble(row[column.ColumnName]);
                                workbook.CurrentWorksheet.AddNextCell(value,s);
                            }
                            else if (column.DataType == typeof(decimal))
                            {
                                decimal value = Convert.ToDecimal(row[column.ColumnName]);
                                workbook.CurrentWorksheet.AddNextCell(value,s);
                            }
                            else if (column.DataType == typeof(int))
                            {
                                int value = Convert.ToInt32(row[column.ColumnName]);
                                workbook.CurrentWorksheet.AddNextCell(value,s);
                            }
                            else if (column.DataType == typeof(bool))
                            {
                                bool value = Convert.ToBoolean(row[column.ColumnName]);
                                workbook.CurrentWorksheet.AddNextCell(value,s);
                            }
                            else
                            {
                                string value = Convert.ToString(row[column.ColumnName]);
                                workbook.CurrentWorksheet.AddNextCell(value,s);
                            }
                        }
                        catch (Exception ex)
                        {
                            string errorText = ex.Message;
                            throw;
                        }
                    }

                    workbook.CurrentWorksheet.GoToNextRow();

                    rowIndex++;
                    double percentage = (double)rowIndex / maxRecords;
                    progress.Report(percentage);
                }

                workbook.Save();
            }

            if (File.Exists(this.ExportFile) == true)
            {
                this.Run();
            }
        }

        private Task<bool> GenerateItems(Action<IProgress<double>> buildExport, IProgress<double> progress)
        {
            return Task.Run(() =>
            {
                buildExport(progress);
                return true;
            });
        }

        private void UpdateProgressText(double percentage)
        {
            this.ProgressBarValue = percentage * 100;
            this.ProgressBarText = (percentage).ToString("0%");

        }

        private void CreateAllRows(IProgress<double> progress)
        {
            using (PasswordPinRepository repository = new PasswordPinRepository())
            {
                IEnumerable<PasswordPin> overviewSource = repository.List();
                if (overviewSource.Count() > 0)
                {
                    DataTable dtPassword = overviewSource.Cast<PasswordPin>().ToDataTable<PasswordPin>();
                    dtPassword.TableName = nameof(PasswordPin);
                    dtPassword.Columns.Add("AccessTypText", typeof(string)).SetOrdinal(0);
                    dtPassword.Columns.Remove("Id");
                    dtPassword.Columns.Remove("ShowDescription");
                    dtPassword.Columns.Remove("Symbol");
                    dtPassword.Columns.Remove("Background");
                    dtPassword.Columns.Remove("CompanyId");
                    dtPassword.Columns.Remove("SyncItemStatus");
                    dtPassword.Columns.Remove("LastExport");
                    dtPassword.Columns.Remove("ShowLast");
                    dtPassword.Columns.Remove("IsShowLast");
                    dtPassword.Columns.Remove("CreatedBy");
                    dtPassword.Columns.Remove("CreatedOn");
                    dtPassword.Columns.Remove("ModifiedBy");
                    dtPassword.Columns.Remove("ModifiedOn");
                    dtPassword.Columns.Remove("Timestamp");
                    dtPassword.Columns.Remove("Fullname");

                    if (Path.GetExtension(this.ExportFile.ToLower()) == ".xml")
                    {
                        this.BuildExportXML(dtPassword, progress);
                    }
                    else if (Path.GetExtension(this.ExportFile.ToLower()) == ".xlsx")
                    {
                        this.BuildExportExcel(dtPassword, progress);
                    }
                }
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

        private bool IsFileLocked(string file)
        {
            try
            {
                if (File.Exists(file) == true)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi != null)
                    {
                        using (FileStream stream = fi.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            stream.Close();
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }

        public void Run()
        {
            if (File.Exists(this.ExportFile) == true)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "explorer.exe";
                startInfo.Arguments = $"/e, /select, \"{this.ExportFile}\"";
                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    try
                    {
                        process.Start();
                    }
                    catch (Exception ex)
                    {
                        ex.Data.Add("Filename", this.ExportFile);
                        throw;
                    }
                }
            }
        }

        private string BrushToHex(Color color)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = color;
            return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", brush.Color.A,brush.Color.R, brush.Color.G, brush.Color.B);
        }
    }
}
