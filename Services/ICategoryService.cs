using TaskManagement.API.DTOs;

namespace TaskManagement.API.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateAsync(Guid userId, CreateCategoryDto createCategoryDto);
        Task<IEnumerable<CategoryDto>> GetAllAsync(Guid userId);
        Task<CategoryDto?> GetByIdAsync(Guid id);
        Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteAsync(Guid id);
    }
}