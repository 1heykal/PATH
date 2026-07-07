using PATH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PATH.Domain.Models
{
    public class GetOrganizationByIdModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatorName { get; set; }
        public OrganizationRole CurrentUserRole { get; set; }

        public ICollection<GetOrganizationMembers> Members { get; set; } = new HashSet<GetOrganizationMembers>();

        public ICollection<ProjectBasicInfo> Projects { get; set; } = new HashSet<ProjectBasicInfo>();
    }
}
