using TaskManagement.API.DTOs;

namespace TaskManagement.API.Services
{
    public interface ITaskCommentService
    {
        Task<TaskCommentDto> CreateAsync(Guid userId, Guid taskId, CreateTaskCommentDto dto);
        Task<IEnumerable<TaskCommentDto>> GetByTaskIdAsync(Guid userId, Guid taskId);
    }
}
