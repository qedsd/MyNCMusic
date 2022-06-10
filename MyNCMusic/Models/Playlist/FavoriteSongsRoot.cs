using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class FavoriteSongsRoot
    {
        public List<long> Ids { get; set; }
        public long CheckPoint { get; set; }
        public int Code { get; set; }
        public List<MusicItem> Songs { get; set; }
    }
}
