namespace Rokys.Audit.Common.Constant
{
   
    public sealed record SystemConfigRelation(string Code)
    {        
        // Tipo de referencia: 'EnterpriseGrouping', 'Enterprise', 'Store', etc.
        public static readonly SystemConfigRelation EnterpriseGrouping = new("EnterpriseGrouping");
    }

}
