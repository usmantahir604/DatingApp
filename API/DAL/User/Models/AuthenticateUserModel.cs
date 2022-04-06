namespace API.DAL.User.Models
{
    public class AuthenticateUserModel
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
