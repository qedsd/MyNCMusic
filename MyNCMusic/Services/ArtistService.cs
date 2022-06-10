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
    public static class ArtistService
    {
        /// <summary>
        /// 获取我收藏的歌手
        /// </summary>
        /// <returns></returns>
        public static MyCollectionfArtistRoot GetMyCollectionOfArtist()
        {
            string result = Http.Get(ConfigService.ApiUri + @"/artist/sublist");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<MyCollectionfArtistRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }
        /// <summary>
        /// 获取我收藏的歌手
        /// </summary>
        /// <returns></returns>
        public static async Task<MyCollectionfArtistRoot> GetMyCollectionOfArtistAsync()
        {
            string result = await Http.GetAsync(ConfigService.ApiUri + @"/artist/sublist");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<MyCollectionfArtistRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 获取歌手基本详细信息
        /// </summary>
        /// <param name="id">歌手id</param>
        /// <returns></returns>
        public static ArtistBaseDetailRoot GetArtistBaseDetail(long id)
        {
            string result = Http.Get(ConfigService.ApiUri + @"/artists?id=" + id);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<ArtistBaseDetailRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }
        /// <summary>
        /// 获取歌手基本详细信息
        /// </summary>
        /// <param name="id">歌手id</param>
        /// <returns></returns>
        public static async Task<ArtistBaseDetailRoot> GetArtistBaseDetailAsync(long id)
        {
            string result = await Http.GetAsync(ConfigService.ApiUri + @"/artists?id=" + id);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<ArtistBaseDetailRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 返回所有艺术家名字组成的string
        /// </summary>
        /// <param name="arItems">艺术家list</param>
        /// <returns></returns>
        public static string GetArNames_ArtistsItem(List<Artist> arItems)
        {
            string name = "";
            for (int i = 0; i < arItems.Count; i++)
            {
                if (i != 0)
                    name += "/";
                name += arItems[i].Name;
            }
            return name;
        }
    }
}
