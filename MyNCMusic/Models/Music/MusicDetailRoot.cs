using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class MusicDetailRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public List<MusicItem> Songs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PrivilegesItem> Privileges { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Code { get; set; }
    }
}
