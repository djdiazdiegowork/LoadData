using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.PMI;
using PMILoadDataClient;
using PMILoadDataClient.Utils.Enums;
using PageOperation = Common.PageOperation;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Probando GetTraces

            //Task.Run(GetTraces);
            //Task.Run(GetPrgTbs);

            ////var kg = 76d;
            ////var meter = 1.59d;

            ////var imc = kg / Math.Pow(meter, 2);
            ////Console.WriteLine(imc);

            #endregion

            #region Probando async
            ////Probando el async
            //Task.Run(PROCESO_AMBITO_ATRIBUTO);
            //Thread.Sleep(TimeSpan.FromSeconds(10));
            #endregion

            #region Obteniendo el path del ejecutable
            //var logsDirectory = Path.Combine(Environment.CurrentDirectory, "logs");
            //Directory.CreateDirectory(logsDirectory);
            //Console.WriteLine(logsDirectory);
            #endregion

            #region Testing DateTime String Format

            //var dateTime = DateTime.Now.Add(TimeSpan.FromHours(8));
            //Console.WriteLine(dateTime.ToString("yyyy-MM-dd H:mm:ss"));

            #endregion

            #region Probando el string con el string.format

            //String a = "juan";
            //var toPrint = $"{a}";
            //Console.WriteLine(toPrint);
            //a = "jose";
            //Console.WriteLine(toPrint);

            #endregion

            #region Probando el visualizado custom de los enums

            //var tempTable = TempTable.PROGRAMTB_PRGMNAME;
            //Console.WriteLine($"{tempTable:G}");

            #endregion

            #region Parseando con timespan

            var txt = "0:00:10 PM";
            var format = txt.Length > 10 ? "hh:mm:ss tt" : "h:mm:ss tt";

            var dateTime = DateTime.ParseExact(txt, format, CultureInfo.InvariantCulture);
            var result = dateTime.TimeOfDay;
            var hours = result.Hours;
            var min = result.Minutes;
            var seconds = result.Seconds;

            var totalMili = result.TotalMilliseconds;

            Console.WriteLine("El tiempo es : horas: {0}, minutos: {1}, segundos: {2}", hours, min, seconds);
            Console.WriteLine("El total de milisegundos es : {0}", totalMili);

            #endregion

            Console.ReadLine();
        }

        static async Task GetPrgTbs()
        {
            var timeIni = DateTime.MinValue;
            var timeEnd = DateTime.MaxValue;

            var timeIniSolr = "2019-01-08 16:10:03";
            var timeEndSolr = "2019-01-10 16:10:03";

            var atributesToProcess = new List<AttributeBase>
            {
                //********************************SELECT INSTANCIA*************************************
                new AttributeSelect("PRGMID", AttributeType.Instance, AttributeDataType.TypeDouble),
                new AttributeSelect("PRGMNAME", AttributeType.Instance, AttributeDataType.TypeString),
                new AttributeSelect("PRGMTYPEID", AttributeType.Instance, AttributeDataType.TypeDouble),
                new AttributeSelect("VIDEOFILENAME", AttributeType.Instance, AttributeDataType.TypeString),
                new AttributeSelect("DIRECTORY", AttributeType.Instance, AttributeDataType.TypeString),
                new AttributeSelect("DURATIONMS", AttributeType.Instance, AttributeDataType.TypeDouble),
                new AttributeSelect("CREATOR", AttributeType.Instance, AttributeDataType.TypeString),
                new AttributeSelect("CREATETIME", AttributeType.Instance, AttributeDataType.TypeDate),
                new AttributeSelect("EDITOR", AttributeType.Instance, AttributeDataType.TypeString),
                new AttributeSelect("EDITTIME", AttributeType.Instance, AttributeDataType.TypeDate),
                new AttributeSelect("SUBTYPEID", AttributeType.Instance, AttributeDataType.TypeDouble),
                new AttributeSelect("BITRATE", AttributeType.Instance, AttributeDataType.TypeDouble),
                new AttributeSelect("FILESIZE", AttributeType.Instance, AttributeDataType.TypeDouble),
                new AttributeSelect("LOCKED", AttributeType.Instance, AttributeDataType.TypeDouble),
                new AttributeSelect("CHECKOR", AttributeType.Instance, AttributeDataType.TypeString),
                new AttributeSelect("CHECKTIME", AttributeType.Instance, AttributeDataType.TypeDate),
                new AttributeSelect("CHECKUP", AttributeType.Instance, AttributeDataType.TypeDouble),
                //**********************************************************************************

                new AttributeFilter("PRGMNAME", AttributeType.Instance, AttributeDataType.TypeString,
                    new List<string>() {"Ben10"}, OperatorForValues.Equal, OperatorForLinkAtribute.None,
                    ParenthesisThisAtribute.None),
                //new AttributeFilter("TIEMPO", AttributeType.Event, AttributeDataType.TypeDate,
                //    new List<string>() {timeIniSolr, timeEndSolr}, OperatorForValues.Between,
                //    OperatorForLinkAtribute.AndOperator, ParenthesisThisAtribute.None),
                //new AttributeFilter("ID_ENTIDAD", AttributeType.Instance, AttributeDataType.TypeString,
                //    new List<string>() {"1", "2", "3", "4"}, OperatorForValues.Equal, OperatorForLinkAtribute.None,
                //    ParenthesisThisAtribute.None),
            };


            var loadDataClient =
                new LoadDataClient("https://localhost:44368", 1, atributesToProcess, true);
            await loadDataClient.Connect("mariana/172.10.10.10");

            IEnumerable<PROGRAMTB> datas = null;
            var iterIndex = 0;
            //// Ejemplo de ciclo infinito (es solo para probar el next y el previous)
            do
            {
                Console.WriteLine("Iterando");
                //var pageOperation = iterIndex++ % 2 == 0 ? PageOperation.Next : PageOperation.Previous;
                var pageOperation = iterIndex++ <= 1  ? PageOperation.Next : PageOperation.Previous;
                datas = await loadDataClient.FetchData(pageOperation);
                if (datas != null)
                    Console.WriteLine(string.Join(",", datas.Select(prg => prg.PRGMID)));

            } while (datas != null);

            Console.WriteLine("FIN");
        }
    }
}
