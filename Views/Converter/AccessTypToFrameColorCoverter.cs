namespace PasswortNET.Views.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    using PasswortNET.Core;

    public class AccessTypToFrameColorCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush result = Brushes.Transparent;

            if (value != null)
            {
                if (Enum.IsDefined(typeof(AccessTyp), value))
                {
                    if (this.CastToEnum<AccessTyp>(value) == AccessTyp.Website)
                    {
                        result = Brushes.Green;
                    }
                    else if (this.CastToEnum<AccessTyp>(value) == AccessTyp.Pin)
                    {
                        result = Brushes.Blue;
                    }
                    else if (this.CastToEnum<AccessTyp>(value) == AccessTyp.Passwort)
                    {
                        result = Brushes.Violet;
                    }
                    else if (this.CastToEnum<AccessTyp>(value) == AccessTyp.License)
                    {
                        result = Brushes.Yellow;
                    }
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        private T CastToEnum<T>(object o)
        {
            T enumVal = (T)Enum.Parse(typeof(T), o.ToString());
            return enumVal;
        }
    }
}