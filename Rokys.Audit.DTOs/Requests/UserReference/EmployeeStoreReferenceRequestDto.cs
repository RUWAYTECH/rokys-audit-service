namespace Rokys.Audit.DTOs.Requests.UserReference
{
    public class EmployeeStoreReferenceRequestDto
    {
        public Guid StoreId { get; set; }
        public DateTime? AssignmentDate { get; set; }
    }
}
