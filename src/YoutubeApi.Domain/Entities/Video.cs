using System.ComponentModel.DataAnnotations.Schema;

namespace YoutubeApi.Domain.Entities
{
    public class Video : BaseEntity
    {
        public Guid UserId { get; set; }
        public string? VideoTitle { get; set; }
        public string? Description { get; set; }
        public int? Duration { get; set; }
        public int? ViewCount { get; set; }
        public int? LikeCount { get; set; }
        public int? DislikeCount { get; set; }
        public string? PublicId { get; set; }
        public string? ChannelId { get; set; }
        public string? ChannelTitle { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string? Url { get; set; }
        public IList<Comment> Comments { get; set; }
        public bool HasValidationError { get; set; }
        public string? ValidationErrorMessage { get; set; }
        [NotMapped]
        public bool IsChecked { get; set; }
    }
}
