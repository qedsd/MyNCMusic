using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class LyricRoot
    {
        public string Sgc { get; set; }
        
        public string Sfy { get; set; }
        
        public string Qfy { get; set; }
        
        public TransUser TransUser { get; set; }
        
        public Lrc Lrc { get; set; }
        
        public Klyric Klyric { get; set; }
        
        public Tlyric Tlyric { get; set; }
        
        public int Code { get; set; }
    }
}
