using PATH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PATH.Domain.Models
{
    public class UpdateTaskStatusModel
    {
        public Status NewStatus { get; set; }
    }
}
