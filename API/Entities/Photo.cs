using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public int PublicId { get; set; }
        public string AppUserId { get; set; }

        [ForeignKey(nameof(AppUserId))]
        public ApplicationUser AppUser { get; set; }
    }
}