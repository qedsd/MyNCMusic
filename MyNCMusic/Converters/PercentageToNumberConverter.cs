using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MyNCMusic.Converters
{
    /// <summary>
    /// 百分比和100制数值转换
    /// </summary>
    public class PercentageToNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (double)value * 100;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (double)value / 100; ;
        }
    }
}
