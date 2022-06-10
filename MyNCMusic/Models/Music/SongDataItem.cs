using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class SongDataItem
    {
        public int Id { get; set; }
        
        public string Url { get; set; }
        
        public int Br { get; set; }
        
        public int Size { get; set; }
        
        public string Md5 { get; set; }
        
        public int Code { get; set; }
       
        public int Expi { get; set; }
        
        public string Type { get; set; }
        
        public double Gain { get; set; }
        
        public int Fee { get; set; }
       
        public string Uf { get; set; }
      
        public int Payed { get; set; }
        
        public int Flag { get; set; }
       
        public string CanExtend { get; set; }
       
        public string FreeTrialInfo { get; set; }
        
        public string Level { get; set; }
        
        public string EncodeType { get; set; }
    }
}
