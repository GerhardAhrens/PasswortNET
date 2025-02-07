//-----------------------------------------------------------------------
// <copyright file="ProgressBarColorConverter.cs" company="Lifeprojects.de">
//     Class: ProgressBarColorConverter
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>14.03.2022</date>
//
// <summary>
// Converter Class wandelt die Text-Hintergrundfarbe vom ProgressBar ab 0% bis 45%, 45% bis 55%, ab 55% in eine andere Farbe um
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Views.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    [ValueConversion(typeof(int), typeof(Brush))]
    public class ProgressBarColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush result = Brushes.White;
            if (value != null && value is double)
            {
                if ((double)value <= 45)
                {
                    result = Brushes.Blue;
                }
                else if ((double)value > 45 && (double)value <= 55)
                {
                    result = Brushes.Green;
                }
                else if ((double)value > 55)
                {
                    result = Brushes.White;
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