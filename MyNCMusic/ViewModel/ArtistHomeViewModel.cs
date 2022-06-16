using MyNCMusic.Controls;
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
    internal class ArtistHomeViewModel
    {
        public BitmapImage ArtistImage { get; set; }
        public string ArtistName { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }
        public List<MusicItem> HotMusics { get; set; }
        public List<Album> Albums { get; set; }
        private int pivotIndex = 0;
        public int PivotIndex
        {
            get => pivotIndex;
            set
            {
                pivotIndex = value;
                if(value == 1)//专辑
                {
                    LoadAlbums();
                }
            }
        }
        private ArtistBaseDetailRoot ArtistBaseDetailRoot { get; set; }
        public ArtistHomeViewModel(ArtistBaseDetailRoot artistBaseDetailRoot)
        {
            if (artistBaseDetailRoot != null)
            {
                ArtistBaseDetailRoot = artistBaseDetailRoot;
                ArtistImage = new BitmapImage(new Uri(artistBaseDetailRoot.Artist.Img1v1Url));
                ArtistName = artistBaseDetailRoot.Artist.Name;
                HotMusics = artistBaseDetailRoot.HotSongs;
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var temp in artistBaseDetailRoot.Artist.Alias)
                {
                    stringBuilder.Append(temp);
                    stringBuilder.Append(";");
                }
                Alias = stringBuilder.ToString();
            }
        }
        private async void LoadAlbums()
        {
            if (Albums == null)
            {
                WaitingPopup.Show();
                ArtistAllAlbumRoot artistAllAlbumRoot = await AlbumService.GetArtistAllAlbumsAsync(ArtistBaseDetailRoot.Artist.Id);
                WaitingPopup.Hide();
                Albums = artistAllAlbumRoot.HotAlbums;
            }
        }
        /// <summary>
        /// 播放单曲
        /// </summary>
        /// <param name="music"></param>
        public async void PlayHotMusic(MusicItem music = null)
        {
            if (music == null)
            {
                music = HotMusics?.FirstOrDefault();
            }
            if (music != null)
            {
                await PlayingService.ChangePlayingSongAsync(music.Id, music.Al.Id, HotMusics, music);
            }
        }
    }
}
