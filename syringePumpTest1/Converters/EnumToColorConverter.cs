using SyringePumpTest1.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SyringePumpTest1.Converters
{
    public class EnumToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Status status)
            {
                switch (status)
                {
                    case Status.Input:
                        return Brushes.BlueViolet;
                    case Status.Output:
                        return Brushes.Orange;
                    case Status.Font:
                        return Brushes.Black;
                    case Status.Inactive:
                        return Brushes.Gray;
                    default:
                        return Brushes.Gray;
                }
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
