using HNG.StageTwoTask.BackendC_.Models.Access;


namespace HNG.StageTwoTask.BackendC_.Models.Organisation
{
    public class OrganisationResponseModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public ResponseModelData Data { get; set; }
    }

    public class ResponseModelData
    {
        public string AccessToken { get; set; }
        public User User { get; set; }
        public List<Organisations> Organisations { get; set; }
    }

}
