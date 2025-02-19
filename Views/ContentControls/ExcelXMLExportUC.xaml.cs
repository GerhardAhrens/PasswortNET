namespace PasswortNET.Views.ContentControls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    using ModernBaseLibrary.Core;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.Core.Enums;

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

        public double ProgressBarValue
        {
            get => base.GetValue<double>();
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
            this.CmdAgg.AddOrSetCommand("LogoffCommand", new RelayCommand(p1 => this.LogoffHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("ExportCommand", new RelayCommand(p1 => this.ExportHandler(), p2 => true));

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
                this.ExportFile = $"Export.{format}";
            }
        }

        private void ExportHandler()
        {
            this.ProgressBarValue = 0;

            for (int i = 0; i < 1000000; i++)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Input,
                    new Action(() =>
                    {
                        System.Threading.Thread.Sleep(100);
                        this.ProgressBarValue = i/10000;
                    }));
            }

            this.ProgressBarValue = 0;
        }

        private void LogoffHandler(object p1)
        {
            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                MenuButton = MainButton.Login,
            });
        }
    }
}
