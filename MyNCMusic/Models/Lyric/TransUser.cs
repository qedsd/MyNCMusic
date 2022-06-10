using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class TransUser
    {
        public long Id { get; set; }
        
        public int Status { get; set; }
        
        public int Demand { get; set; }
        
        public long Userid { get; set; }
        
        public string Nickname { get; set; }
        
        public long Uptime { get; set; }
    }
}
