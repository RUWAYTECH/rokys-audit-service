namespace Rokys.Audit.Model.Tables
{
    public class Proveedor: AuditEntity
    {
        public Proveedor()
        {
           
        }
        public int IdProveedor { get; set; }
        public string RUC { get; set; }
        public string RazonSocial { get; set; }
    }
}
