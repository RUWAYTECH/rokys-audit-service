namespace Rokys.Audit.Common.Constant
{
    public class Constants
    {
        public const string SystemUser = "system.default";

        public struct ClaimNames
        {
            public const string UserId = "sub";
            public const string FirstName = "first_name";
            public const string LastName = "last_name";
            public const string NameId = "name";
            public const string Email = "email";
            public const string ProfileName = "profile_name";
            public const string ProfileId = "profileId";
            public const string ApplicationId = "applicationId";
            public const string TokenName = "tokenName";
            public const string VigenciaToken = "vigenciaToken";
            public const string EmployeeId = "employeeId";
            public const string Profiles = "profiles";
            public const string FullName = "fullName";
            public const string Position = "position";
        }

        public struct MailTemplate
        {
            public const string OPERACIONES_CONFIRMACION = @"\Template\Mail\Operaciones.Confirmacion.html";
        }
        public struct FileDirectories
        {
            public const string Uploads = "Uploads";
        }
    }
}
