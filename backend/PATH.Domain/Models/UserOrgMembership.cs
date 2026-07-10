using PATH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PATH.Domain.Models
{
    public class UserOrgMembership
    {
        public Guid OrganizationId { get; set; }
        public Guid UserId { get; set; }
        public OrganizationRole Role { get; set; }
        public UserOrgPermissions Permissions { get; set; }

    }
}
