namespace PasswortNET.Views.ContentControls
{
    using System.Windows.Controls;
    using System.Windows;

    using ModernUI.MVVM.Base;
    using System.Windows.Input;
    using PasswortNET.Core;
    using ModernBaseLibrary.Core;
    using PasswortNET.DataRepository;
    using PasswortNET.Model;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Windows.Data;
    using ModernBaseLibrary.Extension;
    using LiteDB;

    /// <summary>
    /// Interaktionslogik für AuditTrailDetailUC.xaml
    /// </summary>
    public partial class AuditTrailDetailUC : UserControlBase
    {
        public AuditTrailDetailUC(ChangeViewEventArgs args) : base(typeof(AuditTrailDetailUC))
        {
            this.InitializeComponent();

            this.Id = args.EntityId;
            this.InitCommands();

            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Unloaded", this.OnUcUnloaded);
            this.DataContext = this;
        }

        #region Properties
        public string Title
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public ICollectionView DialogDataView
        {
            get => base.GetValue<ICollectionView>();
            set => base.SetValue(value);
        }

        public ChangeTracking CurrentSelectedItem
        {
            get => base.GetValue<ChangeTracking>();
            set => base.SetValue(value);
        }

        public int RowCount
        {
            get => base.GetValue<int>();
            set => base.SetValue(value);
        }

        private Guid Id { get; set; }
        #endregion Properties

        public override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("BackCommand", new RelayCommand(p1 => this.BackHandler(p1), p2 => true));
        }

        #region UserControl Events
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this);
            this.LoadDataHandler();
        }

        private void OnUcUnloaded(object sender, RoutedEventArgs e)
        {
            this.DialogDataView = null;
        }
        #endregion UserControl Events

        #region Load Data
        private void LoadDataHandler()
        {
            try
            {
                using (ObjectRuntime objectRuntime = new ObjectRuntime())
                {
                    using (PasswordPinRepository repository = new PasswordPinRepository())
                    {
                        PasswordPin item = repository.GetById(this.Id);
                        if (item != null)
                        {
                            this.Title = item.Title;
                        }
                    }

                    using (ChangeTrackingRepository repository = new ChangeTrackingRepository())
                    {
                        IEnumerable<ChangeTracking> overviewSource = repository.List(i => i.ObjectId == this.Id.ToString()).OrderByDescending(o => o.Timestamp);
                        if (overviewSource != null)
                        {
                            this.DialogDataView = CollectionViewSource.GetDefaultView(overviewSource);
                            if (this.DialogDataView != null)
                            {
                                this.DialogDataView.MoveCurrentToFirst();
                                this.RowCount = this.DialogDataView.Count<ChangeTracking>();
                            }
                        }
                    }

                    StatusbarMain.Statusbar.SetNotification($"Bereit: {objectRuntime.ResultMilliseconds()}ms");
                }
            }
            catch (FileLockException ex)
            {
                App.ErrorMessage(ex);
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }
        #endregion Load Data

        #region Command Handler
        private void BackHandler(object p1)
        {
            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
            {
                Sender = this.GetType().Name,
                EntityId = this.Id,
                MenuButton = FunctionButtons.PasswordDetail,
            });
        }
        #endregion Command Handler
    }
}
