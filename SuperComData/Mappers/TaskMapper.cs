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
        //Entity -> DTO
        public static TaskItemDto ToDto(this TaskItem entity)
        {
            if (entity == null) return null!;

            return new TaskItemDto
            {
                Id = entity.Id,
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

        //DTO -> Entity
        //we don't map Tags here because they require DB lookups in the Service
        public static TaskItem ToEntity(this TaskItemDto dto)
        {
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
