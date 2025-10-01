namespace Rokys.Audit.Model.Tables
{
    public class Proveedor: AuditEntity
    {
        public Guid IdProveedor { get; set; }
        public string RUC { get; set; }
        public string RazonSocial { get; set; }
    }
}
