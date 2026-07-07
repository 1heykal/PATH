using PATH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PATH.Domain.Models
{
    public class AddMemberToOrganizationResponse
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }

        public string MemberName { get; set; }

        public string OrganizationName { get; set; }

        public string UserEmail { get; set; }

        public Guid UserId { get; set; }

        public OrganizationRole Role { get; set; }

        public DateTime JoinedAt { get; set; }
    }
}
