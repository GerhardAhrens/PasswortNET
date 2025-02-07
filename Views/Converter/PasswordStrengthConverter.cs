//-----------------------------------------------------------------------
// <copyright file="PasswordStrengthConverter.cs" company="Lifeprojects.de">
//     Class: PasswordStrengthConverter
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>14.03.2022</date>
//
// <summary>
// Die Converter Class gibt bei Änderung des Passwort Text die Passwortstärke als Farbe
// Rot-Orange-Grün zurück
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Views.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    using ModernIU.Controls;

    [ValueConversion(typeof(int), typeof(SolidColorBrush))]
    public class PasswordStrengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush result = Brushes.Transparent;

            if (value != null && value is string pwd)
            {
                int id = PasswordGenerator.PasswordStrength(pwd);
                if (id == 1)
                {
                    result = Brushes.Red;
                }
                else if (id == 2)
                {
                    result = Brushes.Orange;
                }
                else if (id == 3)
                {
                    result = Brushes.Green;
                }
                else
                {
                    result = Brushes.Transparent;
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}