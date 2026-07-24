using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs;
using TaskManagement.API.Models;

namespace TaskManagement.API.Services
{
    public class TaskAttachmentService : ITaskAttachmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly string _uploadPath;

        public TaskAttachmentService(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _uploadPath = Path.Combine(env.ContentRootPath, "Uploads");

            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);
        }

        public async Task<TaskAttachmentDto> UploadAsync(Guid userId, Guid taskId, IFormFile file)
        {
            var taskExists = await _context.Tasks
                .AnyAsync(t => t.Id == taskId && t.UserId == userId);

            if (!taskExists)
                throw new KeyNotFoundException("Görev bulunamadı.");

            if (file.Length == 0)
                throw new InvalidOperationException("Dosya boş.");

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_uploadPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new TaskAttachment
            {
                TaskId = taskId,
                FileName = file.FileName,
                FilePath = filePath,
                FileSize = file.Length,
                ContentType = file.ContentType
            };

            _context.TaskAttachments.Add(attachment);
            await _context.SaveChangesAsync();

            return _mapper.Map<TaskAttachmentDto>(attachment);
        }

        public async Task<IEnumerable<TaskAttachmentDto>> GetByTaskIdAsync(Guid userId, Guid taskId)
        {
            var taskExists = await _context.Tasks
                .AnyAsync(t => t.Id == taskId && t.UserId == userId);

            if (!taskExists)
                throw new KeyNotFoundException("Görev bulunamadı.");

            var attachments = await _context.TaskAttachments
                .AsNoTracking()
                .Where(a => a.TaskId == taskId && a.Task.UserId == userId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TaskAttachmentDto>>(attachments);
        }

        public async Task<(byte[] Content, string ContentType, string FileName)?> DownloadAsync(
            Guid userId,
            Guid attachmentId)
        {
            var attachment = await _context.TaskAttachments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == attachmentId && a.Task.UserId == userId);

            if (attachment == null || !File.Exists(attachment.FilePath))
                return null;

            var content = await File.ReadAllBytesAsync(attachment.FilePath);
            return (content, attachment.ContentType, attachment.FileName);
        }
    }
}
