using MyNCMusic.Controls;
using MyNCMusic.Models;
using MyNCMusic.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace MyNCMusic.ViewModel
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    internal class AlbumViewModel
    {
        public BitmapImage AlbumImage { get; set; }
        public string AlbumName { get; set; }
        public List<Artist> Artists { get; set; }
        public List<MusicItem> Musics { get; set; }
        public string Des { get; set; }
        public int CommentCount { get; set; }

        private AlbumRoot AlbumRoot;
        public AlbumViewModel(AlbumRoot albumRoot)
        {
            AlbumRoot = albumRoot;
            AlbumImage = new BitmapImage(new Uri(albumRoot.Album.PicUrl));
            AlbumName = albumRoot.Album.Name;
            CommentCount = albumRoot.Album.Info.CommentCount;
            Des = albumRoot.Album.Description;
            Musics = albumRoot.Songs;
            Artists = albumRoot.Album.Artists;
        }

        public ICommand CheckCommentCommand => new DelegateCommand(async() =>
        {
            CommentRoot commentRoot = await CommentService.GetAlbumCommentAsync(AlbumRoot.Album.Id);
            NavigateService.NavigateToComment(commentRoot);
        });
        public ICommand PlayAllCommand => new DelegateCommand(() =>
        {
            PlayMusic();
        });
        public ICommand CheckArtistCommand => new DelegateCommand<Artist>(async (artist) =>
        {
            WaitingPopup.Show();
            if (artist == null)
            {
                artist = Artists.First();
            }
            ArtistBaseDetailRoot artistBaseDetailRoot = await ArtistService.GetArtistBaseDetailAsync(artist.Id);
            NavigateService.NavigateToArtistAsync(artistBaseDetailRoot);
            WaitingPopup.Hide();
        });
        public async void PlayMusic(MusicItem music = null)
        {
            if (music == null)
            {
                music = Musics?.FirstOrDefault();
            }
            if (music != null)
            {
                await PlayingService.ChangePlayingSongAsync(music.Id, AlbumRoot.Album.Id, Musics, music);
            }
        }

    }
}
