using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class CommentThread
    {
        public string Id { get; set; }
        
        public ResourceInfo ResourceInfo { get; set; }
        
        public int ResourceType { get; set; }
        
        public int CommentCount { get; set; }
        
        public int LikedCount { get; set; }
        
        public int ShareCount { get; set; }
        
        public int HotCount { get; set; }
        
        public int ResourceId { get; set; }
        
        public int ResourceOwnerId { get; set; }
        
        public string ResourceTitle { get; set; }
    }
}
