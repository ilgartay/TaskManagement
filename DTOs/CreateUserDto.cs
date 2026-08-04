using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs
{
    public class CreateUserDto
    {
        [Required, StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }
}
