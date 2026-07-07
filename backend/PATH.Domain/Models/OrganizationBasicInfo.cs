using PATH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PATH.Domain.Models
{
    public class OrganizationBasicInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
