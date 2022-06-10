using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class MyCollectionfArtistRoot
    {
        public List<Artist> Data { get; set; }
        
        public string HasMore { get; set; }
        
        public int Count { get; set; }
       
        public int Code { get; set; }
    }
}
