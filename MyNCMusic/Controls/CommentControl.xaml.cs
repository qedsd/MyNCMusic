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
    public sealed partial class CommentControl : UserControl
    {
        public CommentControl()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(List<Models.CommentItem>), typeof(CommentControl), new PropertyMetadata(null, new PropertyChangedCallback(ItemsSourcePropertyChanged)));
        private static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ((CommentControl)d).CommentListBox.ItemsSource = (List<Models.CommentItem>)e.NewValue;
            }
        }
        public List<Models.CommentItem> ItemsSource
        {
            get => (List<Models.CommentItem>)GetValue(ItemsSourceProperty);
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }
    }
}
