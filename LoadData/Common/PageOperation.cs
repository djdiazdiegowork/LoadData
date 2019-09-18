namespace Common
{
    /// <summary>
    /// Enum que se va a utilizar para clasificar
    /// las operaciones de cargado de bloques de
    /// datos orientado a paginacion.
    /// </summary>
    public enum PageOperation
    {
        Previous = -1,
        First,
        Next = 1,
        LastVisitedPage,
        LastPageOfResults
    }
}
