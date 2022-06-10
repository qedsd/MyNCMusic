using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class AlbumRoot
    {
        public List<MusicItem> Songs { get; set; }
        
        public int Code { get; set; }
        
        public Album Album { get; set; }
    }
}
