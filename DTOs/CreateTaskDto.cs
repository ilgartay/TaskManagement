using TaskManagement.API.Models;

namespace TaskManagement.API.DTOs
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Priority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid? CategoryId { get; set; }
    }
}