using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokys.Audit.Model.Tables
{
    public class RiskCommonEntity: AuditEntity
    {
        public decimal ObjectiveValue { get; set; }
        public decimal RiskLow { get; set; }
        public decimal RiskModerate { get; set; }
        public decimal RiskHigh { get; set; }
        public decimal RiskCritical { get; set; }
        public decimal Weighting { get; set; }
    }
}
