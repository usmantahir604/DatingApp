using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class ApplicationUser: IdentityUser
    {
        public ApplicationUser()
        {
            RefreshTokens = new HashSet<RefreshToken>();
        }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
