using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs;
using TaskManagement.API.Models;

namespace TaskManagement.API.Services
{
    public class TaskCommentService : ITaskCommentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TaskCommentService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TaskCommentDto> CreateAsync(Guid taskId, Guid userId, CreateTaskCommentDto dto)
        {
            var taskExists = await _context.Tasks.AnyAsync(t => t.Id == taskId);
            if (!taskExists)
                throw new KeyNotFoundException("Görev bulunamadı.");

            var comment = _mapper.Map<TaskComment>(dto);
            comment.TaskId = taskId;
            comment.UserId = userId;

            _context.TaskComments.Add(comment);
            await _context.SaveChangesAsync();

            await _context.Entry(comment).Reference(c => c.User).LoadAsync();

            return _mapper.Map<TaskCommentDto>(comment);
        }

        public async Task<IEnumerable<TaskCommentDto>> GetByTaskIdAsync(Guid taskId)
        {
            var comments = await _context.TaskComments
                .Include(c => c.User)
                .Where(c => c.TaskId == taskId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TaskCommentDto>>(comments);
        }
    }
}