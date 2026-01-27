namespace Rokys.Audit.DTOs.Requests.Group
{
    /// <summary>
    /// DTO para solicitar la clonación de un grupo y todas sus entidades hijas
    /// </summary>
    public class GroupCloneRequestDto
    {
        /// <summary>
        /// ID de la empresa donde se creará el grupo clonado
        /// </summary>
        public Guid? EnterpriseId { get; set; }

        /// <summary>
        /// ID del grupo a clonar
        /// </summary>
        public Guid GroupId { get; set; }

        public string Name { get; set; }

        public decimal Weighting { get; set; }
        public string Code { get; set; }
        public Guid? EnterpriseGroupingId { get; set; }
    }
}