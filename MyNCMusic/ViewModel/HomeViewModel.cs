using MyNCMusic.Helper;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MyNCMusic.ViewModel
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class HomeViewModel
    {
        public Brush MainBackgroundBrush { get; set; }
        public ImageBrush MainImageBrush;

        public HomeViewModel()
        {
            MainImageBrush = new ImageBrush();
            Services.PlayingService.OnPlayingChanged += PlayingService_OnPlayChanged;
            ConfigService.LoadConfig();
            ChangeMainBackgroundImage();
        }

        private void PlayingService_OnPlayChanged(long id,string url)
        {
            ChangeMainBackgroundImage();
        }

        /// <summary>
        /// 读取本地专辑图片修改背景图
        /// </summary>
        private async void ChangeMainBackgroundImage()
        {
            //判断Local是否有文件
            if (await ApplicationData.Current.LocalFolder.TryGetItemAsync(ConfigService.ImageFilename) is StorageFile localFile)//本地有专辑图片，读取
            {
                WriteableBitmap writeableBitmap = await FileHelper.OpenWriteableBitmapFile(localFile);
                MainImageBrush.ImageSource = writeableBitmap;
                MainImageBrush.Stretch = Stretch.UniformToFill;
                MainBackgroundBrush = MainImageBrush;
            }
            else//本地无专辑图片
            {
                SolidColorBrush solidColorBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
                MainBackgroundBrush = solidColorBrush;
            }
        }
    }
}
