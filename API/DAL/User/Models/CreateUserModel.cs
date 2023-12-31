﻿namespace API.DAL.User.Models
{
    public class CreateUserModel
    {
        public string UserName { get; set; }
        public int PlanId { get; set; }
        public string KnownAs { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Password { get; set; }
    }
}
