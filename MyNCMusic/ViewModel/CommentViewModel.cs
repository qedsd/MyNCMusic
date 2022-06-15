using MyNCMusic.Controls;
using MyNCMusic.Models;
using MyNCMusic.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyNCMusic.ViewModel
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    internal class CommentViewModel
    {
        public List<CommentItem> HotComments { get; set; }
        public List<CommentItem> AllComments { get; set; }
        public CommentViewModel(CommentRoot commentRoot)
        {
            HotComments = commentRoot.HotComments;
            AllComments = commentRoot.Comments;
        }
    }
}
