using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class ResponseRoot<T>
    {
        public int Code { get; set; }
        public T Data { get; set; }
    }
}
