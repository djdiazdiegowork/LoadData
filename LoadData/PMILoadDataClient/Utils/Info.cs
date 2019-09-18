using System;

namespace PMILoadDataClient.Utils
{
    /// <summary>
    /// Clase que se va a utilizar para salvar
    /// la informacion del cliente, o sea, la
    /// informacion relacionada con la sesion en
    /// el servidor de datos.
    /// </summary>
    [Serializable]
    class Info
    {
        /// <summary>
        /// Propiedad que especifica el identificador de la
        /// sesion del cliente en el servidor de datos.
        /// </summary>
        public int SessionIdentifier { get; set; }
    }
}
