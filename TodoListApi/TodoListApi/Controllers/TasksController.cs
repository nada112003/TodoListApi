using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApi.Data;
using TodoListApi.Models;

namespace TodoListApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all tasks
        /// </summary>
        /// <returns>List of all tasks</returns>
        /// <response code="200">Returns the list of tasks</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            return await _context.Tasks.ToListAsync();
        }

        /// <summary>
        /// Create a new task
        /// </summary>
        /// <param name="taskItem">The task data</param>
        /// <returns>The created task</returns>
        /// <response code="201">Returns the newly created task</response>
        /// <response code="400">If the task data is invalid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TaskItem>> CreateTask(TaskItem taskItem)
        {
            _context.Tasks.Add(taskItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTasks), new { id = taskItem.Id }, taskItem);
        }

        /// <summary>
        /// Update an existing task
        /// </summary>
        /// <param name="id">The task ID</param>
        /// <param name="taskItem">The updated task data</param>
        /// <returns>No content</returns>
        /// <response code="204">If the task was updated successfully</response>
        /// <response code="400">If the task data is invalid</response>
        /// <response code="404">If the task was not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTask(int id, TaskItem taskItem)
        {
            if (id != taskItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(taskItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        /// <param name="id">The task ID</param>
        /// <returns>No content</returns>
        /// <response code="204">If the task was deleted successfully</response>
        /// <response code="404">If the task was not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var taskItem = await _context.Tasks.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(taskItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Toggle task completion status
        /// </summary>
        /// <param name="id">The task ID</param>
        /// <returns>No content</returns>
        /// <response code="204">If the task status was toggled successfully</response>
        /// <response code="404">If the task was not found</response>
        [HttpPut("{id}/toggle")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ToggleTaskStatus(int id)
        {
            var taskItem = await _context.Tasks.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }

            taskItem.IsCompleted = !taskItem.IsCompleted;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
} 