using System;
using System.Globalization;
using WindowsInput.Native;
using MVVM_Tools.Code.Classes;

namespace App.Logic.Converters
{
    public class KeyToRepresentationConverter : ConverterBase<int, string>
    {
        public override string ConvertInternal(int value, object parameter, CultureInfo culture)
        {
            if (Enum.TryParse(value.ToString(), out VirtualKeyCode keyCode))
                return $"{value} ({keyCode})";

            return $"{value} (?)";
        }

        public override int ConvertBackInternal(string value, object parameter, CultureInfo culture)
        {
            try
            {
                int realNumber = 0;
                foreach (char ch in value)
                {
                    if (ch < '0' || ch > '9')
                        return realNumber;

                    realNumber = checked(realNumber * 10 + (ch - '0'));
                }

                return realNumber;
            }
            catch (OverflowException)
            {
                return 0;
            }
        }
    }
}
