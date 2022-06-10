using MyNCMusic.Models;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MyNCMusic.ViewModel
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    internal class PlayingViewModel
    {
        public BitmapImage AlbumImage { get; set; }
        public string MusicName { get; set; }
        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
        public List<Artist> Artists { get; set; }
        public List<LyricStr> LyricStrs { get; set; }
        public PlayingViewModel()
        {
            PlayingService.OnPlayingChanged += PlayingService_OnPlayChanged;
        }

        private void PlayingService_OnPlayChanged(long id, string url)
        {
            AlbumImage = PlayingService.PlayingAlbumBitmapImage;
            if (PlayingService.IsPlayingSong)
                UpdateSong();
            else
                UpdateRadio();
        }
        private async void UpdateSong()
        {
            MusicName = PlayingService.PlayingSong.Name;
            ArtistName = PlayingService.PlayingSong.Ar.First().Name;
            AlbumName = PlayingService.PlayingSong.Al.Name;
            LyricRoot lyricRoot = await LyricService.GetLyricAsync(PlayingService.PlayingSong.Id);
            if (lyricRoot == null)
            {
                LyricStrs = null;
            }
            else
            {
                LyricStrs = LyricService.GetLyricStrs(lyricRoot);
                if (LyricStrs != null)
                {
                    //歌词滚动到第一个？
                    // ListBox_lyric.ScrollIntoView(lyricStrs.First());
                }
            }
            
        }

        private void UpdateRadio()
        {

        }
    }
}
