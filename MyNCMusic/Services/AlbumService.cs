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
    public static class AlbumService
    {
        /// <summary>
        /// 获取专辑详细
        /// </summary>
        /// <param name="id">专辑id</param>
        /// <returns></returns>
        public static AlbumRoot GetAlbum(long id)
        {
            string result = Http.Get(ConfigService.ApiUri + @"/album?id=" + id);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<AlbumRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 获取我收藏的专辑
        /// </summary>
        /// <returns></returns>
        public static MyCollectionfAlbumRoot GetMyCollectionOfAlbum()
        {
            string result = Http.Get(ConfigService.ApiUri + @"/album/sublist?limit=1000");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<MyCollectionfAlbumRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 获取歌手所有（热门）专辑信息
        /// </summary>
        /// <param name="id">歌手id</param>
        /// <returns></returns>
        public static ArtistAllAlbumRoot GetArtistAllAlbums(long id)
        {
            string result = Http.Get(ConfigService.ApiUri + @"/artist/album?id=" + id + "&limit=1000");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<ArtistAllAlbumRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }
    }
}
