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
     
        public decimal LowRisk { get; set; }

        public decimal ModerateRisk { get; set; }

        public decimal HighRisk { get; set; }
        public decimal Weighting { get; set; }
    }
}
