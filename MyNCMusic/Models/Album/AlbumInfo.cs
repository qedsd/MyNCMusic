using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class AlbumInfo
    {
        public CommentThread CommentThread { get; set; }
        
        public string Liked { get; set; }
        
        public string Comments { get; set; }
        
        public int ResourceType { get; set; }
        
        public int ResourceId { get; set; }
        
        public int CommentCount { get; set; }
        
        public int LikedCount { get; set; }
        
        public int ShareCount { get; set; }
        
        public string ThreadId { get; set; }
    }
}
