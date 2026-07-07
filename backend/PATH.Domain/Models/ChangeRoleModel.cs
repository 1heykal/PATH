using PATH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PATH.Domain.Models
{
    public class ChangeRoleModel
    {
        public Guid OrgId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public OrganizationRole NewRole { get; set; }
    }
}
