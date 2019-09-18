using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.OracleClient;
using Common;
using Common.PMI;
using LoadDataService.Models.Context;
using LoadDataService.Models.Entities;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using WM.AVL;

namespace LoadDataService.Controllers
{
    /// <summary>
    /// Controller para el proceso de extraccion
    /// de los datos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LoadDataController : ControllerBase
    {
        private QueryContext _context;
        private IConfiguration _iConfig;
        public LoadDataController(QueryContext context, IConfiguration iConfig)
        {
            _context = context;
            _iConfig = iConfig;
        }

        // GET api/values
        /// <summary>
        /// Action para crear una session en el servidor para el cliente.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        [HttpPost("CreateSession")]
        public int CreateSession(Config configuration)
        {
            var queryOld = _context.Sessions.FirstOrDefault(q => q.ClientId == configuration.ClientId);
            var dateTimeNow = DateTime.Now;
            //// El proceso de adicion de una nueva sesion se produce solamente cuando
            //// existe una session vieja que hay que eliminar o si no hay ninguna 
            if ((queryOld != null && dateTimeNow - queryOld.LastModification >
                TimeSpan.FromMilliseconds(configuration.Expiration) )|| queryOld == null)
            {
                if (queryOld != null)
                    _context.Sessions.Remove(queryOld);
                //// Generando los string queries para cada una de las tablas.
                var stringQueryPmi = CreateStringQueryPMI(configuration);

                var queryEntity = new Session
                {
                    ClientId = configuration.ClientId,
                    StringQueryPMI = stringQueryPmi,
                    PreviousPages = "0",
                    CurrentPage = 0,
                };

                _context.Sessions.Add(queryEntity);
                _context.SaveChanges();
            }

            return 1;
        }

        /// <summary>
        /// Metodo que se va a utilizar para obtener el valor de un atribute Filter
        /// dado su nombre.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private string FindAttribute(IEnumerable<AttributeFilter> attributes, string name)
        {
            var first = attributes.FirstOrDefault(attr => attr.Name == name);
            return first != null? first.Values[0]: "";
        }

        /// <summary>
        /// Metodo que se encarga de generar el string query
        /// para la tabla PROGRAMTB.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private string CreateStringQueryPMI(Config config)
        {
            var stringQueryResult = string.Empty;

            //// El cuerpo del select
            var selectBody = string.Join(", ", config.AttributesToProcess.Where(attr => attr is AttributeSelect)
                .Select(attr => attr.ToStringQuery()));

            var whereBodyFull = string.Empty;
            if (config.AttributesToProcess.Any(attr => attr is AttributeFilter))
            {
                var whereBody = string.Join(" ", config.AttributesToProcess.Where(attr => attr is AttributeFilter)
                    .Select(attr => attr.ToStringQuery()));
                whereBodyFull = $"AND CONTAINS (DUMMY, '{whereBody}') > 0";
            }

            stringQueryResult = $"SELECT {selectBody} FROM (SELECT {selectBody} FROM PROGRAMTB WHERE PRGMID > :lastId {whereBodyFull} ORDER BY PRGMID ASC) WHERE ROWNUM <= {config.RowSize}";
            return stringQueryResult;
        }



        /// <summary>
        /// Metodo que se va a utilizar para encontrar un conjunto de atributos filter
        /// dentro del conjunto de atributos a procesar.
        /// </summary>
        /// <param name="attributeToProcess"></param>
        /// <returns></returns>
        private void MarkAttributes(List<AttributeBase> attributeToProcess,
            List<string> namesReales)
        {
            foreach (var attr in attributeToProcess)
            {
                attr.ColumnType = namesReales.Contains(attr.Name) ? ColumnType.Real : ColumnType.Dummy;
            }
        }
        

        /// <summary>
        /// Metodo que se va a encargar de cerrar la session con
        /// el cliente.
        /// </summary>
        /// <returns></returns>
        [HttpPost("Close")]
        public ActionResult Close(decimal systemIndentity)
        {
            return null;
        }

        [HttpGet("Test")]
        public ActionResult Test()
        {
            return Content("asasa");
        }

        /// <summary>
        /// Action que se va a encargar de realizar el proceso
        /// de extraccion.
        /// </summary>
        /// <param name="systemIndentity">el valor que identifica el proceso
        /// de extraccion del cliente en el servidor</param>
        /// <param name="pageOperation">valor que indica el tipo
        /// de bloque de datos que se va a pedir.</param>
        [HttpGet("Fetch")]
        public IEnumerable<PROGRAMTB> Fetch(string systemIndentity, PageOperation pageOperation)
        {
            //// listado de trazas que van a ser retornadas.
            var result = new List<PROGRAMTB>();
            var session = _context.Sessions.FirstOrDefault(s => s.ClientId == systemIndentity);

            //// Obteniendo el identificador que se va a utilizar para
            //// realizar el proceso de paginado. 
            var currentPage = session.CurrentPage;
            var previousPages = session.PreviousPages.Split(",");
            var fetchPage = 0;
            var idFetchPage = GetLastId(currentPage, previousPages, pageOperation, out fetchPage);

            //// Atualizando la fecha de modificacion de la sesion.
            session.LastModification = DateTime.Now;
            _context.Update(session);
            _context.SaveChanges();

            //// Sacar el query asociado a la session
            var stringQueryPmi = session.StringQueryPMI;

            //// Listado de los identificadores de los
            //// PRGMTYPETB
            var idPrgTypeTbList = new List<decimal?>();
            //// Listado de los identificadores de los
            //// PRGMSUBTYPETB
            var idPrgSubTypeTbList = new List<decimal?>();

            using (var conn =
                new OracleConnection(_iConfig.GetConnectionString("OracleConnection")))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = stringQueryPmi;
                cmd.Parameters.Add(new OracleParameter("lastId", idFetchPage));

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        decimal? idPgrTypeTb = null;
                        decimal? idPgrSubTypeTb = null;

                        var programtb = CreateFromReader(reader, out idPgrTypeTb, out idPgrSubTypeTb);
                        result.Add(programtb);

                        idPrgTypeTbList.Add(idPgrTypeTb);
                        idPrgSubTypeTbList.Add(idPgrSubTypeTb);
                    }
                }
            }

            //// En caso de que no se encuentren datos se va a devolver null
            if (!result.Any()) return null;

            var getPRGMTYPETBFromDBTask = GetPRGMTYPETBFromDB(idPrgTypeTbList);
            var getPRGMSUBTYPETBFromDB = GetPRGMSUBTYPETBFromDB(idPrgSubTypeTbList);

            var avlPRGMTYPETBs = getPRGMTYPETBFromDBTask;
            var avlPRGMSUBTYPETBs = getPRGMSUBTYPETBFromDB;

            ////Rellenando los PROGRAMTB con sus PRGMTYPETB
            //// y con sus PRGMSUBTYPETB
            for (int i = 0; i < result.Count; i++)
            {
                var pROGRAMTB = result[i];
                ////Para el PRGMTYPETB
                if (pROGRAMTB.PRGMTYPEID.HasValue)
                {
                    var pgrTypeId = pROGRAMTB.PRGMTYPEID.Value;
                    PRGMTYPETB prgTypeTb = null;
                    if (avlPRGMTYPETBs.TryGetValue(pgrTypeId, out prgTypeTb))
                        pROGRAMTB.PRGMTYPETB = prgTypeTb;
                }
                ////Para el PRGMSUBTYPETB
                if (pROGRAMTB.SUBTYPEID.HasValue)
                {
                    var subTypeId = pROGRAMTB.SUBTYPEID.Value;
                    PRGMSUBTYPETB subTypeTb = null;
                    if (avlPRGMSUBTYPETBs.TryGetValue(subTypeId, out subTypeTb))
                        pROGRAMTB.PRGMSUBTYPETB = subTypeTb;
                }
            }

            //// Actualizando la pagina actual.
            session.CurrentPage = fetchPage;

            //// En caso de que se trate de una operacion de Next y este parado en la ultima pagina
            //// se tiene que actualizar el conjunto de paginas por
            //// las que ha pasado la session. 
            if (pageOperation == PageOperation.Next && fetchPage == previousPages.Length-1)
            {
                //// Obteniendo el ultimo identificador de
                //// del conjunto de PROGRAMTB que se obtuvo
                //// para actualizar el proceso de paginacion. 
                var lastIdPROGRAMTB = result.Last().PRGMID;
                session.PreviousPages += $",{lastIdPROGRAMTB}";
            }

            _context.Update(session);
            _context.SaveChanges();


            return result;
        }

        /// <summary>
        /// Metodo que se va a utilizar para encontrar el
        /// identificador por el cual se debe realizar la
        /// busqueda en el metodo Fetch
        /// </summary>
        /// <returns></returns>
        private decimal GetLastId(int currentPage, string[] previousPages, PageOperation pageOperation, out int fetchPage)
        {
            //// En esta operacion no se va a tener en cuenta los valores
            //// "LastVisitedPage" ni "LastPageOfResults" 
            fetchPage = 0;
            var tempFetchPage = currentPage + int.Parse(pageOperation.ToString("d"));
            switch (pageOperation)
            {
                case PageOperation.First:
                    fetchPage = 0;
                    break;
                case PageOperation.Previous:
                    fetchPage = Math.Max(0, tempFetchPage);
                    break;
                case PageOperation.Next:
                    fetchPage = Math.Min(tempFetchPage, previousPages.Length - 1);
                    break;
            }

            return decimal.Parse(previousPages[fetchPage]);
        }

        /// <summary>
        /// Metodo que se va a encargar de generar un conjunto
        /// de clausulas sql enlazadas por OR.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private string CreateOrStatements(IEnumerable<string> values, string fieldName, string operatorForVlaues = "=", string operatorLink = "OR")
        {
            return string.Join($" {operatorLink} ", values.Select(v => $"{fieldName} {operatorForVlaues} {v}"));
        }

        /// <summary>
        /// Metodo que se va a encargar de encontrar un conjunto de
        /// PRGMTYPETB dados por sus identificadores.
        /// </summary>
        /// <param name="idPrgTypeTbList"></param>
        /// <returns></returns>
        private AVLTree<decimal, PRGMTYPETB> GetPRGMTYPETBFromDB(List<decimal?> idPrgTypeTbList)
        {
            var avl = new AVLTree<decimal, PRGMTYPETB>();
            using (var conn =
                new OracleConnection(_iConfig.GetConnectionString("OracleConnection")))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT PRGMTYPEID,PRGMTYPENAME,PRGMTYPEDESCP FROM PRGMTYPETB WHERE  {CreateOrStatements(idPrgTypeTbList.Select(id => id.ToString()), "PRGMTYPEID")} ";
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var prgTypeTb = CreatePRGMTYPETBFromReader(reader);
                        avl.Add(prgTypeTb.PRGMTYPEID, prgTypeTb);
                    }
                }
            }

            return avl;
        }

        /// <summary>
        /// Metodo que se va a encargar de encontrar un conjunto de
        /// PRGMSUBTYPE dados por sus identificadores.
        /// </summary>
        /// <param name="idPrgSubTypeTbList"></param>
        /// <returns></returns>
        private AVLTree<decimal, PRGMSUBTYPETB> GetPRGMSUBTYPETBFromDB(List<decimal?> idPrgSubTypeTbList)
        {
            var avl = new AVLTree<decimal, PRGMSUBTYPETB>();
            using (var conn =
                new OracleConnection(_iConfig.GetConnectionString("OracleConnection")))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    $"SELECT SUBTYPEID,SUBTYPENAME FROM PRGMSUBTYPETB WHERE {CreateOrStatements(idPrgSubTypeTbList.Select(id => id.ToString()), "SUBTYPEID")}";
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var prgSubTypeTb = CreatePRGMSUBTYPETBFromReader(reader);
                        avl.Add(prgSubTypeTb.SUBTYPEID, prgSubTypeTb);
                    }
                }
            }

            return avl;
        }


        /// <summary>
        /// Metodo que se va a encargar de crear la instancia de
        /// PROGRAMTB extraida de la base de datos.
        /// Aqui se asume que el reader solo contega los datos
        /// de la entidad PROGRAMTB.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private PROGRAMTB CreateFromReader(OracleDataReader reader, out decimal? idPgrTypeTb, out decimal? idPgrSubTypeTb)
        {
            idPgrTypeTb = -1;
            idPgrSubTypeTb = -1;

            var result = new PROGRAMTB
            {
                PRGMID = (long)reader.GetDecimal(0),
                PRGMNAME = reader.GetString(1),
                ////////FK a PRGMTYPETB
                PRGMTYPEID = (long?)(reader.IsDBNull(2) ? default(decimal?) : reader.GetDecimal(2)),
                ////VIDEOFILENAME = reader.GetString(3),
                DIRECTORY = reader.GetString(4),
                DURATIONMS = reader.IsDBNull(5) ? default(decimal?) : reader.GetDecimal(5),
                CREATOR = reader.GetString(6),
                CREATETIME = reader.IsDBNull(7) ? default(DateTime?) : reader.GetDateTime(7),
                EDITOR = reader.GetString(8),
                EDITTIME = reader.IsDBNull(9) ? default(DateTime?) : reader.GetDateTime(9),
                ////////FK a PRGMSUBTYPETB
                SUBTYPEID = (long?)(reader.IsDBNull(10) ? default(decimal?) : reader.GetDecimal(10)),
                BITRATE = reader.IsDBNull(11) ? default(decimal?) : reader.GetDecimal(11),
                FILESIZE = reader.IsDBNull(12) ? default(float?) : reader.GetFloat(12),
                LOCKED = reader.IsDBNull(13) ? default(decimal?) : reader.GetDecimal(13),
                CHECKOR = reader.GetString(14),
                CHECKTIME = reader.IsDBNull(15) ? default(DateTime?) : reader.GetDateTime(15),
                CHECKUP = reader.IsDBNull(16) ? default(decimal?) : reader.GetDecimal(16),
            };

            idPgrTypeTb = result.PRGMTYPEID;
            idPgrSubTypeTb = result.SUBTYPEID;

            return result;
        }

        /// <summary>
        /// Metodo que se va a encargar de construir un PRGMTYPETB
        /// a partir de un database reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private PRGMSUBTYPETB CreatePRGMSUBTYPETBFromReader(OracleDataReader reader)
        {
            return new PRGMSUBTYPETB
            {
                SUBTYPEID = (long)reader.GetDecimal(0),
                SUBTYPENAME = !reader.IsDBNull(1) ? reader.GetString(1) : null,
            };
        }

        /// <summary>
        /// Metodo que se va a encargar de construir un PRGMTYPETB
        /// a partir de un database reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private PRGMTYPETB CreatePRGMTYPETBFromReader(OracleDataReader reader)
        {
            return new PRGMTYPETB
            {
                PRGMTYPEID = (long)reader.GetDecimal(0),
                PRGMTYPENAME = reader.GetString(1),
                PRGMTYPEDESCP = !reader.IsDBNull(2) ? reader.GetString(2) : null,
            };
        }

        /// <summary>
        /// Metodo que se va a encargar de extraer los datos
        /// de una tabla. Este metodo es el que se va a utilizar
        /// para buscar los datos de los combobox del buscador.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDataFrom")]
        public IEnumerable<string> GetDataFrom(string tempTable, string q=null)
        {
            //var result = new List<string>() {"algo", "algo1", "otra cosa"};
            var result = new List<string>();
            using (var conn =
                new OracleConnection(_iConfig.GetConnectionString("OracleConnection")))
            {
                var cmd = conn.CreateCommand();

                //// Verificando el caso de que se trate de un query para toda la tabla
                //// o para un conjunto especificos de valores.  
                var bodyWhere = !string.IsNullOrEmpty(q)
                    ? $"WHERE LOWER(VALUE) LIKE '%{q.ToLower()}%'"
                    : string.Empty;

                cmd.CommandText =
                    $"SELECT VALUE FROM {tempTable} {bodyWhere}";
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var values = reader.GetString(0);
                        result.Add(values);
                    }
                }
            }
            return result;
        }

    }
}
