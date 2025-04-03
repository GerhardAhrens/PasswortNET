namespace PasswortNET.Views.ContentControls
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing.Printing;
    using System.Management;
    using System.Printing;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    using ModernBaseLibrary.Extension;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.Core.Enums;
    using PasswortNET.Model;

    /// <summary>
    /// Interaktionslogik für PrintUC.xaml
    /// </summary>
    public partial class PrintUC : UserControlBase
    {
        private const string FONTNAME = "Segoe UI";

        public PrintUC() : base(typeof(PrintUC))
        {
            this.InitializeComponent();

            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);

            this.InitCommands();
            this.DataContext = this;
        }

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("BackAboutCommand", new RelayCommand(this.BackHandler));
            this.CmdAgg.AddOrSetCommand("PrintPasswortCommand", new RelayCommand(this.PrintPasswortHandler));
            this.CmdAgg.AddOrSetCommand("PrintLicenseCommand", new RelayCommand(this.PrintLicenseHandler));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            Keyboard.Focus(this);
            this.IsUCLoaded = true;
        }

        private void BackHandler(object p1)
        {
            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = FunctionButtons.MainOverview,
            });
        }

        private void PrintPasswortHandler(object obj)
        {
            DataTable dataTable = MainWindow.DialogDataView.ToDataTable<PasswordPin>(x => x.AccessTyp != AccessTyp.License,x => x.AccessTyp);
            dataTable.Columns.Remove("Id");
            //dataTable.Columns.Remove("AccessTyp");
            dataTable.Columns.Remove("ShowDescription");
            dataTable.Columns.Remove("Website");
            dataTable.Columns.Remove("CompanyId");
            dataTable.Columns.Remove("Company");
            dataTable.Columns.Remove("CompanyInfoMail");
            dataTable.Columns.Remove("SyncItemStatus");
            dataTable.Columns.Remove("LastExport");
            dataTable.Columns.Remove("ShowLast");
            dataTable.Columns.Remove("IsShowLast");
            dataTable.Columns.Remove("LicenseName");
            dataTable.Columns.Remove("LicenseKey");
            dataTable.Columns.Remove("FullName");
            dataTable.Columns.Remove("Timestamp");
            dataTable.Columns.Remove("Symbol");
            dataTable.Columns.Remove("Background");
            dataTable.Columns.Remove("CreatedBy");
            dataTable.Columns.Remove("CreatedOn");
            dataTable.Columns.Remove("ModifiedBy");
            dataTable.Columns.Remove("ModifiedOn");
            dataTable.Columns.Remove("IsAttachment");
            dataTable.Columns.Remove("SyncHash");
            dataTable.Columns.Remove("ToSearchFilter");

            PrintDialog printDialog = new PrintDialog();
            printDialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PageRangeSelection = PageRangeSelection.AllPages;
                printDialog.UserPageRangeEnabled = false;
                PrintQueue printQueue = printDialog.PrintQueue;
                Tuple<bool,string,string> printerState = this.CheckPrinterState(printQueue);

                if (printerState.Item1 == true)
                {
                    return;
                }


                var headerList = dataTable.Columns.Cast<DataColumn>().Select(e => e.ColumnName.ToString()).ToList();
                FlowDocument fd = new FlowDocument();
                fd.PageHeight = 768;
                fd.PageWidth = 1104;
                fd.IsColumnWidthFlexible = true;

                PageMediaSize pageMediaSize = new PageMediaSize(fd.PageWidth, fd.PageHeight);
                PrintTicket pt = printQueue.DefaultPrintTicket;
                pt.PageMediaSize = pageMediaSize;
                pt.PageOrientation = PageOrientation.Landscape;
                pt.PageOrder = PageOrder.Standard;

                IDocumentPaginatorSource docPaginatorSource = fd as IDocumentPaginatorSource;
                WeakEventManager<DocumentPaginator, AsyncCompletedEventArgs>.AddHandler(docPaginatorSource.DocumentPaginator, "ComputePageCountCompleted", this.OnComputePageCountCompleted);

                fd.PagePadding = new Thickness(50);
                fd.PageWidth = printDialog.PrintableAreaWidth;
                fd.PageHeight = printDialog.PrintableAreaHeight;
                fd.BringIntoView();
                fd.TextAlignment = TextAlignment.Center;
                fd.ColumnGap = 0;
                fd.ColumnWidth = (fd.PageWidth - fd.ColumnGap - fd.PagePadding.Left - fd.PagePadding.Right);

                Paragraph paragraphTitle = new Paragraph(new Run("Password Manager"));
                paragraphTitle.FontStyle = FontStyles.Normal;
                paragraphTitle.FontFamily = new FontFamily(FONTNAME);
                paragraphTitle.FontSize = 20;
                paragraphTitle.TextAlignment = TextAlignment.Center;
                fd.Blocks.Add(paragraphTitle);

                Paragraph paragraphFrom = new Paragraph(new Run($"Gedruckt am: {DateTime.Now.ToShortDateString()}"));
                paragraphFrom.FontStyle = FontStyles.Normal;
                paragraphFrom.FontFamily = new FontFamily(FONTNAME);
                paragraphFrom.FontSize = 10;
                paragraphFrom.TextAlignment = TextAlignment.Right;
                fd.Blocks.Add(paragraphFrom);

                Table table = new Table();
                table.CellSpacing = 0;
                table.BorderBrush = Brushes.White;
                table.BorderThickness = new Thickness(1);
                table.Margin = new Thickness(16, 0, 16, 16);

                /* Spaltenkopf */
                TableColumn categoryCol = new TableColumn();
                TableColumn titelCol = new TableColumn();
                TableColumn descriptionCol = new TableColumn();
                TableColumn userCol = new TableColumn();
                TableColumn passwortCol = new TableColumn();
                TableColumn pinCol = new TableColumn();
                TableColumn regionCol = new TableColumn();

                categoryCol.Width = new GridLength(1, GridUnitType.Auto);
                titelCol.Width = new GridLength(2, GridUnitType.Auto);
                descriptionCol.Width = new GridLength(3, GridUnitType.Auto);
                userCol.Width = new GridLength(1, GridUnitType.Auto);
                passwortCol.Width = new GridLength(1, GridUnitType.Auto);
                pinCol.Width = new GridLength(1, GridUnitType.Auto);
                regionCol.Width = new GridLength(1, GridUnitType.Auto);

                table.Columns.Add(categoryCol);
                table.Columns.Add(titelCol);
                table.Columns.Add(descriptionCol);
                table.Columns.Add(userCol);
                table.Columns.Add(passwortCol);
                table.Columns.Add(pinCol);
                table.Columns.Add(regionCol);

                TableRowGroup headerGroup = new TableRowGroup();
                headerGroup.Background = Brushes.LightGray;
                TableRow headerRow = new TableRow();

                headerRow.Cells.Add(this.CreateHeaderCell("Kategorie", false, false));
                headerRow.Cells.Add(this.CreateHeaderCell("Titel", false, false));
                headerRow.Cells.Add(this.CreateHeaderCell("Beschreibung", false, false));
                headerRow.Cells.Add(this.CreateHeaderCell("Benutzername", false, false));
                headerRow.Cells.Add(this.CreateHeaderCell("Passwort", false, false));
                headerRow.Cells.Add(this.CreateHeaderCell("Pin", false, false));
                headerRow.Cells.Add(this.CreateHeaderCell("Gruppe", true, false));
                headerGroup.Rows.Add(headerRow);
                table.RowGroups.Add(headerGroup);

                /* Datenspalten */
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    TableRowGroup resultGroup = new TableRowGroup();
                    TableRow resultRow = new TableRow();

                    bool lastBottom = (i == (dataTable.Rows.Count - 1));

                    DataRow row = dataTable.Rows[i];
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        string cellText = string.Empty;
                        if (j == 0)
                        {
                            AccessTyp accessTyp = (AccessTyp)row.ItemArray[j].ToInt();
                            cellText = accessTyp.ToString();
                        }
                        else
                        {
                            cellText = row.ItemArray[j].ToString();
                        }

                        resultRow.Cells.Add(CreateCell(cellText, lastBottom, lastBottom));
                    }

                    resultGroup.Rows.Add(resultRow);
                    table.RowGroups.Add(resultGroup);
                }

                fd.Blocks.Add(table);
                printDialog.PrintDocument((docPaginatorSource).DocumentPaginator, "Password Manager");
            }

        }

        private void PrintLicenseHandler(object obj)
        {
            DataTable dataTable = MainWindow.DialogDataView.ToDataTable<PasswordPin>(x => x.AccessTyp == AccessTyp.License, x => x.Title);
            dataTable.Columns.Remove("Id");
            dataTable.Columns.Remove("AccessTyp");
            dataTable.Columns.Remove("ShowDescription");
            dataTable.Columns.Remove("Website");
            dataTable.Columns.Remove("CompanyId");
            dataTable.Columns.Remove("Company");
            dataTable.Columns.Remove("CompanyInfoMail");
            dataTable.Columns.Remove("SyncItemStatus");
            dataTable.Columns.Remove("LastExport");
            dataTable.Columns.Remove("ShowLast");
            dataTable.Columns.Remove("IsShowLast");
            dataTable.Columns.Remove("Username");
            dataTable.Columns.Remove("Passwort");
            dataTable.Columns.Remove("Passwort");
            dataTable.Columns.Remove("Pin");
            dataTable.Columns.Remove("Timestamp");
            dataTable.Columns.Remove("Symbol");
            dataTable.Columns.Remove("Background");
            dataTable.Columns.Remove("CreatedBy");
            dataTable.Columns.Remove("CreatedOn");
            dataTable.Columns.Remove("ModifiedBy");
            dataTable.Columns.Remove("ModifiedOn");
            dataTable.Columns.Remove("IsAttachment");
            dataTable.Columns.Remove("SyncHash");
            dataTable.Columns.Remove("ToSearchFilter");
        }

        private TableCell CreateHeaderCell(string text, bool lastRight, bool lastBottom)
        {
            TableCell tableCell = new TableCell();
            tableCell.BorderBrush = Brushes.LightGray;
            tableCell.BorderThickness = new Thickness(0, 0, lastRight == true ? 0 : 1, lastBottom == true ? 0 : 1);

            Paragraph cellPara = new Paragraph();
            cellPara.FontFamily = new FontFamily(FONTNAME);
            cellPara.FontSize = 11;
            cellPara.Padding = new Thickness(1);
            cellPara.FontWeight = FontWeights.Bold;
            cellPara.BorderBrush = Brushes.Black;
            cellPara.Background = Brushes.LightGray;
            cellPara.Foreground = Brushes.Black;
            cellPara.BorderThickness = new Thickness(0, 0, 1, 1);

            cellPara.Inlines.Add(new Run(text));
            tableCell.Blocks.Add(cellPara);

            return tableCell;
        }

        private TableCell CreateCell(string text, int colSpan, int rowSpan, bool lastRight, bool lastBottom, bool filled, bool boldText, int fontSize = 0)
        {
            TableCell tableCell = new TableCell();
            if (filled)
            {
                tableCell.Background = Brushes.DimGray;
            }
            tableCell.BorderBrush = Brushes.DimGray;
            tableCell.BorderThickness = new Thickness(0, 0, lastRight ? 0 : 1, lastBottom ? 0 : 1);

            if (colSpan > 0)
            {
                tableCell.ColumnSpan = colSpan;
            }

            if (rowSpan > 0)
            {
                tableCell.RowSpan = rowSpan;
            }

            Paragraph cellPara = new Paragraph();
            cellPara.FontFamily = new FontFamily(FONTNAME);
            cellPara.FontSize = 11;
            cellPara.KeepTogether = true;
            if (boldText == true)
            {
                cellPara.FontWeight = FontWeights.Bold;
            }

            if (fontSize > 1)
            {
                cellPara.FontSize = fontSize;
            }

            cellPara.Inlines.Add(new Run(text));
            tableCell.Blocks.Add(cellPara);

            return tableCell;
        }

        private TableCell CreateCell(string text, bool lastRight, bool lastBottom)
        {
            TableCell tableCell = new TableCell();
            tableCell.Background = Brushes.Transparent;
            tableCell.BorderBrush = Brushes.Gray;
            tableCell.BorderThickness = new Thickness(1, 1, lastRight ? 0 : 1, lastBottom ? 0 : 1);

            Paragraph cellPara = new Paragraph();
            cellPara.FontFamily = new FontFamily(FONTNAME);
            cellPara.FontSize = 11;
            cellPara.KeepTogether = true;
            cellPara.Inlines.Add(new Run(text));
            tableCell.Blocks.Add(cellPara);

            return tableCell;
        }

        private TableCell CreateCell(int number, bool lastRight, bool lastBottom, bool boldText = false)
        {
            TableCell tableCell = new TableCell();
            tableCell.BorderBrush = Brushes.Gray;
            tableCell.BorderThickness = new Thickness(0, 0, lastRight ? 0 : 1, lastBottom ? 0 : 1);

            Paragraph cellPara = new Paragraph();
            cellPara.FontFamily = new FontFamily(FONTNAME);
            cellPara.FontSize = 11;
            cellPara.Inlines.Add(new Run(number.ToString()));
            if (boldText)
            {
                cellPara.FontWeight = FontWeights.Bold;
            }

            tableCell.Blocks.Add(cellPara);

            return tableCell;
        }

        private void OnComputePageCountCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
        }

        private Tuple<bool,string,string> CheckPrinterState(PrintQueue printQueue)
        {
            Tuple<bool, string, string> result = new Tuple<bool,string, string>(false,string.Empty, string.Empty);

            if (printQueue == null)
            {
                return result;
            }

            string[] PrinterStatuses = {"Other", "Unknown", "Idle", "Printing", "WarmUp", "Stopped Printing", "Offline"};

            string[] PrinterStates = 
                { 
                "Paused", "Error", "Pending Deletion", "Paper Jam",
                "Paper Out", "Manual Feed", "Paper Problem",
                "Offline", "IO Active", "Busy", "Printing",
                "Output Bin Full", "Not Available", "Waiting",
                "Processing", "Initialization", "Warming Up",
                "Toner Low", "No Toner", "Page Punt",
                "User Intervention Required", "Out of Memory",
                "Door Open", "Server_Unknown", "Power Save" };

            string statusText = string.Empty;
            string stateText = "Unknown";

            ManagementObjectSearcher searcher = new
            ManagementObjectSearcher($"SELECT * FROM Win32_Printer WHERE Name like '%{printQueue.Name}%'");
            foreach (ManagementObject printer in searcher.Get())
            {
                var name = printer.GetPropertyValue("Name");
                var description = printer.GetPropertyValue("Description");
                var portName = printer.GetPropertyValue("PortName");
                var driverName = printer.GetPropertyValue("DriverName");
                var shared = printer.GetPropertyValue("Shared");
                var status = Int32.Parse(printer.GetPropertyValue("PrinterStatus").ToString());
                statusText = PrinterStatuses[status];

                int state = Int32.Parse(printer["PrinterState"].ToString());
                if (state < 30)
                {
                    stateText = PrinterStates[state];
                }

                var isDefault = printer.GetPropertyValue("Default");
                var isNetworkPrinter = printer.GetPropertyValue("Network");
                bool isOffline = printer["WorkOffline"].ToString().ToBool();
                if (name.ToString().Contains("PDF") == true)
                {
                    result = new Tuple<bool,string, string>(true,"Other", "Processing");
                }
                else
                {
                    result = new Tuple<bool,string, string>(isOffline,statusText, stateText);
                }
            }

            return result;
        }
    }
}
