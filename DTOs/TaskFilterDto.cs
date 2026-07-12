using TaskManagement.API.Models;

namespace TaskManagement.API.DTOs
{
    public class TaskFilterDto
    {
        public Priority? Priority { get; set; }
        public TaskManagement.API.Models.TaskStatus? Status { get; set; }
        public Guid? CategoryId { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public string? SearchTerm { get; set; }
    }
}