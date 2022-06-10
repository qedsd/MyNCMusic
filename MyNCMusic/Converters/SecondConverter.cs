using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MyNCMusic.Converters
{
    /// <summary>
    /// 秒数转换成分秒显示的字符串
    /// </summary>
    class SecondConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int i = 0; double d = 0.0;
            if (value.GetType() == i.GetType())
            {
                int dt = (int)value;
                if (dt == 0)
                {
                    return "0:00";
                }
                TimeSpan ts = TimeSpan.FromSeconds(dt);
                return string.Format("{0}:{1:D2}", ts.Minutes, ts.Seconds);
            }
            else if (value.GetType() == d.GetType())
            {
                double dt = (double)value;
                if (dt == 0)
                {
                    return "0:00";
                }
                TimeSpan ts = TimeSpan.FromSeconds(dt);
                return string.Format("{0}:{1:D2}", ts.Minutes, ts.Seconds);
            }
            else
            {
                return "0:00";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
