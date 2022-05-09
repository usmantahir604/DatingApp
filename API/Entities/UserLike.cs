

namespace API.Entities
{
    public class UserLike
    {
        public ApplicationUser SourceUser { get; set; }
        public string SourceUserId { get; set; }
        public ApplicationUser LikedUser { get; set; }
        public string LikedUserId { get; set; }
    }
}
