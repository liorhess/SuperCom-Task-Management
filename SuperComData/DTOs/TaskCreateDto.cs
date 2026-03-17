using System.ComponentModel.DataAnnotations;

namespace SuperComData.DTOs;

public class TaskCreateDto : TaskBaseDto, IValidatableObject 
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DueDate.Date < DateTime.UtcNow.Date)
        {
            yield return new ValidationResult(
                "Due Date cannot be in the past for new tasks.",
                new[] { nameof(DueDate) }
            );
        }
    }
}