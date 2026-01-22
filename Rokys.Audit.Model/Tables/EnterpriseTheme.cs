using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokys.Audit.Model.Tables
{
    public class EnterpriseTheme : AuditEntity
    {
        public Guid EnterpriseThemeId { get; set; }
        public Guid EnterpriseId { get; set; }
        public Enterprise Enterprise { get; set; }

        // Logo
        public byte[]? LogoData { get; set; }
        public string? LogoContentType { get; set; }
        public string? LogoFileName { get; set; }

        // Colores
        public string PrimaryColor { get; set; } = "#0066CC";
        public string SecondaryColor { get; set; } = "#333333";
        public string? AccentColor { get; set; }
        public string? BackgroundColor { get; set; }
        public string? TextColor { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
