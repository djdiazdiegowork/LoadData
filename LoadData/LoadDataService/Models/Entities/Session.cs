using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LoadDataService.Models.Entities
{
    /// <summary>
    /// Entidad que va a almacena una session
    /// con el cliente.
    /// </summary>
    [Table("Session")]
    public class Session
    {
        /// <summary>
        /// PK
        /// </summary>
        [Required]
        public int SessionId { get; set; }
        /// <summary>
        /// El identificador del usuario. Este es el
        /// valor por el cual se va a identificar un query
        /// del cliente.
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Query que se le va a realizar a la tabla
        /// A_EVENTO_ENTIDAD.
        /// </summary>
        public string StringQueryPMI { get; set; }
        
        /// <summary>
        /// La cantidad de datos que se van a devolver, es decir,
        /// la cantidad de eventos que se van a devolver durante
        /// cada extraccion.
        /// </summary>
        public int RowSize { get; set; }

        #region Para la paginacion
        /// <summary>
        /// Contiene los identificadores de los eventos
        /// que representan marcas de paginas separados
        /// por ,
        /// </summary>
        public string PreviousPages { get; set; }
        /// <summary>
        /// Indica la pagina actual en la que esta parado
        /// el cliente, si el valor es mayor que la cantidad
        /// de IDs que contiene "PreviousPages", entonces indica que es
        /// una pagina nueva, y por tanto hay que agregarla a "PreviousPages"
        /// </summary>
        public int CurrentPage { get; set; }
        #endregion

        /// <summary>
        /// La fecha que indica la ultima utilizacion
        /// de la session.
        /// </summary>
        public DateTime LastModification { get; set; }
    }
}
