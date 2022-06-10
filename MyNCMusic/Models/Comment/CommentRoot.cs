using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class CommentRoot
    {
        public string IsMusician { get; set; }
        
        public int Cnum { get; set; }
        
        public int UserId { get; set; }
       
        public List<string> TopComments { get; set; }
        
        public string MoreHot { get; set; }
        
        public List<CommentItem> HotComments { get; set; }
        
        public string CommentBanner { get; set; }
        
        public int Code { get; set; }
        
        public List<CommentItem> Comments { get; set; }
        
        public int Total { get; set; }
        
        public string More { get; set; }
    }
}
