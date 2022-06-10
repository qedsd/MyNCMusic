using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class RecommendMusicData
    {
        public List<MusicItem> DailySongs { get; set; }
        public List<string> OrderSongs { get; set; }
    }
}
