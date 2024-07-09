using HNG.StageTwoTask.BackendC_.Models.Organisation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HNG.StageTwoTask.BackendC_.Models.Access
{
    public class User
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public List<OrganisationUser> OrganisationUser { get; set; }

    }
}
