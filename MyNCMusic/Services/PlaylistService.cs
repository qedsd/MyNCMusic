using MyNCMusic.Helper;
using MyNCMusic.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Services
{
    public static class PlaylistService
    {
        /// <summary>
        /// 获取歌单详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static PlayListDetailRoot GetPlaylistDetail(long id)
        {
            string result = Http.Get(ConfigService.ApiUri + @"/playlist/detail?id=" + id);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<PlayListDetailRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }
        /// <summary>
        /// 获取歌单详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<PlayListDetailRoot> GetPlaylistDetailAsync(long id)
        {
            string result = await Http.GetAsync(ConfigService.ApiUri + @"/playlist/detail?id=" + id);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<PlayListDetailRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 获取我的歌单
        /// </summary>
        /// <returns></returns>
        public static MyPlaylistRoot GetMyPlaylist()
        {
            string result = Http.Get(ConfigService.ApiUri + @"/user/playlist?uid=" + ConfigService.Uid + "&limit=1000");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<MyPlaylistRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }
        /// <summary>
        /// 获取我的歌单
        /// </summary>
        /// <returns></returns>
        public static async Task<MyPlaylistRoot> GetMyPlaylistAsync()
        {
            string result = await Http.GetAsync(ConfigService.ApiUri + @"/user/playlist?uid=" + ConfigService.Uid + "&limit=1000");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<MyPlaylistRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 添加到歌单
        /// </summary>
        /// <param name="pid">歌单id</param>
        /// <param name="tracks">歌曲id</param>
        /// <returns></returns>
        public static bool AddToPlaylist(long pid, long tracks)
        {
            string result = Http.Get(ConfigService.ApiUri + @"/playlist/tracks?op=add&pid=" + pid + "&tracks=" + tracks);
            if (result == null || result.Equals(""))
                return false;
            return true;
        }

        /// <summary>
        /// 添加到歌单
        /// </summary>
        /// <param name="pid">歌单id</param>
        /// <param name="tracks">歌曲id</param>
        /// <returns></returns>
        public static async Task<bool> AddToPlaylistAsync(long pid, long tracks)
        {
            string result = await Http.GetAsync(ConfigService.ApiUri + @"/playlist/tracks?op=add&pid=" + pid + "&tracks=" + tracks);
            if (result == null || result.Equals(""))
                return false;
            return true;
        }

        /// <summary>
        /// 新建歌单
        /// </summary>
        /// <param name="privacy"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PlayListDetailRoot AddNewPlaylist(bool privacy, string name)
        {
            string result = Http.Get(ConfigService.ApiUri + @"/playlist/create?name=" + name + (privacy ? "&privacy=10" : ""));
            return JsonConvert.DeserializeObject<PlayListDetailRoot>(result);
        }

        /// <summary>
        /// 新建歌单
        /// </summary>
        /// <param name="privacy"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task<PlayListDetailRoot> AddNewPlaylistAsync(bool privacy, string name)
        {
            string result = await Http.GetAsync(ConfigService.ApiUri + @"/playlist/create?name=" + name + (privacy ? "&privacy=10" : ""));
            return JsonConvert.DeserializeObject<PlayListDetailRoot>(result);
        }

        /// <summary>
        /// 收藏/不收藏歌单
        /// </summary>
        /// <param name="id">歌单id</param>
        /// <param name="t">1:收藏,2:取消收藏</param>
        /// <returns></returns>
        public static bool SubOrCancelPlayList(long id, int t)
        {
            string result = Http.Get(ConfigService.ApiUri + @"/playlist/subscribe?t=" + t + "&id=" + id);
            if (result == null || result.Equals(""))
                return false;
            else
                return true;
        }

        /// <summary>
        /// 收藏/不收藏歌单
        /// </summary>
        /// <param name="id">歌单id</param>
        /// <param name="t">1:收藏,2:取消收藏</param>
        /// <returns></returns>
        public static async Task<bool> SubOrCancelPlayListAsync(long id, int t)
        {
            string result = await Http.GetAsync(ConfigService.ApiUri + @"/playlist/subscribe?t=" + t + "&id=" + id);
            if (result == null || result.Equals(""))
                return false;
            else
                return true;
        }

        /// <summary>
        /// 推荐歌单
        /// </summary>
        /// <returns></returns>
        public static async Task<RecommendListRoot> GetCommendatoryListAsync()
        {
            string result = await Http.GetAsync(ConfigService.ApiUri + @"/recommend/resource");
            if (result == null || result.Equals(""))
                result = await Http.GetAsync(ConfigService.ApiUri + @"/recommend/resource");
            if (result == null || result.Equals(""))
                return null;
            return JsonConvert.DeserializeObject<RecommendListRoot>(result);
        }
    }
}
