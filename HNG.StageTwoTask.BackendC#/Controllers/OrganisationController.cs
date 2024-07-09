using HNG.StageTwoTask.BackendC_.Models.Organisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HNG.StageTwoTask.BackendC_.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class OrganisationController : ControllerBase
    {
        private readonly IOrganisationService _organisationService;

        public OrganisationController(IOrganisationService organisationService)
        {
            _organisationService = organisationService;
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserDetails(string id)
        {
            // Extract requestingUserId from the JWT token
            var requestingUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var response = await _organisationService.GetUserDetailsAsync(id, requestingUserId);

            if (response.StatusCode == 401)
            {
                return Unauthorized(response);
            }

            if (response.StatusCode == 404)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpGet("organisations")]
        public async Task<IActionResult> GetOrganisations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var response = await _organisationService.GetOrganisationsAsync(userId);

            if (response.StatusCode == 404)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpGet("organisations/{orgId}")]
        public async Task<IActionResult> GetOrganisationById(string orgId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var response = await _organisationService.GetOrganisationByIdAsync(userId, orgId);

            if (response.StatusCode == 404)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost("organisations")]
        public async Task<IActionResult> CreateOrganisation([FromBody] OrganisationRequestModel request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var response = await _organisationService.CreateOrganisationAsync(userId, request);

            if (response.StatusCode == 400)
            {
                return BadRequest(response);
            }

            return StatusCode(201, response);
        }

        [HttpPost("organisations/{orgId}/users")]
        public async Task<IActionResult> AddUserToOrganisation(string orgId, [FromBody] string userId)
        {
            var response = await _organisationService.AddUserToOrganisationAsync(orgId, userId);

            if (response.StatusCode == 400)
            {
                return BadRequest(response);
            }

            if (response.StatusCode == 404)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}
