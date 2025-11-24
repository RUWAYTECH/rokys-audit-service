namespace Rokys.Audit.DTOs.Responses.Group
{
    /// <summary>
    /// DTO de respuesta para la operación de clonación de grupo
    /// </summary>
    public class GroupCloneResponseDto
    {
        /// <summary>
        /// ID del grupo original que fue clonado
        /// </summary>
        public Guid OriginalGroupId { get; set; }

        /// <summary>
        /// ID del nuevo grupo clonado
        /// </summary>
        public Guid ClonedGroupId { get; set; }

        /// <summary>
        /// ID de la empresa donde se creó el grupo clonado
        /// </summary>
        public Guid EnterpriseId { get; set; }

        /// <summary>
        /// Nombre del grupo clonado
        /// </summary>
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad de entidades ScaleGroup clonadas
        /// </summary>
        public int ScaleGroupsCloned { get; set; }

        /// <summary>
        /// Cantidad de entidades TableScaleTemplate clonadas
        /// </summary>
        public int TableScaleTemplatesCloned { get; set; }

        /// <summary>
        /// Cantidad de entidades AuditTemplateField clonadas
        /// </summary>
        public int AuditTemplateFieldsCloned { get; set; }

        /// <summary>
        /// Cantidad de entidades ScoringCriteria clonadas
        /// </summary>
        public int ScoringCriteriaCloned { get; set; }

        /// <summary>
        /// Cantidad de entidades CriteriaSubResult clonadas
        /// </summary>
        public int CriteriaSubResultsCloned { get; set; }

        /// <summary>
        /// Cantidad de archivos adjuntos clonados
        /// </summary>
        public int StorageFilesCloned { get; set; }

        /// <summary>
        /// Fecha y hora de la clonación
        /// </summary>
        public DateTime ClonedAt { get; set; }
    }
}