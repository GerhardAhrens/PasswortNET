namespace PasswortNET.Views.Converter
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using PasswortNET.Core;
    using PasswortNET.Core.Enums;

    public class AccessTypToVisibilityCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility result = Visibility.Visible;

            if (value != null)
            {
                ControlTyp parameterEnum = (ControlTyp)parameter;

                if (Enum.IsDefined(typeof(AccessTyp), value))
                {
                    if (this.CastToEnum<AccessTyp>(value) == AccessTyp.Pin && parameterEnum == ControlTyp.WebsiteTxt)
                    {
                        result = Visibility.Collapsed;
                    }
                    else if (this.CastToEnum<AccessTyp>(value) == AccessTyp.Pin && parameterEnum == ControlTyp.UsernameTxt)
                    {
                        result = Visibility.Collapsed;
                    }
                    else if (this.CastToEnum<AccessTyp>(value) == AccessTyp.Pin && parameterEnum == ControlTyp.PasswortTxt)
                    {
                        result = Visibility.Collapsed;
                    }
                    else if (this.CastToEnum<AccessTyp>(value) == AccessTyp.Passwort && parameterEnum == ControlTyp.WebsiteTxt)
                    {
                        result = Visibility.Collapsed;
                    }
                    else if (this.CastToEnum<AccessTyp>(value) == AccessTyp.Passwort && parameterEnum == ControlTyp.UsernameTxt)
                    {
                        result = Visibility.Collapsed;
                    }
                    else if (this.CastToEnum<AccessTyp>(value) == AccessTyp.Passwort && parameterEnum == ControlTyp.PinTxt)
                    {
                        result = Visibility.Collapsed;
                    }
                    else if (this.CastToEnum<AccessTyp>(value) == AccessTyp.Website && parameterEnum == ControlTyp.PinTxt)
                    {
                        result = Visibility.Collapsed;
                    }
                    else if (this.CastToEnum<AccessTyp>(value) == AccessTyp.Website && parameterEnum == ControlTyp.WebsiteTxt)
                    {
                        result = Visibility.Collapsed;
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