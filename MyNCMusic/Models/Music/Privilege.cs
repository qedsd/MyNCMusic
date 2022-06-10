using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    /// <summary>
    /// 未知
    /// </summary>
    public class Privilege
    {
        public int Id { get; set; }
        public int Fee { get; set; }
        public int Payed { get; set; }
        public int St { get; set; }
        public int Pl { get; set; }
        public int Dl { get; set; }
        public int Sp { get; set; }
        public int Cp { get; set; }
        public int Subp { get; set; }
        public string Cs { get; set; }
        public int Maxbr { get; set; }
        public int Fl { get; set; }
        public string Toast { get; set; }
        public int Flag { get; set; }
        public string PreSell { get; set; }


        public int PlayMaxbr { get; set; }
        public int DownloadMaxbr { get; set; }
        public List<ChargeInfoListItem> ChargeInfoList { get; set; }
    }
}
