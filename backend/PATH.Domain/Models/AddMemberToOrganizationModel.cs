using System.ComponentModel.DataAnnotations;

namespace PATH.Domain
{
    public class AddMemberToOrganizationModel
    {
        [EmailAddress]
        public string UserEmail { get; set; }
        public Guid OrganizationId { get; set; }
    }
}