using TaskManagement.API.DTOs;

namespace TaskManagement.API.Services
{
    public interface ITaskCommentService
    {
        Task<TaskCommentDto> CreateAsync(Guid taskId, Guid userId, CreateTaskCommentDto dto);
        Task<IEnumerable<TaskCommentDto>> GetByTaskIdAsync(Guid taskId);
    }
}