namespace PasswortNET.Views.ContentControls
{
    using System;
    using System.Data;
    using System.Management;
    using System.Printing;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.Core.Enums;

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
            this.CmdAgg.AddOrSetCommand("PrintCommand", new RelayCommand(this.PrintHandler));
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

        private void PrintHandler(object obj)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                PrintQueue printQueue = printDialog.PrintQueue;
                PrinterState printerState = this.CheckPrinterState(printQueue);

                PrintTicket pt = printQueue.UserPrintTicket;
                pt.PageOrientation = PageOrientation.Landscape;

                /*var headerList = dataTable.Columns.Cast<DataColumn>().Select(e => e.ColumnName.ToString()).ToList();*/
                FlowDocument fd = new FlowDocument();

                fd.PageHeight = 768;
                fd.PageWidth = 1104;

                PageMediaSize pageMediaSize = new PageMediaSize(fd.PageWidth, fd.PageHeight);
                pt.PageMediaSize = pageMediaSize;

                fd.IsColumnWidthFlexible = true;

                IDocumentPaginatorSource docPaginatorSource = fd as IDocumentPaginatorSource;

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
                TableRowGroup tableRowGroup = new TableRowGroup();
                TableRow tableRow = new TableRow();
                fd.PagePadding = new Thickness(50);
                fd.PageWidth = printDialog.PrintableAreaWidth;
                fd.PageHeight = printDialog.PrintableAreaHeight;
                fd.BringIntoView();

                fd.TextAlignment = TextAlignment.Center;
                fd.ColumnGap = 0;
                fd.ColumnWidth = (fd.PageWidth - fd.ColumnGap - fd.PagePadding.Left - fd.PagePadding.Right);
                table.CellSpacing = 0;

                printDialog.PrintDocument((docPaginatorSource).DocumentPaginator, "Password Manager");
            }

        }

        private PrinterState CheckPrinterState(PrintQueue printQueue)
        {
            /*https://developpaper.com/c-realize-printer-status-query-and-blocking-printing/*/

            PrinterState printerStatus = PrinterState.None;
            if (printQueue == null)
            {
                return printerStatus;
            }

            ManagementObjectSearcher searcher = new
            ManagementObjectSearcher($"SELECT * FROM Win32_Printer WHERE Name = '{printQueue.Name}'");
            foreach (ManagementObject printer in searcher.Get())
            {
                var name = printer.GetPropertyValue("Name");
                var status = printer.GetPropertyValue("Status");
                var isDefault = printer.GetPropertyValue("Default");
                var isNetworkPrinter = printer.GetPropertyValue("Network");
                int state = Int32.Parse(printer["ExtendedPrinterStatus"].ToString());
                printerStatus = (PrinterState)state;

                int errorState = Int32.Parse(printer["DetectedErrorState"].ToString());
                bool isOffline = printer["WorkOffline"].ToString().ToLower().Equals("false");
                if (name.ToString().Contains("PDF") == true)
                {
                    printerStatus = PrinterState.Other;
                }
            }

            return printerStatus;
        }
    }
}
