using HNG.StageTwoTask.BackendC_.Models.Access;
using HNG.StageTwoTask.BackendC_.Models.Organisation;


public interface IOrganisationService
{
    Task<OrganisationResponseModel> GetUserDetailsAsync(string userId, string requestingUserId);
    Task<OrganisationResponseModel> GetUserByIdAsync(string userId);
    Task<OrganisationResponseModel> GetOrganisationsAsync(string userId);
    Task<OrganisationResponseModel> GetOrganisationByIdAsync(string userId, string orgId);
    Task<OrganisationResponseModel> CreateOrganisationAsync(string userId, OrganisationRequestModel request);
    Task<OrganisationResponseModel> AddUserToOrganisationAsync(string orgId, string userId);
    Task<Organisations> CreateDefaultOrganisationAsync(User user);
}
