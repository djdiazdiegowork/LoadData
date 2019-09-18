using System;
using System.Collections.Generic;
using System.Text;

namespace Common.PMI
{
    /// <summary>
    /// Clase que representa un tipo de programa
    /// de television.
    /// </summary>
    public class PRGMTYPETB
    {
        public long PRGMTYPEID { get; set; }
        public string PRGMTYPENAME { get; set; }
        public string PRGMTYPEDESCP { get; set; }
        public IEnumerable<PRGMSUBTYPETB> Prgmsubtypetbs { get; set; }
    }
}
