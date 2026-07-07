namespace PATH.Domain
{
    public class AddMemberToOrganizationModel
    {
        public string UserEmail { get; set; }
        public Guid OrganizationId { get; set; }
    }
}