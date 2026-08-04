using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs
{
    public class CreateTaskCommentDto
    {
        [Required, StringLength(2000)]
        [RegularExpression(@".*\S.*", ErrorMessage = "Yorum yalnızca boşluk içeremez.")]
        public string Comment { get; set; } = string.Empty;
    }
}
