using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class MyCollectionfAlbumRoot
    {
        public List<CADataItem> Data { get; set; }
        
        public int Count { get; set; }
        
        public string HasMore { get; set; }
        
        public string Cover { get; set; }
        
        public int PaidCount { get; set; }
        
        public int Code { get; set; }
    }
}
