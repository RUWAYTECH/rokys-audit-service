namespace Rokys.Audit.Common.Helpers
{
    public static class SortOrderHelper
    {
        /// <summary>
        /// Obtiene el siguiente valor de SortOrder para un grupo específico.
        /// </summary>
        /// <param name="existingSortOrders">Lista de sortOrders existentes para el grupo.</param>
        /// <returns>El siguiente sortOrder (máximo + 1, o 1 si no hay ninguno).</returns>
        public static int GetNextSortOrder(IEnumerable<int> existingSortOrders)
        {
            return existingSortOrders.Any() ? existingSortOrders.Max() + 1 : 1;
        }
    }
}
