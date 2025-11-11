namespace Rokys.Audit.Common.Constant
{
   
    public sealed record RoleCodes(string Code)
    {        
        // Códigos de roles de auditoría
        public static readonly RoleCodes JefeDeArea = new("A001");
        public static readonly RoleCodes AsistenteAudit = new("A002");
        public static readonly RoleCodes JefeDeOperaciones = new("A003");
        public static readonly RoleCodes Volante = new("A004");
        public static readonly RoleCodes Auditor = new("A005");
        public static readonly RoleCodes JobSupervisor = new("A006");
    }

}
