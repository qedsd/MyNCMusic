using MyNCMusic.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
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
    public sealed partial class CompactOverlayPage : Page
    {
        
        public CompactOverlayPage()
        {
            (Application.Current as App).compactOverlayPage = this;
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Window.Current.SetTitleBar(MyTitleBar);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);  //将传过来的数据 类型转换一下　　
            string name= e.Parameter as string;
            //List<string> list = e.Parameter as List<string>;
            //if (list == null || list.Count != 4)
            //    return;
            //UpdateLayout(list);
            UpdateLayout_name(name);
            Window.Current.SetTitleBar(MyTitleBar);
        }
        public void UpdateLayout_lyric(List<LyricStr> list)
        {
            TextBlock_1_o.Text = list[0].Original==null?"": list[0].Original;
            TextBlock_1_t.Text = list[0].Tran == null ? "" : list[0].Tran;
            TextBlock_2_o.Text = list[1].Original == null ? "" : list[1].Original;
            TextBlock_2_t.Text = list[1].Tran == null ? "" : list[1].Tran;
            TextBlock_3_o.Text = list[2].Original == null ? "" : list[2].Original;
            TextBlock_3_t.Text = list[2].Tran == null ? "" : list[2].Tran;
        }
        public void UpdateLayout_name(string name)
        {
            TextBlock_name.Text = name;
            TextBlock_1_o.Text = "";
            TextBlock_1_t.Text = "";
            TextBlock_2_o.Text = "";
            TextBlock_2_t.Text = "";
            TextBlock_3_o.Text = "";
            TextBlock_3_t.Text = "";
            Grid_background.Background = (Application.Current as App).myMainPage.mainImageBrush;
        }

        private async void Button_standardMode_Click(object sender, RoutedEventArgs e)
        {
            if(await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default))
                Frame.GoBack();
        }
    }
}
