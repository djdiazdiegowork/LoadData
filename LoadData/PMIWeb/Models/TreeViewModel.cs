using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMIWeb.Models
{
    public class TreeViewModel
    {
        public string text { get; set; }
        public List<TreeViewModel> nodes { get; set; }
    }
}
