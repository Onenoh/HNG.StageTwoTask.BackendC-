namespace HNG.StageTwoTask.BackendC_.Models.Access
{
    public class LoginResponseModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public LoginDataResponse Data { get; set; }
    }

    public class LoginDataResponse
    {
        public string AccessToken { get; set; }
        public RegisterData User { get; set; }
    }
}
