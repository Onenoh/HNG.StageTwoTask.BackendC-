using HNG.StageTwoTask.BackendC_.Models;
using HNG.StageTwoTask.BackendC_.Models.Access;
using Microsoft.AspNetCore.Identity.Data;

namespace HNG.StageTwoTask.BackendC_.Implementation.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponseModel> RegisterAsync(RegisterRequestModel request);
        Task<LoginResponseModel> LoginAsync(LoginRequestModel request);
    }
}
