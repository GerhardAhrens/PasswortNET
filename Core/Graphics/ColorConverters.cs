//-----------------------------------------------------------------------
// <copyright file="ColorConverters.cs" company="Lifeprojects.de">
//     Class: ColorConverters
//     Copyright © Lifeprojects.de 2025
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>Gerhard Ahrens@Lifeprojects.de</email>
// <date>27.03.2025 14:27:37</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media;

    public static class ColorConverters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorConverters"/> class.
        /// </summary>
        static ColorConverters()
        {
        }

        public static int ConvertColorNameToIndex(string colorName)
        {
            int indexColor = -1;
            PropertyInfo[] colors = typeof(Brushes).GetProperties();

            if (string.IsNullOrEmpty(colorName) == true)
            {
                indexColor = Array.FindIndex(colors, x => x.Name.ToUpper() == "TRANSPARENT");
            }
            else
            {
                indexColor = Array.FindIndex(colors, x => x.Name.ToUpper() == colorName.ToUpper());
            }

            if (indexColor == -1)
            {
                indexColor = Array.FindIndex(colors, x => x.Name.ToUpper() == "TRANSPARENT");
            }

            return indexColor;
        }

        public static string ConvertIndexToColorName(int colorIndex)
        {
            PropertyInfo[] colors = typeof(Brushes).GetProperties();
            string colorName = colors[colorIndex].Name;
            return colorName;
        }

        public static string ConvertBrushToName(Brush colorName)
        {
            string result = string.Empty;
            string cname = new BrushConverter().ConvertToString(colorName);
            PropertyInfo[] brushes = typeof(Brushes).GetProperties();
            foreach (PropertyInfo item in brushes)
            {
                Brush brush = item.GetValue(brushes) as Brush;
                if (brush.ToString() == cname)
                {
                    result = item.Name;
                    break;
                }

            }
            return result;
        }

        public static Brush ConvertIndexToBrush(int colorIndex)
        {
            PropertyInfo[] colors = typeof(Brushes).GetProperties();
            string colorName = colors[colorIndex].Name;
            Color col = (Color)ColorConverter.ConvertFromString(colorName);
            Brush brushColor = new SolidColorBrush(col);
            return brushColor;
        }

        public static Brush ConvertNameToBrush(string colorName)
        {
            Color col = (Color)ColorConverter.ConvertFromString(colorName);
            Brush brushColor = new SolidColorBrush(col);
            return brushColor;
        }
    }
}
