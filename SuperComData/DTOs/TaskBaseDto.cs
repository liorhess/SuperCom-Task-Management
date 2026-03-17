using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperComData.DTOs
{
    public abstract class TaskBaseDto
    {
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
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full Name must contain only letters and spaces")]
        public string UserFullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telephone number is required")]
        [RegularExpression(@"^[0-9]{9,12}$", ErrorMessage = "Telephone must be between 9 and 12 digits")]
        public string UserTelephone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string UserEmail { get; set; } = string.Empty;

        private List<string> _tagNames = new List<string>();

        public List<string> TagNames
        {
            get => _tagNames;
            set => _tagNames = value ?? new List<string>();
        }
    }
}
