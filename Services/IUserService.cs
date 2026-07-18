using TaskManagement.API.DTOs;

namespace TaskManagement.API.Services
{
    public interface IUserService
    {
        Task<AuthResponseDto> RegisterAsync(CreateUserDto createUserDto);
Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<UserDto> GetByIdAsync(Guid id);
        Task<UserDto> UpdateAsync(Guid id, UpdateUserDto updateUserDto);
    }
}