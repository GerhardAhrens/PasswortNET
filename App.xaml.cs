﻿namespace PasswortNET
{
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Threading;

    using PasswortNET.Core;
    using PasswortNET.DataRepository;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string DEFAULTLANGUAGE = "de-DE";
        private const string SHORTNAME = "PW";
        private static readonly string MessageBoxTitle = "PasswortNET Application";
        private static readonly string UnexpectedError = "An unexpected error occured.";
        private string exePath = string.Empty;
        private string exeName = string.Empty;

        public App()
        {
            try
            {
                using (DatabaseBackup db = new DatabaseBackup())
                {
                    db.CheckAndRun();
                }

                /* Name der EXE Datei*/
                exeName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
                /* Pfad der EXE-Datei*/
                exePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                /* Synchronisieren einer Textenigabe mit dem primären Windows (wegen Validierung von Eingaben)*/
                FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;

                /* Alle nicht explicit abgefangene Exception spätesten hier abfangen und anzeigen */
                this.DispatcherUnhandledException += this.OnDispatcherUnhandledException;
            }
            catch (Exception ex)
            {
                ex.Data.Add("UserDomainName", Environment.UserDomainName);
                ex.Data.Add("UserName", Environment.UserName);
                ex.Data.Add("exePath", exePath);
                ErrorMessage(ex, "General Error: ");
                ApplicationExit();
            }
        }

        /// <summary>
        /// Festlegung für Abfrage des Programmendedialog
        /// </summary>
        public static bool ExitApplicationQuestion { get; set; }

        /// <summary>
        /// Festlegung für das Speichern der Position des Main-Windows
        /// </summary>
        public static bool SaveLastWindowsPosition { get; set; }

        /// <summary>
        /// Festlegung für die aktuelle Laufzeitumgebung der Applikation
        /// </summary>
        public static int RunEnvironment { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                /* Initalisierung Spracheinstellung */
                InitializeCultures(DEFAULTLANGUAGE);
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                ErrorMessage(ex, "General Error: ");
                ApplicationExit();
            }
        }


        public static void ErrorMessage(Exception ex, string message = "")
        {
            string expMsg = ex.Message;
            var aex = ex as AggregateException;

            if (aex != null && aex.InnerExceptions.Count == 1)
            {
                expMsg = aex.InnerExceptions[0].Message;
            }

            if (string.IsNullOrEmpty(message) == true)
            {
                message = UnexpectedError;
            }

            StringBuilder errorText = new StringBuilder();
            if (ex.Data != null && ex.Data.Count > 0)
            {
                foreach (DictionaryEntry item in ex.Data)
                {
                    errorText.AppendLine($"{item.Key} : {item.Value}");
                }
            }

            MessageBox.Show(
                message + $"{expMsg}\n{ex.Message}\n{errorText.ToString()}",
                MessageBoxTitle,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public static void InfoMessage(string message)
        {
            MessageBox.Show(
                message,
                MessageBoxTitle,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private void InitializeCultures(string language)
        {
            if (string.IsNullOrEmpty(language) == false)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(DEFAULTLANGUAGE);
            }

            if (string.IsNullOrEmpty(language) == false)
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(DEFAULTLANGUAGE);
            }

            FrameworkPropertyMetadata frameworkMetadata = new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(new CultureInfo(language).IetfLanguageTag));
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), frameworkMetadata);
        }

        /// <summary>
        /// Screen zum aktualisieren zwingen, Globale Funktion
        /// </summary>
        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Debug.WriteLine($"{exeName}-{(e.Exception as Exception).Message}");
        }

        /// <summary>
        /// Programmende erzwingen
        /// </summary>
        public static void ApplicationExit()
        {
            Application.Current.Shutdown();
            Process.GetCurrentProcess().Kill();
        }
    }
}
