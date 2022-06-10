using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class ResourceInfo
    {
        public int Id { get; set; }
        
        public long UserId { get; set; }
        
        public string Name { get; set; }
        
        public string ImgUrl { get; set; }
        
        public string Creator { get; set; }
        
        public string EncodedId { get; set; }
        
        public string SubTitle { get; set; }
        
        public string WebUrl { get; set; }
    }
}
