using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rokys.Audit.Common.Constant;

namespace Rokys.Audit.DTOs.Common
{
    public class AuditPeriodPreEvaluationDto
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }
}