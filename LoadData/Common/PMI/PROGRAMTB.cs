using System;

namespace Common.PMI
{
    /// <summary>
    /// Clase que representa un programa de television
    /// </summary>
    public class PROGRAMTB
    {
        public long PRGMID { get; set; }
        public string PRGMNAME { get; set; }
        /*********SECCION PARA EL TIPO DE PROGRAMA********/
        public long? PRGMTYPEID { get; set; }
        public PRGMTYPETB PRGMTYPETB { get; set; }
        /*************************************************/
        public string VIDEOFILENAME { get; set; }
        public string DIRECTORY { get; set; }
        public decimal? DURATIONMS { get; set; }
        public string CREATOR { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string EDITOR { get; set; }
        public DateTime? EDITTIME { get; set; }
        /*****SECCION PARA EL SUB TIPO DE PROGRAMA********/
        public long? SUBTYPEID { get; set; }
        public PRGMSUBTYPETB PRGMSUBTYPETB { get; set; }
        /*************************************************/
        public decimal? BITRATE { get; set; }
        public float? FILESIZE { get; set; }
        public decimal? LOCKED { get; set; }
        public string CHECKOR { get; set; }
        public DateTime? CHECKTIME { get; set; }
        public decimal? CHECKUP { get; set; }
    }
}
