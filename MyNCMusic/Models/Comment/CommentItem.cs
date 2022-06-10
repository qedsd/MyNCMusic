using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class CommentItem
    {
        public CommentUser User { get; set; }
        
        public string ShowFloorComment { get; set; }
        
        public int Status { get; set; }
        
        public long CommentId { get; set; }

        public string Content { get; set; }
        
        public long Time { get; set; }
        
        public int LikedCount { get; set; }
        
        public string ExpressionUrl { get; set; }
        
        public int CommentLocationType { get; set; }
        
        public long ParentCommentId { get; set; }

        public string RepliedMark { get; set; }
        
        public string Liked { get; set; }
    }
}
