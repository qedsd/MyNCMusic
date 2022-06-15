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
    public sealed partial class AlbumDetail : Page
    {
        private ViewModel.AlbumViewModel VM;
        public AlbumDetail()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);  //将传过来的数据 类型转换一下　　
            if (e.Parameter == null)
                return;
            var albumRoot = (AlbumRoot)e.Parameter;
            VM = new ViewModel.AlbumViewModel(albumRoot);
            DataContext = VM;
        }

        private void ListBox_Artists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Artist artist = ((ListBox)sender).SelectedItem as Artist;
            if (artist != null)
            {
                VM.CheckArtistCommand.Execute(artist);
            }
        }

        private void MusicList_OnChangedSong(MusicItem musicItem)
        {
            VM.PlayMusic(musicItem);
        }

        private void MusicList_OnChangedAlbum(AlbumRoot album)
        {
            return;
        }

        private void MusicList_OnChangedArtist(ArtistBaseDetailRoot artist)
        {
            VM.CheckArtistCommand.Execute(artist);
        }
    }
}
