using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PMIWeb.Utils
{
    public class NpoiMemoryStream : MemoryStream
    {
        public bool AllowClose { get; set; }
        public NpoiMemoryStream()
        {
            AllowClose = true;
        }

        public override void Close()
        {
            if (AllowClose)
            {
                base.Close();
            }
        }
    }
}
