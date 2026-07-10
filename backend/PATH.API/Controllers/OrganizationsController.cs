using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PATH.Domain;
using PATH.Domain.Models;
using PATH.Infrastructure;
using System.Security.Claims;

namespace PATH.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrganizationsController : ControllerBase
    {
        private readonly OrganizationService _organizationService;

        public OrganizationsController(OrganizationService organizationService)
        {
            _organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));
        }

        [HttpPost]
        public async Task<ActionResult<CreateOrganizationResponse>> CreateOrganization(CreateOrganizationModel model)
        {
            var organization = await _organizationService.CreateOrganization(GetAuthorId(), model);
            return CreatedAtAction(nameof(CreateOrganization), new { id = organization.Id }, organization);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetOrganizationByIdModel>> GetOrganizationById(Guid id)
        {
            var organization = await _organizationService.GetOrganizationById(GetAuthorId(), id);
            return Ok(organization);
        }

        [HttpGet]
        public async Task<ActionResult<List<OrganizationBasicInfo>>> GetAllOrganizations()
        {
            var organizations = await _organizationService.GetAllOrganizations(GetAuthorId());
            return Ok(organizations);
        }

        [HttpPost("members")]
        public async Task<ActionResult<AddMemberToOrganizationResponse>> AddMemberToOrganization(AddMemberToOrganizationModel model)
        {
            var response = await _organizationService.AddMemberToOrganization(GetAuthorId(), model);
            return CreatedAtAction(nameof(AddMemberToOrganization), new { id = response.Id }, response);
        }

        [HttpGet("{id}/members")]
        public async Task<ActionResult<List<OrganizationMemberBasicInfo>>> GetOrganizationMembers(Guid id)
        {
            var response = await _organizationService.GetOrganizationMembers(GetAuthorId(), id);
            return Ok(response);
        }

        [HttpDelete("{organizationId}/members/{userId}")]
        public async Task<ActionResult> RemoveMemberFromOrganization(Guid organizationId, Guid userId)
        {
            await _organizationService.RemoveMemberFromOrganization(GetAuthorId(), organizationId, userId);
            return NoContent();
        }

        [HttpGet("{id}/me")]
        public async Task<ActionResult<UserOrgMembership>> GetUserOrgMembership(Guid id)
        {
            var response = await _organizationService.GetUserOrgMembership(GetAuthorId(), id);
            return Ok(response);
        }

        private Guid GetAuthorId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);


    }
}
