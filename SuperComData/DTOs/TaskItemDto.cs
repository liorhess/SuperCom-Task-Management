using System.ComponentModel.DataAnnotations;

namespace SuperComData.DTOs;

public class TaskItemDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Due date is required")]
    public DateTime DueDate { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Priority must be between 1 (Low) and 5 (High)")]
    public int Priority { get; set; }

    [Required(ErrorMessage = "User's full name is required")]
    public string UserFullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telephone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string UserTelephone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    public string UserEmail { get; set; } = string.Empty;

    public List<string> TagNames { get; set; } = new List<string>();
}