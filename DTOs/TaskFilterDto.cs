using System.ComponentModel.DataAnnotations;
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

        [StringLength(200)]
        public string? SearchTerm { get; set; }

        [Range(1, 1_000_000, ErrorMessage = "Page değeri 1 ile 1000000 arasında olmalıdır.")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize değeri 1 ile 100 arasında olmalıdır.")]
        public int PageSize { get; set; } = 10;
    }
}
