using System.Globalization;

namespace Rokys.Audit.Common.Helpers
{
    /// <summary>
    /// Utilidad para convertir valores de configuración según su tipo de dato
    /// </summary>
    public static class ConfigurationValueConverter
    {
       
        /// <summary>
        /// Convierte un valor de configuración a int según el tipo de dato especificado
        /// </summary>
        /// <param name="configValue">Valor de configuración a convertir</param>
        /// <param name="dataType">Tipo de dato del valor</param>
        /// <param name="defaultValue">Valor por defecto si la conversión falla (default: 0)</param>
        /// <returns>Valor convertido a int</returns>
        public static int ConvertToInt(string? configValue, string? dataType, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(configValue))
                return defaultValue;

            var normalizedType = dataType?.ToLower();

            return normalizedType switch
            {
                "int" or "integer" or "number" => int.TryParse(configValue, out int result) ? result : defaultValue,
                "decimal" or "float" => decimal.TryParse(configValue, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal decimalValue)
                    ? (int)Math.Round(decimalValue)
                    : defaultValue,
                _ => int.TryParse(configValue, out int defaultResult) ? defaultResult : defaultValue
            };
        }

        /// <summary>
        /// Convierte un valor de configuración a boolean según el tipo de dato especificado
        /// </summary>
        /// <param name="configValue">Valor de configuración a convertir</param>
        /// <param name="dataType">Tipo de dato del valor</param>
        /// <param name="defaultValue">Valor por defecto si la conversión falla (default: false)</param>
        /// <returns>Valor convertido a bool</returns>
        public static bool ConvertToBoolean(string? configValue, string? dataType, bool defaultValue = false)
        {
            if (string.IsNullOrWhiteSpace(configValue))
                return defaultValue;

            var normalizedType = dataType?.ToLower();

            return normalizedType switch
            {
                "boolean" or "bool" => ParseBoolean(configValue, defaultValue),
                _ => bool.TryParse(configValue, out bool result) ? result : defaultValue
            };
        }

        /// <summary>
        /// Método genérico para convertir valores numéricos según el tipo de dato especificado
        /// </summary>
        /// <typeparam name="T">Tipo numérico de destino (float, decimal, double, etc.)</typeparam>
        /// <param name="configValue">Valor de configuración a convertir</param>
        /// <param name="dataType">Tipo de dato del valor</param>
        /// <param name="defaultValue">Valor por defecto si la conversión falla</param>
        /// <param name="tryParseNumeric">Delegado para parsear el tipo numérico específico</param>
        /// <param name="tryParseInt">Delegado para parsear valores enteros</param>
        /// <returns>Valor convertido al tipo especificado</returns>
        public static T ConvertToNumeric<T>(
            string? configValue,
            string? dataType,
            T defaultValue,
            TryParseDelegate<T> tryParseNumeric,
            TryParseDelegate<int> tryParseInt) where T : struct
        {
            if (string.IsNullOrWhiteSpace(configValue))
                return defaultValue;

            var normalizedType = dataType?.ToLower();

            return normalizedType switch
            {
                "number" or "decimal" or "float" => tryParseNumeric(configValue, out T result) ? result : defaultValue,
                "int" or "integer" => tryParseInt(configValue, out int intValue)
                    ? ConvertToType<T>(intValue)
                    : defaultValue,
                _ => tryParseNumeric(configValue, out T defaultResult) ? defaultResult : defaultValue
            };
        }

        /// <summary>
        /// Convierte un valor int a un tipo genérico numérico
        /// </summary>
        /// <typeparam name="T">Tipo numérico de destino</typeparam>
        /// <param name="value">Valor entero a convertir</param>
        /// <returns>Valor convertido al tipo especificado</returns>
        public static T ConvertToType<T>(int value) where T : struct
        {
            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parsea valores booleanos comunes
        /// </summary>
        private static bool ParseBoolean(string value, bool defaultValue)
        {
            if (bool.TryParse(value, out bool result))
                return result;

            return value.ToLower() switch
            {
                "1" or "yes" or "si" or "true" => true,
                "0" or "no" or "false" => false,
                _ => defaultValue
            };
        }

        /// <typeparam name="T">Tipo del valor a parsear</typeparam>
        /// <param name="value">Cadena a parsear</param>
        /// <param name="result">Resultado del parseo</param>
        /// <returns>True si el parseo fue exitoso, false en caso contrario</returns>
        /// <summary>
        /// Delegado para métodos TryParse
        /// </summary>
        public delegate bool TryParseDelegate<T>(string value, out T result);
    }
}
