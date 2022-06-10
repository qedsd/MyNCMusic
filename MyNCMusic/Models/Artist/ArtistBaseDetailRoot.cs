using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class ArtistBaseDetailRoot
    {
        public Artist Artist { get; set; }
        
        public List<MusicItem> HotSongs { get; set; }
        
        public string More { get; set; }
        
        public int Code { get; set; }
    }
}
