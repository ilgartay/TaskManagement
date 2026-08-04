using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs
{
    public class UpdateCategoryDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required, StringLength(20)]
        [RegularExpression("^#[0-9a-fA-F]{6}$", ErrorMessage = "Renk #RRGGBB formatında olmalıdır.")]
        public string Color { get; set; } = "#007bff";
    }
}
