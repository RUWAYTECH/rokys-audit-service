namespace Rokys.Audit.DTOs.Common
{
    public class EmployeeStoreDto
    {
        public Guid UserReferenceId { get; set; }
        public Guid StoreId { get; set; }
        public DateTime AssignmentDate { get; set; }
    }
}
