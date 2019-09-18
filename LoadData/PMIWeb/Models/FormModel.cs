using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMIWeb.Models
{
    public class FormModel
    {
        public string GeneralSearch { get; set; }

        /// <summary>
        /// Datos numericos
        /// </summary>
        private decimal pRGMID_VAL_1 = -1;
        private string dURATIONMS_VAL_1 = "-1";
        private decimal bITRATE_VAL_1 = -1;
        private decimal fILESIZE_VAL_1 = -1;
        private string cREATETIME_VAL_1 = "-1";
        private string eDITTIME_VAL_1 = "-1";
        private string cHECKTIME_VAL_1 = "-1";
        private string dIRECTORY_VAL = "-1";


        public decimal PRGMID_VAL_1
        {
            get { return pRGMID_VAL_1; }
            set { pRGMID_VAL_1 = value; }
        }
        public decimal PRGMID_VAL_2 { get; set; }
        public string PRGMID_COMP { get; set; }
        public string DURATIONMS_VAL_1
        {
            get { return dURATIONMS_VAL_1; }
            set { dURATIONMS_VAL_1 = value; }
        }
        public string DURATIONMS_VAL_2 { get; set; }
        public string DURATIONMS_COMP { get; set; } 
        public decimal BITRATE_VAL_1
        {
            get { return bITRATE_VAL_1; }
            set { bITRATE_VAL_1 = value; }
        }
        public decimal BITRATE_VAL_2 { get; set; }
        public string BITRATE_COMP { get; set; } 
        public string BITRATE_TYPE { get; set; }
        public decimal FILESIZE_VAL_1
        {
            get { return fILESIZE_VAL_1; }
            set { fILESIZE_VAL_1 = value; }
        }
        public decimal FILESIZE_VAL_2 { get; set; }
        public string FILESIZE_COMP { get; set; }
        public string FILESIZE_TYPE { get; set; }
        /// <summary>
        /// Fechas
        /// </summary>
        public string CREATETIME_VAL_1
        {
            get { return cREATETIME_VAL_1; }
            set { cREATETIME_VAL_1 = value; }
        }
        public string CREATETIME_VAL_2 { get; set; }
        public string CREATETIME_COMP { get; set; }
        public string EDITTIME_VAL_1
        {
            get { return eDITTIME_VAL_1; }
            set { eDITTIME_VAL_1 = value; }
        }
        public string EDITTIME_VAL_2 { get; set; }
        public string EDITTIME_COMP { get; set; }
        public string CHECKTIME_VAL_1
        {
            get { return cHECKTIME_VAL_1; }
            set { cHECKTIME_VAL_1 = value; }
        }
        public string CHECKTIME_VAL_2 { get; set; }
        public string CHECKTIME_COMP { get; set; }
        public string PRGMTYPENAME_VAL { get; set; }
        public string SUBTYPENAME_VAL { get; set; }
        public string VIDEOFILENAME_VAL { get; set; }
        public string PRGMNAME_VAL { get; set; }
        public string CREATOR_VAL { get; set; }
        public string EDITOR_VAL { get; set; }
        public string CHECKOR_VAL { get; set; }
        public string DIRECTORY_VAL
        {
            get { return dIRECTORY_VAL; }
            set
            {
                dIRECTORY_VAL = value != null
                    ? value.Replace(
                        @"\\", @"\")
                    : value;
            }
        }
        public string LOCKED_VAL { get; set; }
        public string CHECKUP_VAL { get; set; }
    }
}
