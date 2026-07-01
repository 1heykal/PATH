using PATH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PATH.Domain.Models
{
    public class ChangeRoleModel
    {
        [Required]
        public Guid UserId { get; set; }

        [AllowedValues(values: ["admin", "user", "manager"], ErrorMessage = "Please provide a valid role."),]
        public string NewRole { get; set; }
    }
}
