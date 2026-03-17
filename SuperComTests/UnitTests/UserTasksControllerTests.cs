using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SuperComApi.Controllers;
using SuperComData.DTOs;
using SuperComData.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperComTests.UnitTests
{
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
}
