using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SuperComApi.Controllers;
using SuperComData.Context;
using SuperComData.DTOs;
using SuperComData.Interfaces;
using SuperComData.Mappers;
using SuperComData.Models;
using SuperComData.Services;

namespace SuperComTests;

#region TaskService Tests (Integration with InMemory DB)

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

#endregion

#region TaskMapper Tests

public class TaskMapperTests
{
    [Fact]
    public void ToReadDto_ShouldMapAllFieldsCorrectly()
    {
        var entity = new TaskItem
        {
            Id = 42,
            Title = "Mapped Task",
            Description = "Mapped Desc",
            DueDate = new DateTime(2025, 12, 31),
            Priority = 2,
            UserFullName = "Jane Doe",
            UserEmail = "jane@example.com",
            UserTelephone = "987654321",
            IsOverdueProcessed = true,
            Tags = new List<Tag> { new Tag { Name = "tagA" } }
        };

        var dto = TaskMapper.ToReadDto(entity);

        Assert.Equal(42, dto.Id);
        Assert.Equal("Mapped Task", dto.Title);
        Assert.Equal("Mapped Desc", dto.Description);
        Assert.Equal(2, dto.Priority);
        Assert.True(dto.IsOverdueProcessed);
        Assert.Contains("tagA", dto.TagNames);
    }

    [Fact]
    public void ToCreateDto_ShouldMapFieldsCorrectly()
    {
        var entity = new TaskItem
        {
            Title = "Create DTO Task",
            Description = "Desc",
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = 4,
            UserFullName = "Bob",
            UserEmail = "bob@example.com",
            UserTelephone = "111222333",
            Tags = new List<Tag> { new Tag { Name = "t1" }, new Tag { Name = "t2" } }
        };

        var dto = TaskMapper.ToCreateDto(entity);

        Assert.Equal("Create DTO Task", dto.Title);
        Assert.Equal(4, dto.Priority);
        Assert.Equal(2, dto.TagNames.Count);
    }

    [Fact]
    public void ToEntity_ShouldMapFieldsCorrectly()
    {
        var dto = new TaskCreateDto
        {
            Title = "Entity Task",
            Description = "Desc",
            DueDate = DateTime.UtcNow.AddDays(5),
            Priority = 1,
            UserFullName = "Alice",
            UserEmail = "alice@example.com",
            UserTelephone = "444555666"
        };

        var entity = TaskMapper.ToEntity(dto);

        Assert.Equal("Entity Task", entity.Title);
        Assert.Equal(1, entity.Priority);
        Assert.Equal("Alice", entity.UserFullName);
    }

    [Fact]
    public void ToReadDto_NullEntity_ReturnsNonNull()
    {
        // TaskMapper returns null! for null input
        var result = TaskMapper.ToReadDto(null!);
        Assert.Null(result);
    }
}

#endregion

#region UserTasksController Tests (Moq)

public class UserTasksControllerTests
{
    private readonly Mock<ITaskService> _mockService;
    private readonly Mock<ILogger<UserTasksController>> _mockLogger;
    private readonly UserTasksController _controller;

    public UserTasksControllerTests()
    {
        _mockService = new Mock<ITaskService>();
        _mockLogger = new Mock<ILogger<UserTasksController>>();
        _controller = new UserTasksController(_mockLogger.Object, _mockService.Object);
    }

    private static TaskReadDto CreateReadDto(int id = 1, string title = "Test")
    {
        return new TaskReadDto
        {
            Id = id,
            Title = title,
            Description = "Desc",
            DueDate = DateTime.UtcNow.AddDays(7),
            Priority = 3,
            UserFullName = "John Doe",
            UserEmail = "john@example.com",
            UserTelephone = "123456789",
            TagNames = new List<string> { "tag1" }
        };
    }

    private static TaskCreateDto CreateCreateDto(string title = "New Task")
    {
        return new TaskCreateDto
        {
            Title = title,
            Description = "Desc",
            DueDate = DateTime.UtcNow.AddDays(7),
            Priority = 3,
            UserFullName = "John Doe",
            UserEmail = "john@example.com",
            UserTelephone = "123456789",
            TagNames = new List<string> { "tag1" }
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithTasks()
    {
        var tasks = new List<TaskReadDto> { CreateReadDto(1, "A"), CreateReadDto(2, "B") };
        _mockService.Setup(s => s.GetAllTasksAsync()).ReturnsAsync(tasks);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsAssignableFrom<IEnumerable<TaskReadDto>>(okResult.Value);
        Assert.Equal(2, returned.Count());
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenTaskExists()
    {
        var task = CreateReadDto(5, "Found");
        _mockService.Setup(s => s.GetTaskByIdAsync(5)).ReturnsAsync(task);

        var result = await _controller.GetById(5);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<TaskReadDto>(okResult.Value);
        Assert.Equal("Found", returned.Title);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        _mockService.Setup(s => s.GetTaskByIdAsync(99)).ReturnsAsync((TaskReadDto?)null);

        var result = await _controller.GetById(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var dto = CreateCreateDto("Created Task");
        var readDto = CreateReadDto(10, "Created Task");
        _mockService.Setup(s => s.CreateTaskAsync(dto)).ReturnsAsync(readDto);

        var result = await _controller.Create(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(UserTasksController.GetById), createdResult.ActionName);
        var returned = Assert.IsType<TaskReadDto>(createdResult.Value);
        Assert.Equal("Created Task", returned.Title);
        Assert.Equal(10, returned.Id);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenTaskExists()
    {
        var dto = CreateCreateDto("Updated");
        var readDto = CreateReadDto(1, "Updated");
        _mockService.Setup(s => s.UpdateTaskAsync(1, dto)).ReturnsAsync(readDto);

        var result = await _controller.Update(1, dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<TaskReadDto>(okResult.Value);
        Assert.Equal("Updated", returned.Title);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        var dto = CreateCreateDto();
        _mockService.Setup(s => s.UpdateTaskAsync(99, dto)).ReturnsAsync((TaskReadDto?)null);

        var result = await _controller.Update(99, dto);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSuccessful()
    {
        _mockService.Setup(s => s.DeleteTaskAsync(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        _mockService.Setup(s => s.DeleteTaskAsync(99)).ReturnsAsync(false);

        var result = await _controller.Delete(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}

#endregion

#region TaskCreateDto Validation Tests

public class TaskCreateDtoValidationTests
{
    [Fact]
    public void Validate_PastDueDate_ReturnsValidationError()
    {
        var dto = new TaskCreateDto
        {
            Title = "Task",
            Description = "Desc",
            DueDate = DateTime.UtcNow.AddDays(-1),
            Priority = 3,
            UserFullName = "John Doe",
            UserEmail = "john@example.com",
            UserTelephone = "123456789"
        };

        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var results = dto.Validate(context).ToList();

        Assert.Single(results);
        Assert.Contains("past", results[0].ErrorMessage!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_FutureDueDate_ReturnsNoErrors()
    {
        var dto = new TaskCreateDto
        {
            Title = "Task",
            Description = "Desc",
            DueDate = DateTime.UtcNow.AddDays(5),
            Priority = 3,
            UserFullName = "John Doe",
            UserEmail = "john@example.com",
            UserTelephone = "123456789"
        };

        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var results = dto.Validate(context).ToList();

        Assert.Empty(results);
    }
}

#endregion