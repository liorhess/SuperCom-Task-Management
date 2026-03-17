using SuperComData.DTOs;
using SuperComData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperComData.Mappers
{
    public static class TaskMapper
    {
        //Entity -> TaskCreateDto
        public static TaskCreateDto ToCreateDto(this TaskItem entity)
        {
            if (entity == null) return null!;

            return new TaskCreateDto
            {
                Title = entity.Title,
                Description = entity.Description,
                DueDate = entity.DueDate,
                Priority = entity.Priority,
                UserFullName = entity.UserFullName?? string.Empty,
                UserEmail = entity.UserEmail ?? string.Empty,
                UserTelephone = entity.UserTelephone?? string.Empty,
                TagNames = entity.Tags?.Select(t => t.Name).ToList() ?? new List<string>()
            };
        }

        //Entity -> TaskReadDto
        public static TaskReadDto ToReadDto(this TaskItem entity)
        {
            if (entity == null) return null!;

            return new TaskReadDto
            {
                Id = entity.Id,
                IsOverdueProcessed = entity.IsOverdueProcessed,
                Title = entity.Title,
                Description = entity.Description,
                DueDate = entity.DueDate,
                Priority = entity.Priority,
                UserFullName = entity.UserFullName ?? string.Empty,
                UserEmail = entity.UserEmail ?? string.Empty,
                UserTelephone = entity.UserTelephone ?? string.Empty,
                TagNames = entity.Tags?.Select(t => t.Name).ToList() ?? new List<string>()
            };
        }

        //TaskCreateDTO -> Entity
        public static TaskItem ToEntity(this TaskCreateDto dto)
        {
            if (dto == null) return null!;
            return new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Priority = dto.Priority,
                UserFullName = dto.UserFullName,
                UserEmail = dto.UserEmail,
                UserTelephone = dto.UserTelephone
            };
        }
    }
}
