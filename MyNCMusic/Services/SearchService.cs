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
    public static class SearchService
    {
        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="type">1: 单曲, 10: 专辑, 100: 歌手, 1000: 歌单</param>
        /// <returns></returns>
        public static async Task<SearchRoot> SearchCloundAsync(string keyword, int type)
        {
            string result = await Http.GetAsync(ConfigService.ApiUri + @"/cloudsearch?keywords=" + keyword + "&type=" + type);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<SearchRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }
    }
}
