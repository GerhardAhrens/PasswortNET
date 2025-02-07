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

        #region Login und Change Password
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
        #endregion Login und Change Password
    }
}
