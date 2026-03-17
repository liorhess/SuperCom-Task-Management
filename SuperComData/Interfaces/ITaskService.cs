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
        Task<IEnumerable<TaskReadDto>> GetAllTasksAsync();
        Task<TaskReadDto?> GetTaskByIdAsync(int id);
        Task<TaskReadDto?> UpdateTaskAsync(int id, TaskCreateDto taskItemDto);
        Task<TaskReadDto> CreateTaskAsync(TaskCreateDto taskItemDto);
        Task<bool> DeleteTaskAsync(int id);
    }
}
