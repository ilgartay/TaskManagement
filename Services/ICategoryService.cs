using TaskManagement.API.DTOs;

namespace TaskManagement.API.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateAsync(Guid userId, CreateCategoryDto createCategoryDto);
        Task<IEnumerable<CategoryDto>> GetAllAsync(Guid userId);
        Task<CategoryDto?> GetByIdAsync(Guid userId, Guid id);
        Task<CategoryDto> UpdateAsync(Guid userId, Guid id, UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteAsync(Guid userId, Guid id);
    }
}
