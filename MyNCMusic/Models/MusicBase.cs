using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class MusicBase
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsPlaying { get; set; } = false;
        public bool IsRadio { get; set; } = false ;
    }
}
