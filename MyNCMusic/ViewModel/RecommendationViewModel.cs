using MyNCMusic.Helper;
using MyNCMusic.Models;
using MyNCMusic.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyNCMusic.ViewModel
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class RecommendationViewModel
    {
        public List<MusicItem> RecommendMusics { get; set; }
        public List<MusicItem> RandomFavoriteMusics { get; set; }
        public List<MusicItem> SearchMusics { get; set; }
        public List<Album> SearchAlbums { get; set; }
        public List<Artist> SearchArtists { get; set; }
        public List<PlaylistItem> SearchPlaylists { get; set; }
        public List<RecommendList> RecommendPlaylists { get; set; }
        public RecommendList SelectedPlaylist { get; set; }
        private int selectedIndex;
        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                selectedIndex = value;
                switch (selectedIndex)
                {
                    case 0: LoadRecommendPlaylists();break;//推荐歌单
                    case 1: LoadRecommendMusics(); break;//日推歌曲
                    case 2: LoadRandomFavorite(); break;//随机喜欢
                    case 3: break;//搜索
                }
            }
        }
        private int searchSelectedIndex;
        public int SearchSelectedIndex
        {
            get => searchSelectedIndex;
            set
            {
                searchSelectedIndex = value;
                Search();
            }
        }
        private string searchText;
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;

            }
        }
        public PlaylistItem SelectedSearchPlaylist { get; set; }
        public RecommendationViewModel()
        {
            LoadRecommendPlaylists();
        }

        /// <summary>
        /// 点击推荐歌单
        /// </summary>
        public ICommand ClickRecommendPlaylistCommand => new DelegateCommand(() =>
        {
            if (SelectedPlaylist != null)
            {
                NavigateService.NavigateToPlaylistAsync(SelectedPlaylist.Id);
            }
        });
        /// <summary>
        /// 推荐歌单
        /// </summary>
        private async void LoadRecommendPlaylists()
        {
            if(RecommendPlaylists == null)
            {
                Controls.WaitingPopup.Show();
                var recommendListRoot = await PlaylistService.GetCommendatoryListAsync();
                RecommendPlaylists = recommendListRoot.Recommend;
                Controls.WaitingPopup.Hide();
            }
        }
        /// <summary>
        /// 推荐单曲
        /// </summary>
        private async void LoadRecommendMusics()
        {
            if (RecommendMusics == null)
            {
                Controls.WaitingPopup.Show();
                RecommendMusics = await SongService.GetRecommandSongAsync();
                Controls.WaitingPopup.Hide();
            }
        }
        /// <summary>
        /// 随机喜欢的
        /// </summary>
        /// <param name="totalCount"></param>
        private async void LoadRandomFavorite(int totalCount = 50)
        {
            if(PlayingService.FavoriteMusics.Count == 0|| RandomFavoriteMusics!=null)
            {
                return;
            }
            HashSet<long> ids = new HashSet<long>();
            Random random = new Random();
            while (ids.Count < totalCount)
            {
                while(true)
                {
                    int k = random.Next(0, PlayingService.FavoriteMusics.Count - 1);
                    if(ids.Add(PlayingService.FavoriteMusics.ElementAt(k)))
                    {
                        break;
                    }
                }
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach(long id in ids)
            {
                stringBuilder.Append(id.ToString());
                stringBuilder.Append(",");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            Controls.WaitingPopup.Show();
            MusicDetailRoot musicDetailRoot = await SongService.GetMusicDetail_PostAsync(stringBuilder.ToString());
            Controls.WaitingPopup.Hide();
            RandomFavoriteMusics = musicDetailRoot?.Songs;
        }
        
        /// <summary>
        /// 播放推荐单曲
        /// </summary>
        /// <param name="music"></param>
        public async void PlayRecommendMusic(MusicItem music = null)
        {
            if(music==null)
            {
                music = RecommendMusics?.FirstOrDefault();
            }
            if(music!=null)
            {
                PlayingService.PlayingListId = music.Al.Id;
                await PlayingService.ChangePlayingSong(music.Id, RecommendMusics, music);
            }
        }

        /// <summary>
        /// 播放随机喜欢单曲
        /// </summary>
        /// <param name="music"></param>
        public async void PlayRandomFavoriteMusic(MusicItem music = null)
        {
            if (music == null)
            {
                music = RandomFavoriteMusics?.FirstOrDefault();
            }
            if (music != null)
            {
                PlayingService.PlayingListId = music.Al.Id;
                await PlayingService.ChangePlayingSong(music.Id, RandomFavoriteMusics, music);
            }
        }

        /// <summary>
        /// 播放搜索单曲
        /// </summary>
        /// <param name="music"></param>
        public async void PlaySearchMusic(MusicItem music = null)
        {
            if (music == null)
            {
                music = SearchMusics?.FirstOrDefault();
            }
            if (music != null)
            {
                PlayingService.PlayingListId = music.Al.Id;
                await PlayingService.ChangePlayingSong(music.Id, SearchMusics, music);
            }
        }

        /// <summary>
        /// 点击搜索歌单
        /// </summary>
        public ICommand ClickSearchPlaylistCommand => new DelegateCommand(() =>
        {
            if (SelectedSearchPlaylist != null)
            {
                NavigateService.NavigateToPlaylistAsync(SelectedSearchPlaylist.Id);
            }
        });
        public async void Search()
        {
            if(SearchText==null||SearchText == string.Empty)
            {
                SearchMusics = null;
                SearchAlbums = null;
                SearchArtists = null;
                SearchPlaylists = null;
                return;
            }
            int searchType = 0;
            switch(SearchSelectedIndex)
            {
                case 0:searchType = 1;break;
                case 1:searchType = 10; break;
                case 2:searchType = 100; break;
                case 3:searchType = 1000; break;
            }
            Controls.WaitingPopup.Show();
            SearchRoot searchRoot = await SearchService.SearchCloundAsync(SearchText, searchType);
            Controls.WaitingPopup.Hide();
            if (searchRoot != null && searchRoot.Result != null)
            {
                switch (searchType)
                {
                    case 1:
                        {
                            SearchMusics = searchRoot.Result.Songs;
                        }
                        break;
                    case 10:
                        {
                            SearchAlbums = searchRoot.Result.Albums;
                        }
                        break;
                    case 100:
                        {
                            SearchArtists = searchRoot.Result.Artists;
                        }
                        break;
                    case 1000:
                        {
                            SearchPlaylists = searchRoot.Result.Playlists;
                        }
                        break;
                }
            }
        }
    }
}
