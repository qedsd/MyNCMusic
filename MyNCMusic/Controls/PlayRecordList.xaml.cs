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
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace MyNCMusic.Controls
{
    public sealed partial class PlayRecordList : UserControl
    {
        public PlayRecordList()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(List<Models.RecordDataItem>), typeof(PlayRecordList), new PropertyMetadata(null, new PropertyChangedCallback(ItemsSourcePropertyChanged)));
        private static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlayRecordList)d).ListBox.ItemsSource = (List<Models.RecordDataItem>)e.NewValue;
        }
        public List<Models.RecordDataItem> ItemsSource
        {
            get => (List<Models.RecordDataItem>)GetValue(ItemsSourceProperty);
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }
        public delegate void ChangedRecordDelegate(Models.RecordDataItem record);
        public event ChangedRecordDelegate OnChangedRecord;
        private void ListBox_Click(object sender, DoubleTappedRoutedEventArgs e)
        {
            Models.RecordDataItem record = ((ListBox)sender).SelectedItem as Models.RecordDataItem;
            if(record != null)
            {
                OnChangedRecord?.Invoke(record);
            }
        }
        public delegate void ChangedAlbumDelegate(long id);
        public event ChangedAlbumDelegate OnChangedAlbum;
        private void Button_Album_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var record = button.DataContext as Models.RecordDataItem;
            if( record != null )
            {
                OnChangedAlbum?.Invoke(record.Song.Al.Id);
            }
        }
    }
}
