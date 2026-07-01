using PATH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PATH.Domain.Models
{
    public class AddTaskModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public Status Status { get; set; } = Status.Todo;

        public Priority Priority { get; set; } = Priority.Low;

        public DateTime DueDate { get; set; }

        public Guid AssignedToId { get; set; }

        public Guid ProjectId { get; set; }
    }
}
