using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs;
using TaskManagement.API.Models;

namespace TaskManagement.API.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TaskService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TaskItemDto> CreateAsync(Guid userId, CreateTaskDto createTaskDto)
        {
            var task = _mapper.Map<TaskItem>(createTaskDto);
            task.UserId = userId;

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return _mapper.Map<TaskItemDto>(task);
        }

        public async Task<IEnumerable<TaskItemDto>> GetAllAsync(Guid userId)
        {
            var tasks = await _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TaskItemDto>>(tasks);
        }

        public async Task<TaskItemDto?> GetByIdAsync(Guid id)
        {
            var task = await _context.Tasks
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);

            return task == null ? null : _mapper.Map<TaskItemDto>(task);
        }

        public async Task<TaskItemDto> UpdateAsync(Guid id, UpdateTaskDto updateTaskDto)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
                throw new KeyNotFoundException("Görev bulunamadı.");

            _mapper.Map(updateTaskDto, task);
            task.UpdatedAt = DateTime.UtcNow;

            if (task.Status == Models.TaskStatus.Completed && task.CompletedAt == null)
                task.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<TaskItemDto>(task);
        }
        public async Task<TaskStatsDto> GetStatsAsync(Guid userId)
{
    var tasks = _context.Tasks.Where(t => t.UserId == userId);

    return new TaskStatsDto
    {
        TotalTasks = await tasks.CountAsync(),
        PendingTasks = await tasks.CountAsync(t => t.Status == Models.TaskStatus.Pending),
        InProgressTasks = await tasks.CountAsync(t => t.Status == Models.TaskStatus.InProgress),
        CompletedTasks = await tasks.CountAsync(t => t.Status == Models.TaskStatus.Completed),
        CancelledTasks = await tasks.CountAsync(t => t.Status == Models.TaskStatus.Cancelled),
        OverdueTasks = await tasks.CountAsync(t =>
            t.DueDate != null &&
            t.DueDate < DateTime.UtcNow &&
            t.Status != Models.TaskStatus.Completed &&
            t.Status != Models.TaskStatus.Cancelled)
    };
}

public async Task<IEnumerable<TaskItemDto>> GetOverdueAsync(Guid userId)
{
    var tasks = await _context.Tasks
        .Include(t => t.Category)
        .Where(t => t.UserId == userId &&
                    t.DueDate != null &&
                    t.DueDate < DateTime.UtcNow &&
                    t.Status != Models.TaskStatus.Completed &&
                    t.Status != Models.TaskStatus.Cancelled)
        .OrderBy(t => t.DueDate)
        .ToListAsync();

    return _mapper.Map<IEnumerable<TaskItemDto>>(tasks);
}

        public async Task<bool> DeleteAsync(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
                return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<TaskItemDto>> GetFilteredAsync(Guid userId, TaskFilterDto filterDto)
        {
            var query = _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .AsQueryable();

            if (filterDto.Priority != null)
                query = query.Where(t => t.Priority == filterDto.Priority);

            if (filterDto.Status != null)
                query = query.Where(t => t.Status == filterDto.Status);

            if (filterDto.CategoryId != null)
                query = query.Where(t => t.CategoryId == filterDto.CategoryId);

            if (filterDto.DueDateFrom != null)
                query = query.Where(t => t.DueDate >= filterDto.DueDateFrom);

            if (filterDto.DueDateTo != null)
                query = query.Where(t => t.DueDate <= filterDto.DueDateTo);

            if (!string.IsNullOrEmpty(filterDto.SearchTerm))
                query = query.Where(t => t.Title.Contains(filterDto.SearchTerm) ||
                                         (t.Description != null && t.Description.Contains(filterDto.SearchTerm)));

            var tasks = await query
    .OrderByDescending(t => t.CreatedAt)
    .Skip((filterDto.Page - 1) * filterDto.PageSize)
    .Take(filterDto.PageSize)
    .ToListAsync();

return _mapper.Map<IEnumerable<TaskItemDto>>(tasks);
        }
    }
}