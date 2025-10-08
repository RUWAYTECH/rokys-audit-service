namespace Rokys.Audit.Model.Tables
{
    /// <summary>
    /// Entidad que relaciona empleados con tiendas asignadas
    /// </summary>
    public class EmployeeStore
    {
        /// <summary>
        /// ID de la relación empleado-tienda
        /// </summary>
        public Guid EmployeeStoreId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// ID de referencia del usuario
        /// </summary>
        public Guid UserReferenceId { get; set; }

        /// <summary>
        /// ID de la tienda
        /// </summary>
        public Guid StoreId { get; set; }

        /// <summary>
        /// Fecha de asignación
        /// </summary>
        public DateTime AssignmentDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Indica si la asignación está activa
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Usuario que creó el registro
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Usuario que actualizó el registro
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        // Navegación
        public virtual UserReference? UserReference { get; set; }
        public virtual Stores? Store { get; set; }
    }
}