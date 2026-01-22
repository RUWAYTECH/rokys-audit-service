using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rokys.Audit.Common.Helpers
{
    public static class GenerateColor
    {
         /// <summary>
        /// Genera un color único basado en el índice cuando se agotan los colores predefinidos
        /// </summary>
        public static string GenerateColorFromIndex(int index)
        {
            // Generar colores usando HSL para asegurar buena distribución
            var hue = (index * 137.508) % 360; // Usar ángulo dorado para buena distribución
            var saturation = 70 + (index % 30); // Saturación entre 70-100%
            var lightness = 45 + (index % 20);  // Luminosidad entre 45-65%
            
            // Convertir HSL a RGB y luego a HEX
            return HslToHex(hue, saturation, lightness);
        }

        /// <summary>
        /// Convierte valores HSL a formato hexadecimal
        /// </summary>
        private static string HslToHex(double h, double s, double l)
        {
            h /= 360;
            s /= 100;
            l /= 100;

            double r, g, b;

            if (s == 0)
            {
                r = g = b = l; // Gris
            }
            else
            {
                double HueToRgb(double p, double q, double t)
                {
                    if (t < 0) t += 1;
                    if (t > 1) t -= 1;
                    if (t < 1.0/6) return p + (q - p) * 6 * t;
                    if (t < 1.0/2) return q;
                    if (t < 2.0/3) return p + (q - p) * (2.0/3 - t) * 6;
                    return p;
                }

                var q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                var p = 2 * l - q;
                r = HueToRgb(p, q, h + 1.0/3);
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - 1.0/3);
            }

            var rInt = (int)Math.Round(r * 255);
            var gInt = (int)Math.Round(g * 255);
            var bInt = (int)Math.Round(b * 255);

            return $"#{rInt:X2}{gInt:X2}{bInt:X2}";
        }
    }
}