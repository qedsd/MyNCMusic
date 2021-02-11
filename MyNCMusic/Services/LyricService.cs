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
    public static class LyricService
    {
        /// <summary>
        /// 分析提取歌词
        /// </summary>
        /// <param name="lyricRoot">原始获得的歌词实例</param>
        /// <returns>用于滚动显示的歌词</returns>
        public static List<LyricStr> GetLyricStrs(LyricRoot lyricRoot)
        {
            List<LyricStr> lyricStrs = new List<LyricStr>();
            if (lyricRoot.lrc != null && lyricRoot.lrc.lyric != null)//原歌词
            {
                var array = lyricRoot.lrc.lyric.Split("\n");
                foreach (var temp in array)
                {
                    var array2 = temp.Split(new char[2] { '[', ']' });
                    if (array2.Length != 3)
                        continue;
                    var array3 = array2[1].Split(new char[2] { ':', '.' });
                    if (array3.Length != 3)
                        continue;
                    DateTime dateTime = new DateTime();
                    try
                    {
                        dateTime = dateTime.AddMinutes(double.Parse(array3[0]));
                        dateTime = dateTime.AddSeconds(double.Parse(array3[1]));
                        dateTime = dateTime.AddMilliseconds(double.Parse(array3[2]));
                        lyricStrs.Add(new LyricStr() { DateTime = dateTime, Original = array2[2] });
                    }
                    catch (Exception) { }
                }
            }
            if (lyricRoot.tlyric != null && lyricRoot.tlyric.lyric != null)//翻译歌词
            {
                var array = lyricRoot.tlyric.lyric.Split("\n");
                foreach (var temp in array)
                {
                    var array2 = temp.Split(new char[2] { '[', ']' });
                    if (array2.Length != 3)
                        continue;
                    var array3 = array2[1].Split(new char[2] { ':', '.' });
                    if (array3.Length != 3)
                        continue;
                    DateTime dateTime = new DateTime();
                    try
                    {
                        dateTime = dateTime.AddMinutes(double.Parse(array3[0]));
                        dateTime = dateTime.AddSeconds(double.Parse(array3[1]));
                        dateTime = dateTime.AddMilliseconds(double.Parse(array3[2]));
                        var found = lyricStrs.Find(p => p.DateTime.TimeOfDay == dateTime.TimeOfDay);
                        if (found != null)
                            found.Tran = array2[2];
                        else
                            lyricStrs.Add(new LyricStr() { DateTime = dateTime, Original = array2[2] });
                    }
                    catch (Exception) { }
                }
            }

            return lyricStrs;
        }

        /// <summary>
        /// 获取歌词
        /// </summary>
        /// <param name="id">音乐id</param>
        /// <returns></returns>
        public static LyricRoot GetLyric(long id)
        {
            string result = Http.Get(ConfigService.ApiUri + @"/lyric?id=" + id);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<LyricRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }
    }
}
