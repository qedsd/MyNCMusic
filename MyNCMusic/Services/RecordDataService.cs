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
    public static class RecordDataService
    {
        /// <summary>
        /// 获取听歌记录，注意，此时获取得到的SongItem不包含艺术家等详细信息
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static RecordData GetRecordData(long uid, int type)
        {
            string result = Http.Get(ConfigService.ApiUri + @"/user/record?uid=" + uid+ "&type="+type);
            return JsonConvert.DeserializeObject<RecordData>(result);
        }
    }
}
