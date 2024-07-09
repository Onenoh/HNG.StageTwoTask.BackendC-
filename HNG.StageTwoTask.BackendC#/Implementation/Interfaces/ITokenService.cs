using HNG.StageTwoTask.BackendC_.Models.Access;

namespace HNG.StageTwoTask.BackendC_.Implementation.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
    }
}
