using Microsoft.Toolkit.Uwp.UI.Controls;
using MyNCMusic.Models;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MyNCMusic.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ArtistHome : Page
    {
        private ViewModel.ArtistHomeViewModel VM;
        public ArtistHome()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);  //将传过来的数据 类型转换一下　　
            ArtistBaseDetailRoot artistBaseDetailRoot = e.Parameter as ArtistBaseDetailRoot;
            if (artistBaseDetailRoot == null)
            {
                return;
            }
            else
            {
                VM = new ViewModel.ArtistHomeViewModel(artistBaseDetailRoot);
                DataContext = VM;
            }
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            VM.PivotIndex = 0;//避免页面缓存导致的二次打卡page停留在上一次的index
            Frame.GoBack();
        }

        private void MusicList_OnChangedSong(MusicItem musicItem)
        {
            VM.PlayHotMusic(musicItem);
        }

        private void MusicList_OnChangedArtist(ArtistBaseDetailRoot artist)
        {
            NavigateService.NavigateToArtistAsync(artist);
        }

        private void MusicList_OnChangedAlbum(AlbumRoot album)
        {
            NavigateService.NavigateToAlbumAsync(album);
        }
    }
}
