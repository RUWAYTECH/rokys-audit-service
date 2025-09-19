namespace Retail.CheckList.Common.Constant
{
    public class Constants
    {
        public const string SystemUser = "system.default";

 		public struct ClaimNames
        {
            public const string NameId = "userName";
            public const string Email = "correo";
            public const string ProfileName = "profileName";
            public const string ProfileId = "profileId";
            public const string ApplicationId = "applicationId";
            public const string TokenName = "tokenName";
            public const string VigenciaToken = "vigenciaToken";
            public const string EmployeeId = "EmployeeId";
            public const string Profiles = "profiles";
            public const string NombreCompleto = "nombreCompleto";
            public const string CodigoDivision = "codigoDivision";
        }

        public struct MailTemplate
        {
            public const string OPERACIONES_CONFIRMACION = @"\Template\Mail\Operaciones.Confirmacion.html";
        }
    }
}
