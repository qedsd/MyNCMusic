using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Model
{
    abstract public class JsonBaseObject
    {
        public int Code { get; set; }
    }
    abstract public class PlayingSongBaseObject: INotifyPropertyChanged
    {
        public long Id { get; set; }
        public string Name { get; set; }
        //是否在播放
        public bool _IsPlaying = false;
        public bool IsPlaying
        {
            get { return _IsPlaying; }
            set
            {
                _IsPlaying = value;
                NotifyPropertyChanged("IsPlaying");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
