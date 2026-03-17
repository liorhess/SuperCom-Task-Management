using Microsoft.AspNetCore.Mvc;
using SuperComData.DTOs;
using SuperComData.Interfaces;

namespace SuperComApi.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class UserTasksController : ControllerBase
    {
        private readonly ILogger<UserTasksController> _logger;
        private readonly ITaskService _taskService;

        public UserTasksController(ILogger<UserTasksController> logger, ITaskService taskService)
        {
            _logger = logger;
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tasks = await _taskService.GetTaskByIdAsync(id);
            if (tasks == null)
            {
                return NotFound($"Task with ID {id} was not found.");
            }
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaskCreateDto dto)
        {

            var createdTask = await _taskService.CreateTaskAsync(dto);

            //returns 201 for create
            return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskCreateDto taskItemTdo)
        {
            var task = await _taskService.UpdateTaskAsync(id, taskItemTdo);
            if (task == null)
            {
                return NotFound($"Task with ID {id} was not found.");
            }
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteSucceeded = await _taskService.DeleteTaskAsync(id);
            if (!deleteSucceeded)
            {
                return NotFound($"Task with ID {id} was not found.");
            }
            return NoContent();
        }
    }
}