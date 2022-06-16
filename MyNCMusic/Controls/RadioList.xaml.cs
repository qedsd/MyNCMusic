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
    public sealed partial class RadioList : UserControl
    {
        public RadioList()
        {
            this.InitializeComponent();
        }
        
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(List<Models.DjRadiosItem>), typeof(RadioList), new PropertyMetadata(null, new PropertyChangedCallback(ItemsSourcePropertyChanged)));
        private static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RadioList)d).ListBox.ItemsSource = (List<Models.DjRadiosItem>)e.NewValue;
        }
        public List<Models.DjRadiosItem> ItemsSource
        {
            get => (List<Models.DjRadiosItem>)GetValue(ItemsSourceProperty);
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }
        public delegate void ChangedRadioDelegate(Models.DjRadiosItem Radio);
        public event ChangedRadioDelegate OnChangedRadio;
        private void ListBox_Click(object sender, TappedRoutedEventArgs e)
        {
            Models.DjRadiosItem radio = ((ListBox)sender).SelectedItem as Models.DjRadiosItem;
            if (radio != null)
            {
                OnChangedRadio?.Invoke(radio);
            }
        }
    }
}
