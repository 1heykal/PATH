using System;
using System.Collections.Generic;
using System.Text;

namespace PATH.Domain.Models
{
    public class UserBasicInfo
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateOnly BirthDate { get; set; }

        public int TasksCount { get; set; } = 0;

        public int OrganizationsCount { get; set; } = 0;

        public int ProjectsCount { get; set; } = 0;

    }
}
