using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperComData.DTOs
{
    public class TaskReadDto : TaskBaseDto
    {
        public int Id { get; set; }
        public bool IsOverdueProcessed { get; set; }
    }
}
