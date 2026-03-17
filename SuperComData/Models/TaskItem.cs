using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperComData.Models
{
    public class TaskItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        [Range(1, 5)]
        public int Priority { get; set; }

        [Required]
        public string UserFullName { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string UserTelephone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string UserEmail { get; set; } = string.Empty;

        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

        public bool IsOverdueProcessed { get; set; } = false;

    }
}
