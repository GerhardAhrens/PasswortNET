﻿namespace PasswortNET.Views.ContentControls
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    using ModernBaseLibrary.Core;
    using ModernBaseLibrary.Extension;

    using ModernIU.Controls;

    using ModernUI.MVVM.Base;

    using PasswortNET.Core;
    using PasswortNET.Core.Enums;
    using PasswortNET.DataRepository;
    using PasswortNET.Model;

    /// <summary>
    /// Interaktionslogik für LoginUC.xaml
    /// </summary>
    public partial class LoginUC : UserControlBase
    {
        private INotificationService notificationService = new NotificationService();

        public LoginUC() : base(typeof(LoginUC))
        {
            this.InitializeComponent();

            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<UserControl, MouseWheelEventArgs>.AddHandler(this, "PreviewMouseWheel", this.OnPreviewMouseWheel);
            this.InitCommands();
            this.DataContext = this;
        }

        public string LoginUser
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public string LoginPasswort
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public bool IsPasswordRepeat
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value);
        }

        private int MaxTryLogin { get; set; } = 3;


        public override void InitCommands()
        {
            base.CmdAgg.AddOrSetCommand("CloseWindowCommand", new RelayCommand(p1 => this.CloseWindowHandler(p1), p2 => true));
            base.CmdAgg.AddOrSetCommand("LoginCommand", new RelayCommand(p1 => this.LoginHandler(p1), p2 => true));
            base.CmdAgg.AddOrSetCommand("InputLoginCommand", new RelayCommand(p1 => this.InputLoginHandler(p1), p2 => true));
            base.CmdAgg.AddOrSetCommand("ScalePlusCommand", new RelayCommand(p1 => this.OnScaleFactor(p1), p2 => true));
            base.CmdAgg.AddOrSetCommand("ScaleSubtractCommand", new RelayCommand(p1 => this.OnScaleFactor(p1), p2 => true));
            base.CmdAgg.AddOrSetCommand("ScaleNeutralCommand", new RelayCommand(p1 => this.OnScaleFactor(p1), p2 => true));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            WeakEventManager<IconTextBox, KeyEventArgs>.AddHandler(this.TxtBenutzername, "KeyDown", this.OnKeyDown);
            WeakEventManager<MPasswordBox, KeyEventArgs>.AddHandler(this.TxtPassword, "KeyDown", this.OnKeyDown);
            WeakEventManager<MPasswordBox, KeyEventArgs>.AddHandler(this.TxtPasswordRepeat, "KeyDown", this.OnKeyDown);

            StatusbarMain.Statusbar.SetNotification("Geben Sie einen Benutzer und ein  Passwort an.");

            using (ApplicationSettings settings = new ApplicationSettings())
            {
                if (settings.IsExitSettings() == false)
                {
                    settings.LastAccess = DateTime.Now;
                    settings.LastUser = UserInfo.TS().CurrentDomainUser;
                    settings.RunEnvironment = 0;
                    settings.ExitApplicationQuestion = false;
                    settings.SaveLastWindowsPosition = false;
                    settings.Hash = string.Empty;
                    settings.DatabaseBackupFullname = DatabaseName.FullBackupName;
                    settings.DatabaseFullname = DatabaseName.FullDatabaseName;
                    settings.Save();
                }

                settings.Load();

                App.ExitApplicationQuestion = settings.ExitApplicationQuestion;
                App.SaveLastWindowsPosition = settings.SaveLastWindowsPosition;
                App.RunEnvironment = settings.RunEnvironment;
            }

            this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => { this.TxtBenutzername.Focus(); }));

            using (ApplicationSettings settings = new ApplicationSettings())
            {
                if (settings.IsExitSettings() == true)
                {
                    settings.Load();

                    if (string.IsNullOrEmpty(settings.Hash) == true)
                    {
                        this.IsPasswordRepeat = true;
                    }
                    else
                    {
                        this.IsPasswordRepeat = false;
                    }
                }
            }
        }

        private void LoginHandler(object p1)
        {
            const int MAXLOGIN = 3;
            RunEnvironments runEnvironment = RunEnvironments.None;
            string databaseFile = string.Empty;
            bool isFirstStart = false;
            string userName = this.LoginUser;
            string passwort = this.TxtPassword.Password;
            string passwortRepeat = this.TxtPasswordRepeat.Password;
            string hash = $"{userName}{passwort}".ToMD5(false);
            string encryptHash = hash.Encrypt();
            string compareHash = string.Empty;

            using (ApplicationSettings settings = new ApplicationSettings())
            {
                if (settings.IsExitSettings() == true)
                {
                    settings.Load();

                    if (string.IsNullOrEmpty(settings.Hash) == true)
                    {
                        isFirstStart = true;
                        settings.Hash = encryptHash;
                        string ctrlHash = $"{userName}|{passwort}";
                        settings.ControlHash = ctrlHash.Encrypt();
                        settings.DatabaseFullname = DatabaseName.FullDatabaseName;
                        settings.RunEnvironment = (int)runEnvironment;
                        settings.Save();
                        compareHash = settings.Hash.Decrypt();
                        databaseFile = settings.DatabaseFullname;
                        runEnvironment = settings.RunEnvironment.ToEnum<RunEnvironments>();
                    }
                    else
                    {
                        isFirstStart = false;
                        compareHash = settings.Hash.Decrypt();
                        if (string.IsNullOrEmpty(settings.DatabaseFullname) == false)
                        {
                            databaseFile = settings.DatabaseFullname;
                            runEnvironment = settings.RunEnvironment.ToEnum<RunEnvironments>();
                        }
                        else
                        {
                            settings.DatabaseFullname = DatabaseName.FullDatabaseName;
                            settings.RunEnvironment = (int)runEnvironment;
                            settings.Save();

                            databaseFile = DatabaseName.FullDatabaseName;
                            runEnvironment = settings.RunEnvironment.ToEnum<RunEnvironments>();
                        }
                    }
                }
            }

            if (isFirstStart == true)
            {
                if (string.Equals(passwort, passwortRepeat) == false)
                {
                    this.notificationService.PasswortRepeatWrong();
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => { this.TxtPassword.Focus(); }));
                    return;
                }
            }

            if (string.Equals(hash, compareHash) == false)
            {
                this.MaxTryLogin--;
                if (this.MaxTryLogin == 0)
                {
                    this.notificationService.MaxTryLogin(MAXLOGIN);
                    this.CloseWindowHandler(null);
                }

                NotificationBoxButton result = this.notificationService.BenutzerPasswortFalsch(this.MaxTryLogin);
                if (result == NotificationBoxButton.Yes)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => { this.TxtBenutzername.Focus(); }));
                    return;
                }
                else
                {
                    this.CloseWindowHandler(null);
                }
            }

            if (File.Exists(databaseFile) == true)
            {
                using (DatabaseManager dm = new DatabaseManager(databaseFile))
                {
                    Result<DatabaseInfo> dbi = dm.CheckDatabase();
                    StatusbarMain.Statusbar.Notification = $"Bereit: {dbi.ElapsedTime}ms";
                    StatusbarMain.Statusbar.SetDatabaeInfo(databaseFile, runEnvironment);
                }
            }
            else
            {
                using (DatabaseManager dm = new DatabaseManager(databaseFile))
                {
                    Result<DatabaseInfo> dbi = dm.CheckDatabase();
                    StatusbarMain.Statusbar.Notification = $"Bereit: {dbi.ElapsedTime}ms";
                    StatusbarMain.Statusbar.SetDatabaeInfo(databaseFile, runEnvironment);
                }
                int lastSortItem = 0;
                using (RegionRepository repository = new RegionRepository())
                {
                    if (repository.List().Any() == true)
                    {
                        lastSortItem = repository.List().ToList().Max<Region>(m => m.ItemSorting) + 1;
                    }
                    else
                    {
                        lastSortItem = 1;
                    }

                    Region r = new Region();
                    r.Name = "Alle";
                    r.CreatedBy = UserInfo.TS().CurrentUser;
                    r.CreatedOn = UserInfo.TS().CurrentTime;
                    r.Background = ColorConverters.ConvertBrushToName(Brushes.Transparent);
                    r.Id = Guid.NewGuid();
                    r.ItemSorting = lastSortItem;
                    repository.Add(r);

                    lastSortItem = repository.List().ToList().Max<Region>(m => m.ItemSorting) + 1;
                    Region r1 = new Region();
                    r1.Name = "Zuletzt gesehen";
                    r1.CreatedBy = UserInfo.TS().CurrentUser;
                    r1.CreatedOn = UserInfo.TS().CurrentTime;
                    r1.Background = ColorConverters.ConvertBrushToName(Brushes.Transparent);
                    r1.Id = Guid.NewGuid();
                    r1.ItemSorting = lastSortItem;
                    repository.Add(r1);
                }
            }

            base.EventAgg.Publish<ChangeViewEventArgs>(new ChangeViewEventArgs
                {
                    Sender = this.GetType().Name,
                    MenuButton = FunctionButtons.MainOverview,
                    LoginHash = hash,
                });
        }

        private void CloseWindowHandler(object p1)
        {
            Window mainWindow = this.Parent.TryFindParent<Window>();
            if (mainWindow != null)
            {
                mainWindow.Close();
            }
        }

        private void InputLoginHandler(object p1)
        {
            using (ApplicationSettings settings = new ApplicationSettings())
            {
                settings.Load();
                string ctrlHash = settings.ControlHash.Decrypt();
                this.LoginUser = ctrlHash.Split('|').FirstOrDefault();
                this.TxtPassword.Text = ctrlHash.Split('|').LastOrDefault();
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            IconTextBox it = sender as IconTextBox;
            if (it != null)
            {
                if (it.Name == "TxtBenutzername" && e.Key == Key.Tab)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => { this.TxtPassword.Focus(); }));
                }
            }

            MPasswordBox mp = sender as MPasswordBox;
            if (mp != null)
            {
                if (mp.Name == "TxtPassword" && e.Key == Key.Tab && IsPasswordRepeat == true)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => { this.TxtPasswordRepeat.Focus(); }));
                }
                else if (mp.Name == "TxtPassword" && e.Key == Key.Tab && IsPasswordRepeat == false)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => { this.TxtBenutzername.Focus(); }));
                }
                else if (mp.Name == "TxtPasswordRepeat" && e.Key == Key.Tab)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => { this.TxtBenutzername.Focus(); }));
                }
            }
        }

        private void OnScaleFactor(object commandParam)
        {
            if (commandParam.ToString().ToUpper() == "ADD")
            {
                if (this.Scalefactor.ScaleX <= 3.0)
                {
                    this.Scalefactor.ScaleX = this.Scalefactor.ScaleX + 0.25;
                    this.Scalefactor.ScaleY = this.Scalefactor.ScaleY + 0.25;
                }
            }
            else if (commandParam.ToString().ToUpper() == "SUBTRACT")
            {
                if (this.Scalefactor.ScaleX > 1.35)
                {
                    this.Scalefactor.ScaleX = this.Scalefactor.ScaleX - 0.25;
                    this.Scalefactor.ScaleY = this.Scalefactor.ScaleY - 0.25;
                }
            }
            else if (commandParam.ToString().ToUpper() == "NEUTRAL")
            {
                if (this.Scalefactor.ScaleX != 1.35)
                {
                    this.Scalefactor.ScaleX = 1.35;
                    this.Scalefactor.ScaleY = 1.35;
                }
            }
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) == true)
            {
                if (e.Delta > 0)
                {
                    if (this.Scalefactor.ScaleX <= 2.0)
                    {
                        this.Scalefactor.ScaleX = this.Scalefactor.ScaleX + 0.25;
                        this.Scalefactor.ScaleY = this.Scalefactor.ScaleY + 0.25;
                    }
                }

                if (e.Delta < 0)
                {
                    if (this.Scalefactor.ScaleX > 1.35)
                    {
                        this.Scalefactor.ScaleX = this.Scalefactor.ScaleX - 0.25;
                        this.Scalefactor.ScaleY = this.Scalefactor.ScaleY - 0.25;
                    }
                }
            }
        }
    }
}
