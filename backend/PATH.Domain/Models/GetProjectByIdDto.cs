using PATH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PATH.Domain.Models
{
    public class GetProjectByIdDto
    {
        public Guid Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid CreatedById { get; set; }

        public string CreatorName { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid OrganizationId { get; set; }

        public OrganizationRole CurrentUserRole { get; set; }

        public ICollection<ProjectMemberBasicInfo> Members
        { get; set; }

        public ICollection<GetTaskItemResponse> Tasks { get; set; }
    }
}
