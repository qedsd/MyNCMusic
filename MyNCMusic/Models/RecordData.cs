using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class RecordData: JsonBaseObject
    {
        public List<RecordDataItem> WeekData { get; set; }
        public List<RecordDataItem> AllData { get; set; }
    }
    public class RecordDataItem
    {
        /// <summary>
        /// 不知道干嘛用的
        /// </summary>
        public int PlayCount { get; set; }
        /// <summary>
        /// 播放次数
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MusicItem Song { get; set; }
    }
}

