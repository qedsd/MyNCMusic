using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class CADataItem
    {
        public long SubTime { get; set; }
        
        public List<string> Msg { get; set; }
        
        public List<string> Alias { get; set; }
        
        public List<Artist> Artists { get; set; }
       
        public long PicId { get; set; }
        
        public string PicUrl { get; set; }
        
        public string Name { get; set; }
        
        public long Id { get; set; }
        
        public int Size { get; set; }
        
        public List<string> TransNames { get; set; }
    }
}
