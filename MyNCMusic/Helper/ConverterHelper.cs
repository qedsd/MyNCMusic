using MyNCMusic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MyNCMusic.Helper
{
    /// <summary>
    /// 返回歌名
    /// </summary>
    public class GetAlia : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                List<string> alia = (List<string>)value;
                if (alia.Count == 0)
                    return null;
                string str = " (";
                for (int i = 0; i < alia.Count; i++)
                {
                    if (i != 0)
                        str += "/";
                    str += alia[i];
                }
                return str + ")";
            }
            else
                return null;
            //List<ArItem> arItems = value as List<ArItem>;
            //string name = "";
            //for (int i = 0; i < arItems.Count; i++)
            //{
            //    if (i != 0)
            //        name += "/";
            //    name += arItems[i].name;
            //}
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 返回艺术家名字
    /// </summary>
    public class GetArNames : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            List<Artist> arItems = value as List<Artist>;
            if(arItems != null)
            {
                string name = "";
                for (int i = 0; i < arItems.Count; i++)
                {
                    if (i != 0)
                        name += "/";
                    name += arItems[i].Name;
                }
                return name;
            }
            else
            {
                return null;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 返回艺术家名字
    /// </summary>
    public class GetArNames_ArtistsItem : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "";
            List<Artist> arItems = value as List<Artist>;
            string name = "";
            for (int i = 0; i < arItems.Count; i++)
            {
                if (i != 0)
                    name += "/";
                name += arItems[i].Name;
            }
            return name;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 返回时长
    /// </summary>
    public class GetDt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int dt = (int)value / 1000;
            TimeSpan ts = TimeSpan.FromSeconds(dt);
            return String.Format("{0}:{1:D2}", ts.Minutes, ts.Seconds);
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 返回时长,源数据为秒
    /// </summary>
    public class GetDt_S : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int i = 0; double d = 0.0;
            if (value.GetType() == i.GetType())
            {
                int dt = (int)value;
                if (dt == 0)
                    return "0:00";
                TimeSpan ts = TimeSpan.FromSeconds(dt);
                return String.Format("{0}:{1:D2}", ts.Minutes, ts.Seconds);
            }
            else if (value.GetType() == d.GetType())
            {
                double dt = (double)value;
                if (dt == 0)
                    return "0:00";
                TimeSpan ts = TimeSpan.FromSeconds(dt);
                return String.Format("{0}:{1:D2}", ts.Minutes, ts.Seconds);
            }
            else
                return "0:00";

        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// double返回int
    /// </summary>
    public class ReturnInByDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double t = (double)value;
            return (int)t;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 是否在播放，返回颜色
    /// </summary>
    public class ReturnForegroundIsPlaying : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool t = (bool)value;
            if (t)
            //return new SolidColorBrush(Windows.UI.ViewManagement.UIColorType.Accent);
            {
                Windows.UI.ViewManagement.UISettings uISettings = new Windows.UI.ViewManagement.UISettings();
                return new SolidColorBrush(uISettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent));
            }
            else
                return new SolidColorBrush(Colors.White);
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 修改图片大小-48
    /// </summary>
    public class ReturnImageUriWithParam_48 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;
            string str = (string)value;
            return new BitmapImage(new Uri(str += "?param=48y48"));
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 修改图片大小-160
    /// </summary>
    public class ReturnImageUriWithParam_160 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;
            string str = (string)value;
            return new BitmapImage(new Uri(str += "?param=160y160"));
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 是否在播放，返回visibility
    /// </summary>
    public class GetPlayingIconVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility == false)
                return DependencyProperty.UnsetValue;
            if ((Visibility)value == Visibility.Visible)
                return true;
            else
                return false;
        }
    }


    public class GetFavoriteIconVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isF = (bool)value;
            if (!isF)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility == false)
                return DependencyProperty.UnsetValue;
            if ((Visibility)value == Visibility.Visible)
                return true;
            else
                return false;
        }
    }

    /// <summary>
    /// 时间戳转换日期
    /// </summary>
    public class TimespanToDateTime : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime dateTime= OtherHelper.ConvertToDateTime((long)value);
            return dateTime.ToShortDateString();
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }

    /// <summary>
    /// 音量值转换
    /// </summary>
    public class ChangeVolumeType : IValueConverter
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
