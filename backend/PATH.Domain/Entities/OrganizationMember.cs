using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PATH.Domain.Entities
{
    public class OrganizationMember
    {
        public Guid Id { get; set; }

        public Organization Organization { get; set; }

        [ForeignKey(nameof(Organization))]
        public Guid OrganizationId { get; set; }

        public ApplicationUser User { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public OrganizationRole Role { get; set; } = OrganizationRole.Member;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    }
}
