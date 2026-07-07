using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PATH.Domain.Entities
{
    public class ApplicationUser
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string Username { get; set; } = string.Empty;

        public DateOnly BirthDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrganizationMember> OrganizationMemberships { get; set; } = new HashSet<OrganizationMember>();

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();

        public ICollection<Project> CreatedProjects { get; set; } = new HashSet<Project>();

        public ICollection<TaskItem> Tasks { get; set; } = new HashSet<TaskItem>();

        public ICollection<ProjectMember> ProjectMemberships { get; set; } = new HashSet<ProjectMember>();

    }
}
