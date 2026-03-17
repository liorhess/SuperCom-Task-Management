using Microsoft.EntityFrameworkCore;
using SuperComData.Context;
using SuperComData.Models;
using SuperComData.DTOs;
using SuperComData.Interfaces;
using SuperComData.Mappers;

namespace SuperComData.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;

    public TaskService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskReadDto>> GetAllTasksAsync()
    {
        return await _context.Tasks
            .Include(t => t.Tags)
            .Select(t => new TaskReadDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Priority = t.Priority,
                UserFullName = t.UserFullName,
                UserEmail = t.UserEmail,
                UserTelephone = t.UserTelephone,
                TagNames = t.Tags.Select(tg => tg.Name).ToList()
            }).ToListAsync();
    }

    public async Task<TaskReadDto?> GetTaskByIdAsync(int id)
    {
        var task = await _context.Tasks
            .Include(t => t.Tags)
            .Where(t => t.Id == id).FirstOrDefaultAsync();
        if (task != null)
        {
            return TaskMapper.ToReadDto(task);
        }
        return null;
    }

    public async Task<TaskReadDto?> UpdateTaskAsync(int id, TaskCreateDto task)
    {
        var existingTask = await _context.Tasks
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (existingTask == null) return null;


        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.DueDate = task.DueDate;
        existingTask.Priority = task.Priority;

        existingTask.UserFullName = task.UserFullName;
        existingTask.UserEmail = task.UserEmail;
        existingTask.UserTelephone = task.UserTelephone;

        existingTask.Tags.Clear();

        foreach (var tagName in task.TagNames)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(tg => tg.Name == tagName)
                      ?? new Tag { Name = tagName };

            existingTask.Tags.Add(tag);
        }

        await _context.SaveChangesAsync();
        return TaskMapper.ToReadDto(existingTask);
    }


    public async Task<TaskReadDto> CreateTaskAsync(TaskCreateDto taskItemDto)
    {
        var task = new TaskItem
        {
            Title = taskItemDto.Title,
            Description = taskItemDto.Description,
            DueDate = taskItemDto.DueDate,
            Priority = taskItemDto.Priority,
            UserEmail = taskItemDto.UserEmail,
            UserFullName = taskItemDto.UserFullName,
            UserTelephone = taskItemDto.UserTelephone
        };

        foreach (var tagName in taskItemDto.TagNames)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(x => x.Name == tagName)
                      ?? new Tag { Name = tagName };
            task.Tags.Add(tag);
        }

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return TaskMapper.ToReadDto(task);
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var task = await _context.Tasks.Where(t => t.Id == id).FirstOrDefaultAsync();
        if (task != null)
        {
            _context.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}