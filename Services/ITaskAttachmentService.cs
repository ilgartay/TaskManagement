using TaskManagement.API.DTOs;

namespace TaskManagement.API.Services
{
    public interface ITaskAttachmentService
    {
        Task<TaskAttachmentDto> UploadAsync(Guid userId, Guid taskId, IFormFile file);
        Task<IEnumerable<TaskAttachmentDto>> GetByTaskIdAsync(Guid userId, Guid taskId);
        Task<(byte[] Content, string ContentType, string FileName)?> DownloadAsync(Guid userId, Guid attachmentId);
    }
}
