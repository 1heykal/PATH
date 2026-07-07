using PATH.Domain.Entities;

namespace PATH.Domain
{
    public class OrganizationMemberBasicInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public Guid UserId { get; set; }

        public OrganizationRole Role { get; set; }

        public DateTime JoinedAt { get; set; }
    }
}