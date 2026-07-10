using System;
using System.Collections.Generic;
using System.Text;

namespace PATH.Domain.Models
{
    public class UserOrgPermissions
    {
        public bool CanCreateProject { get; set; }
        public bool CanAddMembers { get; set; }
        public bool CanAssignTasks { get; set; }
    }
}
