using MyNCMusic.Models;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace MyNCMusic.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class RadioDetailPage : Page
    {
        private ViewModel.RadioDetailViewModel VM;
        public RadioDetailPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter == null)
                return;
            VM = new ViewModel.RadioDetailViewModel(e.Parameter as List<RadioSongItem>);
            DataContext = VM;
        }
        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void ListBox_RadioDetail_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var r = ((ListBox)sender).SelectedItem as RadioSongItem;
            if(r != null)
            {
                VM.PlayCommand.Execute(r);
            }
        }
    }
}
