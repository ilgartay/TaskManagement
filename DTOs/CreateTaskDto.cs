using System.ComponentModel.DataAnnotations;
using TaskManagement.API.Models;

namespace TaskManagement.API.DTOs
{
    public class CreateTaskDto
    {
        [Required, StringLength(200)]
        [RegularExpression(@".*\S.*", ErrorMessage = "Başlık yalnızca boşluk içeremez.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [EnumDataType(typeof(Priority))]
        public Priority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
