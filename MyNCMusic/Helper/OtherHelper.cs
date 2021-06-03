using MyNCMusic.Model;
using MyNCMusic.MyUserControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MyNCMusic.Helper
{
    public static class OtherHelper
    {
        /// <summary>
        /// string转md5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Md5(string str)
        {
            HashAlgorithmProvider hashAlgorithm =
                 HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            CryptographicHash cryptographic = hashAlgorithm.CreateHash();
            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            cryptographic.Append(buffer);
            return CryptographicBuffer.EncodeToHexString(cryptographic.GetValueAndReset());
        }

        /// <summary>
        /// 转md5
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>md5加密后的字符串</returns>
        public static string Encrypt(string str)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));//转化为小写的16进制
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 颜色加深、减淡
        /// </summary>
        /// <param name="color"></param>
        /// <param name="correctionFactor"></param>
        /// <returns></returns>
        public static Color ChangeColor(Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }
            if (red < 0) red = 0;
            if (red > 255) red = 255;
            if (green < 0) green = 0;
            if (green > 255) green = 255;
            if (blue < 0) blue = 0;
            if (blue > 255) blue = 255;
            return Color.FromArgb(color.A, Byte.Parse(((int)red).ToString()), Byte.Parse(((int)green).ToString()), Byte.Parse(((int)blue).ToString()));
        }

        /// <summary>
        /// 错误弹窗
        /// </summary>
        /// <param name="content">错误显示string</param>
        public static async void ShowContentDialog(string content = "ERROR")
        {
            try
            {
                TextBlock textBlock = new TextBlock()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextWrapping=TextWrapping.WrapWholeWords
                };
                textBlock.Text = content;
                var dialog = new ContentDialog()
                {
                    Title = "Error!",
                    Content = textBlock,
                    PrimaryButtonText = "关闭",
                    FullSizeDesired = false,
                };
                dialog.PrimaryButtonClick += (_s, _e) => { };
                await dialog.ShowAsync();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 带标题的弹窗
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        public static async void ShowContentDialog(string title = "ERRO", string content = "ERRO")
        {
            TextBlock textBlock = new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping = TextWrapping.WrapWholeWords
            };
            textBlock.Text = content;
            var dialog = new ContentDialog()
            {
                Title = title,
                Content = textBlock,
                PrimaryButtonText = "关闭",
                FullSizeDesired = false,

            };
            dialog.PrimaryButtonClick += (_s, _e) => { };
            try
            {
                await dialog.ShowAsync();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 获取分钟形式的时长
        /// </summary>
        /// <param name="dt">秒</param>
        /// <returns></returns>
        public static string GetDt(int dt)
        {
            TimeSpan ts = TimeSpan.FromSeconds(dt);
            //return String.Format("{0}:{1:D2}", ts.Minutes, ts.Seconds);

            //TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(dt));
            string str = "";
            if (ts.Hours > 0)
            {
                str = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds;
            }
            else
                return String.Format("{0}:{1:D2}", ts.Minutes, ts.Seconds);
            return str;
        }

        /// <summary>
        /// Unix时间戳转DateTime
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(string timestamp)
        {
            DateTime time = DateTime.MinValue;
            DateTime startTime = new DateTime(1970, 1, 1);
            if (timestamp.Length == 10)        //精确到秒
            {
                time = startTime.AddSeconds(double.Parse(timestamp));
            }
            else if (timestamp.Length == 13)   //精确到毫秒
            {
                time = startTime.AddMilliseconds(double.Parse(timestamp));
            }
            return time;
        }

        /// <summary>
        /// Unix时间戳转DateTime
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(long timestamp)
        {
            DateTime time = DateTime.MinValue;
            DateTime startTime = new DateTime(1970, 1, 1);
            if (timestamp >= 1000000000000)   //精确到毫秒
            {
                time = startTime.AddMilliseconds(timestamp);
            }
            else if (timestamp >= 1000000000)        //精确到秒
            {
                time = startTime.AddSeconds(timestamp);
            }
            return time;
        }

        /// <summary>
        /// 获取图片主颜色
        /// </summary>
        /// <param name="b"></param>
        /// <param name="backgroundBrush"></param>
        /// <returns></returns>
        public static async Task<SolidColorBrush> GetMajorColorAndBlur(byte[] b, ImageBrush backgroundBrush)
        {
            WriteableBitmap wb = new WriteableBitmap(1000, 1500);
            using (IRandomAccessStream iras = b.AsBuffer().AsStream().AsRandomAccessStream())
            {
                await wb.SetSourceAsync(iras);
            }
            //高斯模糊
            BlurEffect be = new BlurEffect(wb);
            backgroundBrush.ImageSource = await be.ApplyFilter(10);//高斯模糊等级可以自己定义
            //取主色调并应用到TagsTextBlock
            return new SolidColorBrush(GetColor.GetMajorColor(wb));
        }
        public static async Task<SolidColorBrush> GetMajorColorAndBlur(string url, ImageBrush backgroundBrush)
        {
            WriteableBitmap wb = new WriteableBitmap(1000, 1500);
            HttpClient hc = new HttpClient();
            byte[] b = await hc.GetByteArrayAsync(url);
            hc.Dispose();
            using (IRandomAccessStream iras = b.AsBuffer().AsStream().AsRandomAccessStream())
            {
                await wb.SetSourceAsync(iras);
            }
            //高斯模糊
            BlurEffect be = new BlurEffect(wb);
            backgroundBrush.ImageSource = await be.ApplyFilter(10);//高斯模糊等级可以自己定义
            //取主色调并应用到TagsTextBlock
            return new SolidColorBrush(GetColor.GetMajorColor(wb));
        }

        /// <summary>
        /// 复制txt到粘贴板
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CopyTextToClipboard(string str)
        {
            try
            {
                //创建一个数据包
                DataPackage dataPackage = new DataPackage();//设置创建包里的文本内容
                dataPackage.SetText(str);//把数据包放到剪贴板里
                Clipboard.SetContent(dataPackage);
                NotifyPopup notifyPopup = new NotifyPopup("已复制", "\xF78C", Colors.MediumSeaGreen);
                notifyPopup.Show();
                return true;
            }
            catch (Exception er) 
            {
                NotifyPopup notify = new NotifyPopup(er.Message, "\xE10A");
                notify.Show();
                return false; 
            }
        }
    }
}
