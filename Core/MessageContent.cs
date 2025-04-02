//-----------------------------------------------------------------------
// <copyright file="MsgDialog.cs" company="NRM Netzdienste Rhein-Main GmbH">
//     Class: MsgDialog
//     Copyright © NRM Netzdienste Rhein-Main GmbH 2023
// </copyright>
//
// <author>DeveloperName - NRM Netzdienste Rhein-Main GmbH</author>
// <email>DeveloperName@nrm-netzdienste.de</email>
// <date>17.11.2023 16:49:00</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Core
{
    using System;
    using System.Text;
    using ModernBaseLibrary.Text;

    using ModernIU.Controls;

    using PasswortNET.Core.Enums;
    using PasswortNET.Views;

    public static class MessageContent
    {
        public static NotificationBoxButton ApplicationExit(this INotificationService @this)
        {
            (string InfoText, string CustomText, double FontSize) msgText = ("Programm beenden", $"Soll das Programm beendet werden?", 18);
            NotificationBoxButton questionResult = NotificationBoxButton.None;

            @this.ShowDialog<QuestionYesNo>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    questionResult = ((System.Tuple<ModernIU.Controls.NotificationBoxButton>)tag).Item1;
                }
            });

            return questionResult;
        }

        public static NotificationBoxButton ApplicationExit2(this INotificationService @this)
        {
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<html><body scroll=\"no\">");
            htmlContent.Append("<h2 style=\"color:black;\">Soll das Programm beendet werden?</h2>");
            htmlContent.Append($"<h3 style=\"color:blue;\">Datum/Zeit: {DateTime.Now}</h3>");
            htmlContent.Append("</body></html>");

            (string InfoText, string CustomText, double FontSize) msgText = ("Programm beenden", htmlContent.ToString(), 0);
            NotificationBoxButton questionResult = NotificationBoxButton.None;

            @this.ShowDialog<QuestionHtmlYesNo>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    questionResult = ((System.Tuple<ModernIU.Controls.NotificationBoxButton>)tag).Item1;
                }
            });

            return questionResult;
        }

        public static NotificationBoxButton FeaturesNotFound(this INotificationService @this, string featuresText = "")
        {
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<html><body scroll=\"yes\">");
            htmlContent.Append($"<h3 style=\"color:Red;\">Die gew&uuml;nschte Funktion '{featuresText}' steht nicht zur Verf&uuml;gung.</h2>");
            htmlContent.Append($"<h3 style=\"color:black;\">Bei Bedarf kontaktieren Sie den Applikationsverantwortlichen.</h3>");
            htmlContent.Append("</body></html>");

            (string InfoText, string CustomText, double FontSize) msgText = ("Funktion nicht gefunden", htmlContent.ToString(), 0);
            Tuple<NotificationBoxButton, object> resultOK = new Tuple<NotificationBoxButton, object>(NotificationBoxButton.Ok, null);

            @this.ShowDialog<MessageHtmlOk>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    resultOK = (Tuple<NotificationBoxButton, object>)tag;
                }
            });

            return resultOK.Item1;
        }

        public static NotificationBoxButton FeaturesNotFound2(this INotificationService @this, string featuresText = "")
        {
            (string InfoText, string CustomText, double FontSize) msgText = ("Funktion nicht gefunden", $"Die gewünschte Funktion '{featuresText}' steht nicht zur Verfügung\nBei Bedarf kontaktieren Sie den Verantwortlichen der Applikation.",18);
            Tuple<NotificationBoxButton, object> resultOK = new Tuple<NotificationBoxButton, object>(NotificationBoxButton.Ok, null);

            @this.ShowDialog<MessageOk>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    resultOK = (Tuple<NotificationBoxButton, object>)tag;
                }
            });

            return resultOK.Item1;
        }

        #region Login und Change Password
        public static NotificationBoxButton DatebaseNotExist(this INotificationService @this, string fileName)
        {
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<html><body scroll=\"no\">");
            htmlContent.Append($"<h2 style=\"color:red;\">Die Passwort Datenbank wurde nicht gefunden!</h2>");
            htmlContent.Append($"<h3 style=\"color:black;\">Datenbank Datei: {fileName}</h3>");
            htmlContent.Append("</body></html>");

            (string InfoText, string CustomText, double FontSize) msgText = ("Benutzer/Passwort", htmlContent.ToString(), 0);
            Tuple<NotificationBoxButton, object> resultOK = new Tuple<NotificationBoxButton, object>(NotificationBoxButton.Ok, null);

            @this.ShowDialog<MessageHtmlOk>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    resultOK = (Tuple<NotificationBoxButton, object>)tag;
                }
            });

            return resultOK.Item1;
        }

        public static NotificationBoxButton BenutzerPasswortFalsch(this INotificationService @this, int maxCount = 3)
        {
            string countMsg = FormatMessage.Get("Sie noch [ein/{0}] [Versuch/Versuche]", maxCount);

            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<html><body scroll=\"no\">");
            htmlContent.Append("<h2 style=\"color:red;\">Der Benutzer oder das Passwort ist falsch.</h2>");
            htmlContent.Append($"<h3 style=\"color:black;\">Wollen Sie es erneut versuchen? {countMsg}.</h3>");
            htmlContent.Append("</body></html>");

            (string InfoText, string CustomText, double FontSize) msgText = ("Benutzer/Passwort", htmlContent.ToString(), 0);
            NotificationBoxButton questionResult = NotificationBoxButton.None;

            @this.ShowDialog<QuestionHtmlYesNo>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    questionResult = ((System.Tuple<ModernIU.Controls.NotificationBoxButton>)tag).Item1;
                }
            });

            return questionResult;
        }

        public static NotificationBoxButton MaxTryLogin(this INotificationService @this, int maxCount = 3)
        {
            string countMsg = FormatMessage.Get("[einem/{0}] [Versuch/Versuche]", maxCount);

            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<html><body scroll=\"no\">");
            htmlContent.Append($"<h2 style=\"color:red;\">Der Benutzer oder das Passwort ist falsch. Sie haben die maximale Anzahl von {countMsg} erreicht.</h2>");
            htmlContent.Append($"<h3 style=\"color:black;\">Die Anwendung wird beendet.</h3>");
            htmlContent.Append("</body></html>");

            (string InfoText, string CustomText, double FontSize) msgText = ("Benutzer/Passwort", htmlContent.ToString(), 0);
            Tuple<NotificationBoxButton, object> resultOK = new Tuple<NotificationBoxButton, object>(NotificationBoxButton.Ok, null);

            @this.ShowDialog<MessageHtmlOk>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    resultOK = (Tuple<NotificationBoxButton, object>)tag;
                }
            });

            return resultOK.Item1;
        }

        public static NotificationBoxButton PasswortRepeatWrong(this INotificationService @this)
        {
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<html><body scroll=\"no\">");
            htmlContent.Append($"<h2 style=\"color:red;\">Das Passwort und die Passwort Wiederholung stimmen nicht überein.</h2>");
            htmlContent.Append($"<h3 style=\"color:black;\">Prüfen Sie Ihre Eingabe.</h3>");
            htmlContent.Append("</body></html>");

            (string InfoText, string CustomText, double FontSize) msgText = ("Benutzer/Passwort", htmlContent.ToString(), 0);
            Tuple<NotificationBoxButton, object> resultOK = new Tuple<NotificationBoxButton, object>(NotificationBoxButton.Ok, null);

            @this.ShowDialog<MessageHtmlOk>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    resultOK = (Tuple<NotificationBoxButton, object>)tag;
                }
            });

            return resultOK.Item1;
        }

        public static NotificationBoxButton PasswortIsEquals(this INotificationService @this)
        {
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<html><body scroll=\"no\">");
            htmlContent.Append($"<h2 style=\"color:red;\">Die alte und neue Benutzer/Passwort Kombination müssen unterschiedlich sein .</h2>");
            htmlContent.Append($"<h3 style=\"color:black;\">Prüfen Sie Ihre Eingabe.</h3>");
            htmlContent.Append("</body></html>");

            (string InfoText, string CustomText, double FontSize) msgText = ("Benutzer/Passwort", htmlContent.ToString(), 0);
            Tuple<NotificationBoxButton, object> resultOK = new Tuple<NotificationBoxButton, object>(NotificationBoxButton.Ok, null);

            @this.ShowDialog<MessageHtmlOk>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    resultOK = (Tuple<NotificationBoxButton, object>)tag;
                }
            });

            return resultOK.Item1;
        }

        public static NotificationBoxButton NoteForLogoff(this INotificationService @this)
        {
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<html><body scroll=\"no\">");
            htmlContent.Append($"<h2 style=\"color:red;\">Nach der erfolgreichen &Auml;nderung des Passwort muss sich neu angemeldet werden.</h2>");
            htmlContent.Append("</body></html>");

            (string InfoText, string CustomText, double FontSize) msgText = ("Benutzer/Passwort", htmlContent.ToString(), 0);
            Tuple<NotificationBoxButton, object> resultOK = new Tuple<NotificationBoxButton, object>(NotificationBoxButton.Ok, null);

            @this.ShowDialog<MessageHtmlOk>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    resultOK = (Tuple<NotificationBoxButton, object>)tag;
                }
            });

            return resultOK.Item1;
        }

        public static NotificationBoxButton ChangeLoginData(this INotificationService @this)
        {
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<html><body scroll=\"no\">");
            htmlContent.Append($"<h2 style=\"color:black;\">Ihre Logindaten wurden geändert.</h2>");
            htmlContent.Append($"<h3 style=\"color:black;\">Sie müssen sich mit den neuen Logindaten wieder anmelden.</h3>");
            htmlContent.Append("</body></html>");

            (string InfoText, string CustomText, double FontSize) msgText = ("Ändern Benutzer/Passwort", htmlContent.ToString(), 0);
            Tuple<NotificationBoxButton, object> resultOK = new Tuple<NotificationBoxButton, object>(NotificationBoxButton.Ok, null);

            @this.ShowDialog<MessageHtmlOk>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    resultOK = (Tuple<NotificationBoxButton, object>)tag;
                }
            });

            return resultOK.Item1;
        }

        public static NotificationBoxButton ChangeLoginDataWrong(this INotificationService @this)
        {
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<html><body scroll=\"no\">");
            htmlContent.Append($"<h2 style=\"color:black;\">bei der Änderung der Logindaten ist ein Fehler aufgetreten.</h2>");
            htmlContent.Append($"<h3 style=\"color:black;\">Überprüfen Sie ihre Eingaben.</h3>");
            htmlContent.Append("</body></html>");

            (string InfoText, string CustomText, double FontSize) msgText = ("Ändern Benutzer/Passwort", htmlContent.ToString(), 0);
            Tuple<NotificationBoxButton, object> resultOK = new Tuple<NotificationBoxButton, object>(NotificationBoxButton.Ok, null);

            @this.ShowDialog<MessageHtmlOk>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    resultOK = (Tuple<NotificationBoxButton, object>)tag;
                }
            });

            return resultOK.Item1;
        }

        public static NotificationBoxButton SaveNewPasswortYN(this INotificationService @this)
        {
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<html><body scroll=\"no\">");
            htmlContent.Append($"<h2 style=\"color:black;\">Wollen Sie das neue Passwort speichern?</h2>");
            htmlContent.Append("</body></html>");

            (string InfoText, string CustomText, double FontSize) msgText = ("Ändern Benutzer/Passwort", htmlContent.ToString(), 0);
            NotificationBoxButton questionResult = NotificationBoxButton.None;

            @this.ShowDialog<QuestionHtmlYesNo>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    questionResult = ((System.Tuple<ModernIU.Controls.NotificationBoxButton>)tag).Item1;
                }
            });

            return questionResult;
        }

        #endregion Login und Change Password

        #region Bearbeiten von Passwörtern, Webzugänge, Pins
        public static NotificationBoxButton NoRowSelected(this INotificationService @this)
        {
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<html><body scroll=\"yes\">");
            htmlContent.Append($"<h2 style=\"color:Red;\">Kein Eintrag zur Bearbeitung ausgew&auml;hlt.</h2>");
            htmlContent.Append($"<h3 style=\"color:black;\">Sie müssen zur Bearbeitung einen Eintrag ausw&auml;hlen.</h3>");
            htmlContent.Append("</body></html>");

            (string InfoText, string CustomText, double FontSize) msgText = ("Funktion nicht gefunden", htmlContent.ToString(), 0);
            Tuple<NotificationBoxButton, object> resultOK = new Tuple<NotificationBoxButton, object>(NotificationBoxButton.Ok, null);

            @this.ShowDialog<MessageHtmlOk>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    resultOK = (Tuple<NotificationBoxButton, object>)tag;
                }
            });

            return resultOK.Item1;
        }

        public static NotificationBoxButton DeleteSelectedRow(this INotificationService @this, string titel = "")
        {
            (string InfoText, string CustomText, double FontSize) msgText = ("Eintrag löschen", $"\nSoll gewählte Eintrag '{titel}' gelöscht werden?\nDer Eintrag wird danach endgültig aus der Datenbank entfernt.", 18);
            NotificationBoxButton questionResult = NotificationBoxButton.None;

            @this.ShowDialog<QuestionYesNo>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    questionResult = ((System.Tuple<ModernIU.Controls.NotificationBoxButton>)tag).Item1;
                }
            });

            return questionResult;
        }

        public static NotificationBoxButton CopySelectedRow(this INotificationService @this, string titel = "")
        {
            (string InfoText, string CustomText, double FontSize) msgText = ("Eintrag kopieren", $"\nSoll gewählte Eintrag '{titel}' kopiert werden?", 18);
            NotificationBoxButton questionResult = NotificationBoxButton.None;

            @this.ShowDialog<QuestionYesNo>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    questionResult = ((System.Tuple<ModernIU.Controls.NotificationBoxButton>)tag).Item1;
                }
            });

            return questionResult;
        }
        #endregion Bearbeiten von Passwörtern, Webzugänge, Pins

        #region Export, Import, Data Sync
        public static NotificationBoxButton NoFolderForSync(this INotificationService @this, SyncDirection syncDirection)
        {
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<html><body scroll=\"no\">");
            if (syncDirection == SyncDirection.SyncExport)
            {
                htmlContent.Append($"<h2 style=\"color:red;\">Es wurde keine Verzeichnis zum Export der Datenbank ausgewählt.</h2>");
            }
            else if (syncDirection == SyncDirection.SyncExport)
            {
                htmlContent.Append($"<h2 style=\"color:red;\">Es wurde keine Verzeichnis zum Import der Datenbank ausgewählt.</h2>");
            }

            htmlContent.Append($"<h3 style=\"color:black;\">Es werden keine Daten exportiert.</h3>");
            htmlContent.Append("</body></html>");

            (string InfoText, string CustomText, double FontSize) msgText = ("Datenbank Synchronisieren", htmlContent.ToString(), 0);
            Tuple<NotificationBoxButton, object> resultOK = new Tuple<NotificationBoxButton, object>(NotificationBoxButton.Ok, null);

            @this.ShowDialog<MessageHtmlOk>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    resultOK = (Tuple<NotificationBoxButton, object>)tag;
                }
            });

            return resultOK.Item1;
        }

        public static NotificationBoxButton NoDataForSync(this INotificationService @this)
        {
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<html><body scroll=\"no\">");
            htmlContent.Append($"<h2 style=\"color:red;\">Es sind keine Daten zum synchronisieren vorhanden.</h2>");
            htmlContent.Append($"<h3 style=\"color:black;\">Es werden keine Daten exportiert.</h3>");
            htmlContent.Append("</body></html>");

            (string InfoText, string CustomText, double FontSize) msgText = ("Datenbank Synchronisieren", htmlContent.ToString(), 0);
            Tuple<NotificationBoxButton, object> resultOK = new Tuple<NotificationBoxButton, object>(NotificationBoxButton.Ok, null);

            @this.ShowDialog<MessageHtmlOk>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    resultOK = (Tuple<NotificationBoxButton, object>)tag;
                }
            });

            return resultOK.Item1;
        }
        #endregion Export, Import, Data Sync

        #region Working Region Tag
        public static NotificationBoxButton DeleteRegionItem(this INotificationService @this, string msg = "")
        {
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<html><body scroll=\"no\">");
            htmlContent.Append($"<h2 style=\"color:black;\">Soll der Tag {msg} gelöscht werden?</h2>");
            htmlContent.Append("</body></html>");

            (string InfoText, string CustomText, double FontSize) msgText = ("Bearbeiten Tag", htmlContent.ToString(), 0);
            NotificationBoxButton questionResult = NotificationBoxButton.None;

            @this.ShowDialog<QuestionHtmlYesNo>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    questionResult = ((System.Tuple<ModernIU.Controls.NotificationBoxButton>)tag).Item1;
                }
            });

            return questionResult;
        }

        public static NotificationBoxButton DefaultValueNotDelete(this INotificationService @this)
        {
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<html><body scroll=\"no\">");
            htmlContent.Append($"<h2 style=\"color:black;\">Standardwerte können nicht gelöscht werden!</h2>");
            htmlContent.Append($"<h3 style=\"color:black;\">Überprüfen Sie ihre Auswahl.</h3>");
            htmlContent.Append("</body></html>");

            (string InfoText, string CustomText, double FontSize) msgText = ("Bearbeiten Tag", htmlContent.ToString(), 0);
            Tuple<NotificationBoxButton, object> resultOK = new Tuple<NotificationBoxButton, object>(NotificationBoxButton.Ok, null);

            @this.ShowDialog<MessageHtmlOk>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    resultOK = (Tuple<NotificationBoxButton, object>)tag;
                }
            });

            return resultOK.Item1;
        }
        #endregion Working Region Tag

        #region Drucken
        public static NotificationBoxButton PrinterNotReady(this INotificationService @this, string addText = "")
        {
            (string InfoText, string CustomText, double FontSize) msgText = ("Drucken", $"Der gewählte Drucker '{addText}' ist nicht bereit!\nPrüfen Sie den Drucker ob die eingeschaltet ist und Papier eingelegt ist.", 16);
            Tuple<NotificationBoxButton, object> resultOK = new Tuple<NotificationBoxButton, object>(NotificationBoxButton.Ok, null);

            @this.ShowDialog<MessageOk>(msgText, (result, tag) =>
            {
                if (result == true && tag != null)
                {
                    resultOK = (Tuple<NotificationBoxButton, object>)tag;
                }
            });

            return resultOK.Item1;
        }
        #endregion
    }
}
