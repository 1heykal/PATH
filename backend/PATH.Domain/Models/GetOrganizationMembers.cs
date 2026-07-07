using PATH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PATH.Domain.Models
{
    public class GetOrganizationMembers
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public Guid UserId { get; set; }

        public OrganizationRole Role { get; set; }

        public DateTime JoinedAt { get; set; }
    }
}
