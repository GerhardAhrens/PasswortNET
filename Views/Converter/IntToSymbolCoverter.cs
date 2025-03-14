namespace PasswortNET.Views.Converter
{

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Data;

    using PasswortNET.Core;

    public class IntToSymbolCoverter : IValueConverter
    {
        private Dictionary<int, string> symbols;
        public IntToSymbolCoverter()
        {
            this.symbols = this.FillSymbol();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = string.Empty;

            if (value != null)
            {
                if (value is int)
                {

                    return this.symbols.ContainsKey((int)value +1) == true ? this.symbols[(int)value +1] : string.Empty;
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        private Dictionary<int, string> FillSymbol()
        {
            using (SymbolsList sl = new SymbolsList())
            {
                return sl.GetSymbols();
            }
        }
    }
}