using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PMILoadDataClient.Utils
{
    /// <summary>
    /// Clase que se va a utilizar para representar una session
    /// en el servicio de extraccion de datos.
    /// </summary>
    class Session
    {
        /// <summary>
        /// La cantidad de milisegundos que son necesarios para realizar
        /// una expiracion en el servidor de datos.
        /// </summary>
        public long Expiration { get; set; }

        private string _sessionIndetifier;
        /// <summary>
        /// Valor que identifica la session en el servidor.
        /// </summary>
        public string SessionIndentifier
        {
            get { return _sessionIndetifier; }
            set
            {
                _sessionIndetifier = value;
            }
        }

        /// <summary>
        /// Constructor de una session.
        /// </summary>
        public Session(string clientIdentifier, long expiration)
        {
            SessionIndentifier = clientIdentifier;
            Expiration = expiration;
        }
        

        /// <summary>
        /// Metodo que se encarga de salvar los
        /// datos que conforman una identificacion.
        /// </summary>
        /// <param name="sessionIdentifier">valor que identifica la session</param>
        private void SaveIdentifier(int sessionIdentifier)
        {
            //// Creando la carpeta de trabajo en caso de que esta no exista.
            var infoDirectory = Path.Combine(Environment.CurrentDirectory, "info");
            if (!Directory.Exists(infoDirectory)) Directory.CreateDirectory(infoDirectory);

            var info = new Info {SessionIdentifier = sessionIdentifier};
            //// Serializar el objeto info
            var formatter = new BinaryFormatter();
            var fileStreamWriter = new FileStream(Path.Combine(infoDirectory, sessionIdentifier.ToString()),
                FileMode.Create, FileAccess.Write);
            formatter.Serialize(fileStreamWriter, info);
            fileStreamWriter.Close();
        }

        /// <summary>
        /// Metodo que se encarga de cargar el dato que identifica
        /// la session. Si no existe este dato, retorna -1;
        /// </summary>
        /// <returns></returns>
        private int LoadIdentifier()
        {
            //// Deserializar el objeto info salvado como un binario
            try
            {
                var infoDirectory = Path.Combine(Environment.CurrentDirectory, "info");
                var files = Directory.GetFiles(infoDirectory);
                if (files.Length > 0)
                {
                    var fileName = Directory.GetFiles(infoDirectory)[0];
                    var formatter = new BinaryFormatter();
                    FileStream fileStreamReader = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    var info = (Info) formatter.Deserialize(fileStreamReader);
                    fileStreamReader.Close();
                    return info.SessionIdentifier;
                }

                return -1;
            }
            catch (Exception e)
            {
                return -1;
            }
        }
    }
}
