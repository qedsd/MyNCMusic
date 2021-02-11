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
    public static class SongService
    {
        /// <summary>
        /// 获取歌曲详细
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static MusicDetailRoot GetMusicDetail_Get(string ids)
        {
            string result = Http.Get(ConfigService.ApiUri + @"/song/detail?ids=" + ids);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<MusicDetailRoot>(result);
            }
            catch (Exception) { return null; }
        }
        /// <summary>
        /// 获取歌曲详细
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static MusicDetailRoot GetMusicDetail_Post(string ids)
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);

            string result = Http.Post(ConfigService.ApiUri + @"/song/detail?timestamp=" + Convert.ToInt64(ts.TotalSeconds).ToString(), "ids=" + ids);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<MusicDetailRoot>(result);
            }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// 获取歌曲url
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static SongUrlRoot GetMusicUrl(long id)
        {
            string result = Http.Get(ConfigService.ApiUri + @"/song/url?id=" + id);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<SongUrlRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 获取喜欢的歌曲
        /// </summary>
        /// <returns></returns>
        public static FavoriteSongsRoot GetFavoriteSongs()
        {
            string result = Http.Get(ConfigService.ApiUri + @"/likelist");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<FavoriteSongsRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 获取相似歌曲
        /// </summary>
        /// <param name="id">音乐id</param>
        /// <returns></returns>
        public static SimiSongsRoot GetSimiSongs(long id)
        {
            string result = Http.Get(ConfigService.ApiUri + @"/simi/song?id=" + id);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<SimiSongsRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 喜欢/不喜欢歌曲
        /// </summary>
        /// <param name="id">音乐id</param>
        /// <param name="b">true 即喜欢 , 若传 false, 则取消喜欢</param>
        /// <returns></returns>
        public static bool LoveOrDontLoveSong(long id, bool b)
        {
            string result = Http.Get(ConfigService.ApiUri + @"/like?id=" + id + "&like=" + b.ToString());
            if (result == null || result.Equals(""))
                return false;
            else
                return true;
        }

        /// <summary>
        /// 听歌打卡
        /// </summary>
        /// <param name="songId">歌曲id</param>
        /// <param name="sourceId">源id（专辑、歌单）,仅当</param>
        /// <param name="s">持续时间（秒）</param>
        /// <returns></returns>
        public static void MarkPlayDuration(long songId, long sourceId, long s)
        {
            if (s < 3)
                return;
            Http.Get(ConfigService.ApiUri + @"/scrobble?id=" + songId + "&sourceid=" + sourceId + "&time=" + s);
        }

        
    }
}
