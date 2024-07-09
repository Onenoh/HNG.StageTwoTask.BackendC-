using HNG.StageTwoTask.BackendC_.Models.Access;
using System.ComponentModel.DataAnnotations;

namespace HNG.StageTwoTask.BackendC_.Models.Organisation
{
    public class OrganisationUser
    {

        public Guid UserId { get; set; }
        public User User { get; set; }
        public string OrgId { get; set; }
        public Organisations Organisation { get; set; }
    }
}
