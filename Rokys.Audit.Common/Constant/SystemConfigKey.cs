namespace Rokys.Audit.Common.Constant
{
   
    public sealed record SystemConfigKey(string Code)
    {        
        // Códigos de claves de configuración del sistema
        public static readonly SystemConfigKey ScoreApplyToActionPlan = new("CONF001");
    }

}
