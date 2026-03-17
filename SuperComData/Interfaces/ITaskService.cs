using SuperComData.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperComData.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItemDto>> GetAllTasksAsync();
        Task<TaskItemDto?> GetTaskByIdAsync(int id);
        Task<TaskItemDto?> UpdateTaskAsync(int id, TaskItemDto taskItemDto);
        Task<TaskItemDto> CreateTaskAsync(TaskItemDto taskItemDto);
        Task<bool> DeleteTaskAsync(int id);
    }
}
