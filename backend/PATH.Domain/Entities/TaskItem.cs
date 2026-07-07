using PATH.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PATH.Domain.Entities
{
    public class TaskItem
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public Status Status { get; set; } = Status.Todo;

        public Priority Priority { get; set; } = Priority.Low;

        public DateTime DueDate { get; set; }

        public ApplicationUser AssignedTo { get; set; }

        [ForeignKey(nameof(AssignedTo))]
        public Guid AssignedToId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Project Project { get; set; }

        [ForeignKey(nameof(Project))]
        public Guid ProjectId { get; set; }

        public TaskItem()
        {
            
        }


        public TaskItem(AddTaskModel model)
        {

            Title = model.Title;
            Description = model.Description;
            Status = model.Status;
            Priority = model.Priority;
            DueDate = model.DueDate;
            AssignedToId = model.AssignedToId;
            ProjectId = model.ProjectId;
        }

    }
}
