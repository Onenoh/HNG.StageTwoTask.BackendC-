using System.Net;

namespace HNG.StageTwoTask.BackendC_.Models.Access
{
    public class RegisterResponseModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public RegisterResponseData Data { get; set; }
    }

    public class RegisterResponseData
    {
        public string AccessToken { get; set; }
        public RegisterData User { get; set; }
        public List<ErrorField> Errors { get; set; } = new List<ErrorField>();
    }

    public class RegisterData
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class ErrorField
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }

}
