using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Common;
using Common.PMI;
using Newtonsoft.Json;
using PMILoadDataClient.Utils;
using PMILoadDataClient.Utils.Enums;
using PageOperation = Common.PageOperation;

namespace PMILoadDataClient
{
    /// <summary>
    /// Clase que se va a utilizar como cliente del
    /// servicio de extraccion de datos implementado
    /// utilizando como tecnologia Oracle Text.
    /// </summary>
    public class LoadDataClient
    {
        /// <summary>
        /// Clase que se va a encargar de crear el cliente
        /// http que se va a utilizar para realizar los
        /// request al servidor de datos.
        /// </summary>
        private HttpClient _client;
        /// <summary>
        /// Objeto que representa una sesion en el servidor
        /// que se encarga de cargar los datos.
        /// </summary>
        private Session _session;
        /// <summary>
        /// Variable que contiene la cantidad de filas que van
        /// a ser retornadas durate cada extraccion.
        /// </summary>
        private int _rowSize;
        /// <summary>
        /// Variable que va a tener el conjunto de los atributos
        /// que van a ser utilizados para realizar el query.
        /// </summary>
        private List<AttributeBase> _attributesToProcess;
        /// <summary>
        /// Variable que va a tener el valor que indica si la
        /// extraccion se va a realizar por instancia o por
        /// instancia mes anno.
        /// </summary>
        private bool _traceByInstanceMonthYear;
        /// <summary>
        /// Clase que va a contener la configuracion para realizar
        /// el proceso de extraccion, a partir de los parametros
        /// que son pasados por el constructor.
        /// </summary>
        private Config _config;

        /// <summary>
        /// Diccionario que va a contener las direcciones de las
        /// apis para realizar la comunicacion con el servicio de
        /// extraccion de datos.
        /// </summary>
        private Dictionary<Urls, string> urls;

        /// <summary>
        /// Constructor para inicializar el estado
        /// de la instancia del objeto en cuestion.
        /// </summary>
        public LoadDataClient(string url)
        {
            //// Instanciando el HttpClient
            _client = new HttpClient();
            _client.BaseAddress = new Uri(url);

            var headerUrlLoadDataController = "/api/LoadData";
            urls = new Dictionary<Urls, string>();

            //***********************************Crear la session************************************************
            urls.Add(Urls.CreateSession, $"{headerUrlLoadDataController}/{"CreateSession"}");
            //***************************************************************************************************

            //***********************************Hacer el fetch***************
            urls.Add(Urls.Fetch, $"{headerUrlLoadDataController}/{"Fetch"}");
            //***************************************************************************************************

            //***********************************Rellenar los datos de los combobox***************
            urls.Add(Urls.GetDataFrom, $"{headerUrlLoadDataController}/{"GetDataFrom"}");
            //***************************************************************************************************
        }

        /// <summary>
        /// Constructor para el cliente que se encarga de consultar
        /// el servicio de extraccion de datos.
        /// </summary>
        /// <param name="url">la direccion donde se encuentra hosteado el servicio</param>
        /// <param name="rowsSize">la cantidad maxima de filas que se van a retornar
        /// durante cada proceso de extraccion.</param>
        /// <param name="attributesToProcess">el conjunto de atributos que se van a utilizar
        /// para realizar las consultas.</param>
        /// <param name="traceByInstanceMonthYear">el valor que indica si las extracciones
        /// se van a realizar por instancia o por instancia mes anno.</param>
        public LoadDataClient(string url, int rowsSize, List<AttributeBase> attributesToProcess,
            bool traceByInstanceMonthYear) : this(url)
        {
            _rowSize = rowsSize;
            _attributesToProcess = attributesToProcess;
            _traceByInstanceMonthYear = traceByInstanceMonthYear;

            _config = new Config(_rowSize, _attributesToProcess, _traceByInstanceMonthYear);
        }

        /// <summary>
        /// Metodo que se va a utilizar para iniciar el proceso de conexion
        /// con el servidor de datos.
        /// </summary>
        /// /// <param name="ignoreOldSession">Valor que indica si es requerido ignorar
        /// la session creada previamente en caso de que esta exista.</param>
        /// <returns></returns>
        public async Task Connect(string clientIdentifier, long expiration = 0)
        {
            _session = new Session(clientIdentifier, expiration);
            var sessionId = await CreateSession(_session.Expiration);
        }

        /// <summary>
        /// Metodo que se va a utilizar para crear una
        /// sesion en el servicio de extraccion de datos
        /// </summary>
        /// <returns></returns>
        private async Task<int> CreateSession(long expiration)
        {
            _config.Expiration = expiration;
            _config.ClientId = _session.SessionIndentifier;
            ////En caso de que no exista una session para este cliente
            ////se invoca el servicio de extraccion de datos para
            ////obtener uno.  
            string stringData = JsonConvert.SerializeObject(_config);
            var contentData = new StringContent
            (stringData, System.Text.Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync
                (urls[Urls.CreateSession], contentData);
            var responeResult = await response.Content.ReadAsStringAsync();

            int result = 0;
            if (int.TryParse(responeResult, out result))
                return result;
            return -1;
        }

        /// <summary>
        /// Metodo que se encarga de llamar al metodo
        /// del servicio que se encarga de crear el objeto
        /// que contiene la informacion de extraccion
        /// en el servicio de extraccion de datos.
        /// </summary> 
        /// <param name="pageOperation">el tipo de pagina que se esta solicitando</param>
        /// <returns></returns>
        public async Task<IEnumerable<PROGRAMTB>> FetchData(PageOperation pageOperation)
        {
            var getQueryParam =
                $"{urls[Urls.Fetch]}?systemIndentity={_session.SessionIndentifier}&pageOperation={pageOperation:d}";
            var response = await _client.GetAsync(getQueryParam, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<PROGRAMTB>>(data);
        }

        /// <summary>
        /// Metodo que se va a utilizar para obtener
        /// los distintos procesos.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetDataFrom(string tempTable, string q)
        {
            var getQueryParam = $"{urls[Urls.GetDataFrom]}?tempTable={tempTable}&q={q}";
            var response = await _client.GetAsync(getQueryParam, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<string>>(data);
        }
        
        /// <summary>
        /// Metodo que se va a encargar de finalizar la session.
        /// </summary>
        /// <returns></returns>
        public async Task Close()
        {
            
        }
    }
}
