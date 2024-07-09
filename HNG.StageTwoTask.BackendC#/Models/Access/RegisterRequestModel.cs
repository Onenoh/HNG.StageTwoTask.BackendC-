﻿using System.ComponentModel.DataAnnotations;

namespace HNG.StageTwoTask.BackendC_.Models.Access
{
    public class RegisterRequestModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
    }
}
