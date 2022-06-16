using MyNCMusic.Models;
using MyNCMusic.MyUserControl;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MyNCMusic.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PlayedRecord : Page
    {
        public PlayedRecord()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Loaded += PlayedRecord_Loaded;
        }

        private async void PlayedRecord_Loaded(object sender, RoutedEventArgs e)
        {
            Controls.WaitingPopup.Show();
            RecordData recordData = await RecordDataService.GetRecordDataAsync(ConfigService.Uid, 1);
            Controls.WaitingPopup.Hide();
            if (recordData.WeekData != null)
            {
                PlayRecordList_Week.ItemsSource = recordData.WeekData;
            }
            else
            {
                NotifyPopup.ShowError("获取最近一周记录失败");
            }
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(((Pivot)sender).SelectedIndex == 1 && PlayRecordList_All.ItemsSource == null)
            {
                Controls.WaitingPopup.Show();
                RecordData recordData = await RecordDataService.GetRecordDataAsync(ConfigService.Uid, 0);
                Controls.WaitingPopup.Hide();
                if (recordData.AllData != null)
                {
                    PlayRecordList_All.ItemsSource = recordData.AllData;
                }
                else
                {
                    NotifyPopup.ShowError("获取全部记录失败");
                }
            }
        }

        private async void PlayRecordList_OnChangedRecord(RecordDataItem record)
        {
            await PlayingService.ChangePlayingSongAsync(record.Song.Id, record.Song.Al.Id,null,null);
        }

        private async void PlayRecordList_OnChangedAlbum(long id)
        {
            Controls.WaitingPopup.Show();
            AlbumRoot albumRoot = await AlbumService.GetAlbumAsync(id);
            Controls.WaitingPopup.Hide();
            NavigateService.NavigateToAlbumAsync(albumRoot);
        }
    }
}
