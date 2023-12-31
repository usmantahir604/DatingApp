﻿namespace API.DAL.User.Models
{
    public class AuthenticateUserModel
    {
        public string UserName { get; set; }
        public string PhotoUrl { get; set; }
        public string KnownAs { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Gender { get; set; }
    }
}
