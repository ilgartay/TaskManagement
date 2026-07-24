using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.DTOs;
using TaskManagement.API.Extensions;
using TaskManagement.API.Services;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ITaskCommentService _commentService;
        private readonly ITaskAttachmentService _attachmentService;

        public TasksController(
            ITaskService taskService,
            ITaskCommentService commentService,
            ITaskAttachmentService attachmentService)
        {
            _taskService = taskService;
            _commentService = commentService;
            _attachmentService = attachmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _taskService.GetAllAsync(User.GetRequiredUserId());
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var task = await _taskService.GetByIdAsync(User.GetRequiredUserId(), id);

            if (task == null)
                return NotFound(new { message = "Görev bulunamadı." });

            return Ok(task);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFiltered([FromQuery] TaskFilterDto filterDto)
        {
            var tasks = await _taskService.GetFilteredAsync(User.GetRequiredUserId(), filterDto);
            return Ok(tasks);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _taskService.GetStatsAsync(User.GetRequiredUserId());
            return Ok(stats);
        }

        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdue()
        {
            var tasks = await _taskService.GetOverdueAsync(User.GetRequiredUserId());
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskDto createTaskDto)
        {
            var task = await _taskService.CreateAsync(User.GetRequiredUserId(), createTaskDto);
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateTaskDto updateTaskDto)
        {
            try
            {
                var task = await _taskService.UpdateAsync(User.GetRequiredUserId(), id, updateTaskDto);
                return Ok(task);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _taskService.DeleteAsync(User.GetRequiredUserId(), id);

            if (!deleted)
                return NotFound(new { message = "Görev bulunamadı." });

            return NoContent();
        }

        [HttpPost("{taskId}/comments")]
        public async Task<IActionResult> AddComment(Guid taskId, CreateTaskCommentDto dto)
        {
            var comment = await _commentService.CreateAsync(User.GetRequiredUserId(), taskId, dto);
            return Ok(comment);
        }

        [HttpGet("{taskId}/comments")]
        public async Task<IActionResult> GetComments(Guid taskId)
        {
            var comments = await _commentService.GetByTaskIdAsync(User.GetRequiredUserId(), taskId);
            return Ok(comments);
        }

        [HttpPost("{taskId}/attachments")]
        public async Task<IActionResult> UploadAttachment(Guid taskId, IFormFile file)
        {
            var attachment = await _attachmentService.UploadAsync(User.GetRequiredUserId(), taskId, file);
            return Ok(attachment);
        }

        [HttpGet("{taskId}/attachments")]
        public async Task<IActionResult> GetAttachments(Guid taskId)
        {
            var attachments = await _attachmentService.GetByTaskIdAsync(User.GetRequiredUserId(), taskId);
            return Ok(attachments);
        }

        [HttpGet("attachments/{attachmentId}/download")]
        public async Task<IActionResult> DownloadAttachment(Guid attachmentId)
        {
            var result = await _attachmentService.DownloadAsync(User.GetRequiredUserId(), attachmentId);

            if (result == null)
                return NotFound(new { message = "Dosya bulunamadı." });

            return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
        }
    }
}
