using MyNCMusic.Models;
using MyNCMusic.MyUserControl;
using MyNCMusic.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MyNCMusic.ViewModel
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    internal class RadioDetailViewModel
    {
        public BitmapImage RadioImage { get; set; }
        public List<RadioSongItem> RadioSongItems { get; set; }
        public string RadioName { get; set; }
        public string DjName { get; set; }
        public string Des { get; set; }
        public RadioDetailViewModel(List<RadioSongItem> radioSongItems)
        {
            RadioImage = new BitmapImage(new Uri(radioSongItems.First().Radio.PicUrl));
            RadioSongItems = radioSongItems;
            RadioName = radioSongItems.First().Radio.Name;
            DjName = radioSongItems.First().Dj.Nickname;
            Des = radioSongItems.First().Radio.Desc;
        }

        public ICommand PlayCommand => new DelegateCommand<RadioSongItem>(async(r) =>
        {
            if (r == null)
            {
                r = RadioSongItems.FirstOrDefault();
            }
            await PlayingService.ChangePlayingRadio(r.MainSong.Id, RadioSongItems);
        });
    }
}
