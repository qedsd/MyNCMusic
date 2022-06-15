using MyNCMusic.Models;
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
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MyNCMusic.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CommentPage : Page
    {
        private ViewModel.CommentViewModel VM;
        public CommentPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);  //将传过来的数据 类型转换一下　　
            var commentRoot = (CommentRoot)e.Parameter;
            if (commentRoot != null)
            {
                VM = new ViewModel.CommentViewModel(commentRoot);
                DataContext = VM;
            }
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        public static readonly DependencyProperty CanGoBackProperty =
            DependencyProperty.Register("ItemsSource", typeof(List<Models.CommentItem>), typeof(CommentPage), new PropertyMetadata(null, new PropertyChangedCallback(CanGoBackPropertyChanged)));
        private static void CanGoBackPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ((CommentPage)d).Button_Back.Visibility = (bool)e.NewValue?Visibility.Visible:Visibility.Collapsed;
            }
        }
        public bool CanGoBack
        {
            get => (bool)GetValue(CanGoBackProperty);
            set
            {
                SetValue(CanGoBackProperty, value);
            }
        }

        public static readonly DependencyProperty HotCommentsProperty =
            DependencyProperty.Register("HotComments", typeof(List<Models.CommentItem>), typeof(CommentPage), new PropertyMetadata(null, new PropertyChangedCallback(HotCommentsPropertyChanged)));
        private static void HotCommentsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ((CommentPage)d).VM.HotComments = (List<Models.CommentItem>)e.NewValue;
            }
        }
        public List<Models.CommentItem> HotComments
        {
            get => (List<Models.CommentItem>)GetValue(HotCommentsProperty);
            set
            {
                SetValue(HotCommentsProperty, value);
            }
        }

        public static readonly DependencyProperty AllCommentsProperty =
           DependencyProperty.Register("HotComments", typeof(List<Models.CommentItem>), typeof(CommentPage), new PropertyMetadata(null, new PropertyChangedCallback(AllCommentsPropertyChanged)));
        private static void AllCommentsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ((CommentPage)d).VM.AllComments = (List<Models.CommentItem>)e.NewValue;
            }
        }
        public List<Models.CommentItem> AllComments
        {
            get => (List<Models.CommentItem>)GetValue(AllCommentsProperty);
            set
            {
                SetValue(AllCommentsProperty, value);
            }
        }
    }
}
