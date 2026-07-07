using System;
using System.Collections.Generic;
using System.Text;

namespace PATH.Domain.Models
{
    public class ProjectMemberBasicInfo
    {
        public Guid ProjectId { get; set; }
        public string MemberName { get; set; }
        public Guid MemberId { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
