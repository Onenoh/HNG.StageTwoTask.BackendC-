using System.ComponentModel.DataAnnotations;

namespace HNG.StageTwoTask.BackendC_.Models.Access
{
    public class RegisterRequestModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; }
    }
}
