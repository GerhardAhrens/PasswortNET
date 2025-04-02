namespace PasswortNET.Core
{
    using System.ComponentModel;

    public enum FunctionButtons : int
    {
        [Description("Keine Auswahl")]
        None = 0,
        [Description("Home Dialog ohne Funktion")]
        Home = 1,
        [Description("Anmeldung durchf�hren, um PasswortNET verwenden zu k�nnen.")]
        Login = 2,
        [Description("Aktueller Benutzer abmelden")]
        Logoff = 3,
        [Description("Benutzer und/oder Passwort �ndern")]
        ChangePassword = 4,
        [Description("Programmeinstellungen, Passwortdatenbank, Tag")]
        AppSettings = 5,
        [Description("Programminformationen und Statistik")]
        About = 6,
        [Description("Zur�ck")]
        Back = 7,
        [Description("�bersicht Passwort/Zug�nge/Pin/Lizenzen")]
        MainOverview = 8,
        [Description("Bearbeiten Passworte")]
        PasswordDetail = 9,
        [Description("Bearbeiten Web-Zug�nge")]
        WebPageDetail = 10,
        [Description("Bearbeiten Pin")]
        PinDetail = 11,
        [Description("Bearbeiten Lizenzen")]
        LicenseDetail = 12,
        [Description("Datenbank synchronisieren")]
        DataSync = 13,
        [Description("Export Passwortdaten")]
        Export = 14,
        [Description("Drucken Passwortdaten, Lizenzen")]
        Print = 15,
        [Description("Hilfe")]
        Help = 16,
        [Description("�nderungsverfolgung")]
        AuditTrail = 17,
    }
}
