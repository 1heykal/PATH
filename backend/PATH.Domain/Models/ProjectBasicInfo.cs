using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PATH.Domain.Models
{
    public class ProjectBasicInfo
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string CreatorName { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
