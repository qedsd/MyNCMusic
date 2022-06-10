using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class CommentUser
    {
        public string LocationInfo { get; set; }
        
        public string LiveInfo { get; set; }
        
        public int Anonym { get; set; }
        
        public string AvatarUrl { get; set; }
        
        public int AuthStatus { get; set; }
        
        public VipRights VipRights { get; set; }
        
        public long UserId { get; set; }
        
        public int UserType { get; set; }
        
        public string Nickname { get; set; }
        
        public int VipType { get; set; }
        
        public string RemarkName { get; set; }
    }
}
