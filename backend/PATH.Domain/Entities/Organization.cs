using PATH.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PATH.Domain.Entities
{
    public class Organization
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ApplicationUser CreatedBy { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public Guid CreatedById { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrganizationMember> Members { get; set; } = new HashSet<OrganizationMember>();

        public ICollection<Project> Projects { get; set; } = new HashSet<Project>();


    }
}
