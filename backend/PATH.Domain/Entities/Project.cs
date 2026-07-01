using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PATH.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = "Untitled";

        public string Description { get; set; } = string.Empty;

        public ApplicationUser CreatedBy { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public Guid CreatedById { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ProjectMember> Members { get; set; } = new HashSet<ProjectMember>();

        public ICollection<TaskItem> Tasks { get; set; } = new HashSet<TaskItem>();
    }
}
