using MyNCMusic.Helper;
using MyNCMusic.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Services
{
    public static class CommentService
    {
        /// <summary>
        /// 获取歌曲评论
        /// </summary>
        /// <param name="id">音乐id</param>
        /// <returns></returns>
        public static CommentRoot GetSongsComment(long id)
        {
            string result = Http.Get(ConfigService.ApiUri + @"/comment/music?id=" + id + "&limit=100");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<CommentRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 获取专辑评论
        /// </summary>
        /// <param name="id">专辑id</param>
        /// <returns></returns>
        public static CommentRoot GetAlbumComment(long id)
        {
            string result = Http.Get(ConfigService.ApiUri + @"/comment/album?id=" + id + "&limit=100");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<CommentRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 获取歌单评论
        /// </summary>
        /// <param name="id">歌单id</param>
        /// <returns></returns>
        public static CommentRoot GetPlaylistComment(long id)
        {
            string result = Http.Get(ConfigService.ApiUri + @"/comment/playlist?id=" + id + "&limit=100");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<CommentRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 电台评论
        /// </summary>
        /// <param name="id">节目id，即RadioSongItem，不是MainSong</param>
        /// <returns></returns>
        public static CommentRoot GetRadioComment(long id)
        {
            string result = Http.Get(ConfigService.ApiUri + @"/comment/dj?id=" + id + "&limit=100");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<CommentRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }
    }
}
