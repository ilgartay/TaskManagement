using TaskManagement.API.DTOs;

namespace TaskManagement.API.Services
{
    public interface ITaskService
    {
        Task<TaskItemDto> CreateAsync(Guid userId, CreateTaskDto createTaskDto);
        Task<IEnumerable<TaskItemDto>> GetAllAsync(Guid userId);
        Task<TaskItemDto?> GetByIdAsync(Guid userId, Guid id);
        Task<TaskItemDto> UpdateAsync(Guid userId, Guid id, UpdateTaskDto updateTaskDto);
        Task<bool> DeleteAsync(Guid userId, Guid id);
        Task<IEnumerable<TaskItemDto>> GetFilteredAsync(Guid userId, TaskFilterDto filterDto);
        Task<TaskStatsDto> GetStatsAsync(Guid userId);
        Task<IEnumerable<TaskItemDto>> GetOverdueAsync(Guid userId);
    }
}
