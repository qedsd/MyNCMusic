using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class MyPlaylistRoot
    {
        public string Version { get; set; }
        
        public string More { get; set; }
        
        public List<PlaylistItem> Playlist { get; set; }
        
        public int Code { get; set; }
    }
}
