using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokys.Audit.Model.Tables
{
    public class AuditEntity
    {
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }

        public void CreateAudit(string? userName)
        {
            CreatedBy = userName ?? "system.admin";
            CreationDate = DateTime.UtcNow;
            UpdateDate = DateTime.UtcNow;
            UpdatedBy = CreatedBy;
        }

        public void UpdateAudit(string? userName)
        {
            UpdateDate = DateTime.UtcNow;
            UpdatedBy = userName ?? "system.admin";
        }
    }
}
