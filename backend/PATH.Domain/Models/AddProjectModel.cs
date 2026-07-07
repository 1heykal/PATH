using System;
using System.Collections.Generic;
using System.Text;

namespace PATH.Domain.Models
{
    public class AddProjectModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid OrganizationId { get; set; }
    }
}
