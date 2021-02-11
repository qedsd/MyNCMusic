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
    public static class DjRadioService
    {
        /// <summary>
        /// 获取用户创建的电台
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static async Task<DjRadio> GetUserCreatedRadio(long uid)
        {
            string result = await Task.Run(()=>Http.Get(ConfigService.ApiUri + @"/user/audio?uid=" + uid ));
            return JsonConvert.DeserializeObject<DjRadio>(result);
        }
        /// <summary>
        /// 获取用户订阅的电台
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static async Task<DjRadio> GetUserSublistRadio()
        {
            string result = await Task.Run(() => Http.Get(ConfigService.ApiUri + @"/dj/sublist"));
            return JsonConvert.DeserializeObject<DjRadio>(result);
        }

        /// <summary>
        /// 获取电台节目列表
        /// 此时获取到的节目不带有效播放链接
        /// </summary>
        /// <returns></returns>
        public static async Task<RadioPrograms> GetRadioSongItem(long id)
        {
            string result = await Task.Run(() => Http.Get(ConfigService.ApiUri + @"/dj/program?rid="+id+ "&limit=1000"));
            return JsonConvert.DeserializeObject<RadioPrograms>(result);
        }

    }
}
