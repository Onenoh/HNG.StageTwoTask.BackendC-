using System.ComponentModel.DataAnnotations;

namespace HNG.StageTwoTask.BackendC_.Models.Access
{
    public class LoginRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
