using Microsoft.EntityFrameworkCore;
using SuperComData.Context;
using SuperComData.DTOs;
using SuperComData.Models;
using SuperComData.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperComTests.UnitTests
{
    public class TaskServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly TaskService _service;

        public TaskServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _service = new TaskService(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private static TaskCreateDto CreateValidDto(string title = "Test Task", int priority = 3)
        {
            return new TaskCreateDto
            {
                Title = title,
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(7),
                Priority = priority,
                UserFullName = "John Doe",
                UserEmail = "john@example.com",
                UserTelephone = "123456789",
                TagNames = new List<string> { "urgent", "backend" }
            };
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldReturnCreatedTask()
        {
            var dto = CreateValidDto();

            var result = await _service.CreateTaskAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("Test Task", result.Title);
            Assert.Equal("Test Description", result.Description);
            Assert.Equal(3, result.Priority);
            Assert.Equal("John Doe", result.UserFullName);
            Assert.Equal("john@example.com", result.UserEmail);
            Assert.Contains("urgent", result.TagNames);
            Assert.Contains("backend", result.TagNames);
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldPersistToDatabase()
        {
            var dto = CreateValidDto();

            await _service.CreateTaskAsync(dto);

            Assert.Equal(1, await _context.Tasks.CountAsync());
            Assert.Equal(2, await _context.Tags.CountAsync());
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldReuseExistingTags()
        {
            _context.Tags.Add(new Tag { Name = "urgent" });
            await _context.SaveChangesAsync();

            var dto = CreateValidDto();
            await _service.CreateTaskAsync(dto);

            Assert.Equal(2, await _context.Tags.CountAsync());
        }

        [Fact]
        public async Task GetAllTasksAsync_ShouldReturnAllTasks()
        {
            await _service.CreateTaskAsync(CreateValidDto("Task 1"));
            await _service.CreateTaskAsync(CreateValidDto("Task 2"));

            var result = await _service.GetAllTasksAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllTasksAsync_ShouldReturnEmptyWhenNoTasks()
        {
            var result = await _service.GetAllTasksAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ShouldReturnTask_WhenExists()
        {
            var created = await _service.CreateTaskAsync(CreateValidDto("Find Me"));

            var result = await _service.GetTaskByIdAsync(created.Id);

            Assert.NotNull(result);
            Assert.Equal("Find Me", result.Title);
            Assert.Equal(created.Id, result.Id);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _service.GetTaskByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldUpdateFields()
        {
            var created = await _service.CreateTaskAsync(CreateValidDto("Original"));

            var updateDto = CreateValidDto("Updated Title");
            updateDto.Description = "Updated Description";
            updateDto.Priority = 5;
            updateDto.TagNames = new List<string> { "newTag" };

            var result = await _service.UpdateTaskAsync(created.Id, updateDto);

            Assert.NotNull(result);
            Assert.Equal("Updated Title", result!.Title);
            Assert.Equal("Updated Description", result.Description);
            Assert.Equal(5, result.Priority);
            Assert.Single(result.TagNames);
            Assert.Contains("newTag", result.TagNames);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _service.UpdateTaskAsync(999, CreateValidDto());

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldReturnTrue_WhenExists()
        {
            var created = await _service.CreateTaskAsync(CreateValidDto());

            var result = await _service.DeleteTaskAsync(created.Id);

            Assert.True(result);
            Assert.Equal(0, await _context.Tasks.CountAsync());
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldReturnFalse_WhenNotExists()
        {
            var result = await _service.DeleteTaskAsync(999);

            Assert.False(result);
        }
    }
}
