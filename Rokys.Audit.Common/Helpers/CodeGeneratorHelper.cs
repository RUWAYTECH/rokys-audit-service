namespace Rokys.Audit.Common.Helpers
{
    public static class CodeGeneratorHelper
    {
        /// <summary>
        /// Genera el siguiente código secuencial con prefijo, 
        /// permitiendo que el número crezca libremente si supera el padding.
        /// </summary>
        /// <param name="prefix">Prefijo del código (por ejemplo "TPL" o "ORD").</param>
        /// <param name="lastCode">Último código generado (por ejemplo "TPL-000123"), o null si no existe.</param>
        /// <param name="padding">Cantidad de ceros a la izquierda (mínimo). Por defecto: 6.</param>
        /// <returns>Nuevo código generado (por ejemplo "TPL-000124" o "TPL-1000000").</returns>
        public static string GenerateNextCode(string prefix, string? lastCode, int padding = 6)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentException("El prefijo no puede estar vacío.", nameof(prefix));

            int nextNumber = 1;

            if (!string.IsNullOrEmpty(lastCode))
            {
                var parts = lastCode.Split('-');

                if (parts.Length >= 2)
                {
                    // Obtiene la parte numérica (último segmento)
                    var numericPart = parts[^1];
                    if (int.TryParse(numericPart, out int current))
                        nextNumber = current + 1;
                }
            }

            // Si el número tiene más dígitos que el padding, se conserva completo (no se trunca)
            string numericFormatted = nextNumber.ToString($"D{padding}");

            return $"{prefix}-{numericFormatted}";
        }
    }
}
