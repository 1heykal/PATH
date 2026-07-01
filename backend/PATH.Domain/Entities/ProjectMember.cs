using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PATH.Domain.Entities
{
    public class ProjectMember
    {
        public Guid Id { get; set; }

        public Project Project { get; set; }

        [ForeignKey(nameof(Project))]
        public Guid ProjectId { get; set; }

        public ApplicationUser User { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    }
}
