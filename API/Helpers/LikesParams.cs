namespace API.Helpers
{
    public class LikesParams :PaginationParams
    {
        public string UserId { get; set; }
        public string Predicate { get; set; }
    }
}
