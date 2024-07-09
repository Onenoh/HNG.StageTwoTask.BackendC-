using HNG.StageTwoTask.BackendC_.Models.Access;
using HNG.StageTwoTask.BackendC_.Models.Organisation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HNG.StageTwoTask.BackendC_.Models.Organisation
{
    public class Organisations
    {
        public string? OrgId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<OrganisationUser>? OrganisationUser { get; set; }
    }
}
