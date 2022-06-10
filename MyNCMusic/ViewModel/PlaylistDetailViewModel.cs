using MyNCMusic.Models;
using MyNCMusic.MyUserControl;
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
    public class PlaylistDetailViewModel
    {
        /// <summary>
        /// 歌单信息
        /// </summary>
        public PlaylistItem PlaylistInfo { get; set; }
        /// <summary>
        /// 具体每一首歌
        /// </summary>
        public List<MusicItem> Songs { get; set; } = new List<MusicItem>();

        public PlaylistDetailViewModel(PlaylistItem playlistInfo, List<MusicItem> musicItems)
        {
            PlaylistInfo = playlistInfo;
            Songs = musicItems;
        }

        public ICommand SubscribeCommand => new DelegateCommand(async() =>
        {
            Controls.WaitingPopup.Show();
            if (PlaylistInfo.Subscribed)
            {
                if (await PlaylistService.SubOrCancelPlayListAsync(PlaylistInfo.Id, 2))
                {
                    NotifyPopup.ShowSuccess("已取消收藏");
                }
                else
                {
                    NotifyPopup.ShowError("操作失败");
                }
            }
            else
            {
                if (await PlaylistService.SubOrCancelPlayListAsync(PlaylistInfo.Id, 1))
                {
                    NotifyPopup.ShowSuccess("已添加收藏");
                }
                else
                {
                    NotifyPopup.ShowError("操作失败");
                }
            }
            Controls.WaitingPopup.Hide();
        });
        public ICommand PlayCommand => new DelegateCommand<MusicItem>(async(item) =>
        {
            if(PlaylistInfo==null||Songs == null|| Songs.Count==0)
            {
                return;
            }
            if(item == null)
            {
                PlayingService.PlayingListId = PlaylistInfo.Id;
                await PlayingService.ChangePlayingSong(Songs.FirstOrDefault().Id, Songs, Songs.FirstOrDefault());
            }
            else
            {
                PlayingService.PlayingListId = PlaylistInfo.Id;
                await PlayingService.ChangePlayingSong(item.Id, Songs, item);
            }
        });

        public ICommand CheckCommentCommand => new DelegateCommand(async () =>
        {
            Controls.WaitingPopup.Show();
            CommentRoot commentRoot = await CommentService.GetPlaylistCommentAsync(PlaylistInfo.Id);
            Controls.WaitingPopup.Hide();
            NavigateService.NavigateToComment(commentRoot);
        });
    }
}
