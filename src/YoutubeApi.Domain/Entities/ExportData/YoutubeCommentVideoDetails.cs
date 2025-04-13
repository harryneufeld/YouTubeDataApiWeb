namespace YoutubeApi.Domain.Entities.ExportData
{
    public class YoutubeCommentVideoDetails
    {
        public string? VideoChannelTitle { get; set; }
        public string? VideoAuthorChannelId { get; set; }
        public DateTime? VideoPublishedAt { get; set; }
        public string? VideoId { get; set; }
        public string? VideoTitle { get; set; }
        public string? VideoDescription { get; set; }
        public int? VideoDuration { get; set; }
        public int? VideoViewCount { get; set; }
        public int? VideoLikeCount { get; set; }
        public int? VideoDislikeCount { get; set; }
        public int? VideoCommentCount { get; set; }
        public int? CommentLikeCount { get; set; }
        public int? CommentDislikeCount { get; set; }
        public string? CommentId { get; set; }
        public int? CommentReplyCount { get; set; }
        public bool? CommentIsReply { get; set; }
        public string? CommentText { get; set; }
        public string? ParentCommentId { get; set; }
        public string? CommentIsReplyToName { get; set; }
        public DateTime? CommentCreatedAt { get; set; }
    }
}
