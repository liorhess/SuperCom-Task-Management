using SuperComData.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperComTests.UnitTests
{
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
}
