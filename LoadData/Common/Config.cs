using System.Collections.Generic;

namespace Common
{
    /// <summary>
    /// Clase que contiene la configuracion para realizar las consultas
    /// en servicio de extraccion de datos.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Indentificador del cliente de la aplicacion
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// La cantidad de milisegundos que son necesarios para
        /// que una session se vaya a eliminar
        /// </summary>
        public long Expiration { get; set; }
        
        /// <summary>
        /// Especifica el tamanno de bloque que va aser retornado por el
        /// servicio de extraccion de datos.
        /// </summary>
        public int RowSize { get; }
        /// <summary>
        /// Define los atributos que se van a utilizar para el proceso
        /// de consulta en el servicio de extraccion de datos.
        /// </summary>
        public List<AttributeBase> AttributesToProcess { get; }
        /// <summary>
        /// Indica si el proceso de extraccion va a ser realizado por
        /// instancia o por instancia mes anno.
        /// </summary>
        public bool TraceByInstanceMonthYear { get; }
        /// <summary>
        /// Constructor para la clase
        /// </summary>
        public Config(int rowSize, List<AttributeBase> attributesToProcess, bool traceByInstanceMonthYear)
        {
            RowSize = rowSize;
            AttributesToProcess = attributesToProcess;
            TraceByInstanceMonthYear = traceByInstanceMonthYear;
        }
    }
}
