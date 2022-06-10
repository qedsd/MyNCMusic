using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class PlayListDetailRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RelatedVideos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PlaylistItem Playlist { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Urls { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PrivilegesItem> Privileges { get; set; }
    }
}
