namespace HNG.StageTwoTask.BackendC_.Models
{

    public class ErrorResponse
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }

    public class ValidationException : Exception
    {
        public List<ErrorResponse> Errors { get; }

        public ValidationException(List<ErrorResponse> errors)
        {
            Errors = errors;
        }
    }

    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message) : base(message)
        {
        }
    }

}
