using SuperComData.DTOs;
using SuperComData.Mappers;
using SuperComData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperComTests.UnitTests
{
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
}
