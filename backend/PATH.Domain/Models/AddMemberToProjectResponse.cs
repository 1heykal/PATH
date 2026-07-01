using PATH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PATH.Domain.Models
{
    public class AddMemberToProjectResponse
    {
        public Guid ProjectId { get; set; }

        public string UserName { get; set; }
        public Guid UserId { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public static AddMemberToProjectResponse FromEntity(ProjectMember projectMember)
        {
            return new AddMemberToProjectResponse
            {
                ProjectId = projectMember.ProjectId,
                UserId = projectMember.UserId,
                UserName = $"{projectMember.User.FirstName} {projectMember.User.LastName}",
                JoinedAt = projectMember.JoinedAt
            };
        }
    }
}
