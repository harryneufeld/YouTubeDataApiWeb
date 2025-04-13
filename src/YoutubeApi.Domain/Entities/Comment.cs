namespace YoutubeApi.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string Text { get; set; }
        public string Author { get; set; }
        public int LikeCount { get; set; } = 0;
        public int DislikeCount { get; set; } = 0;
        public Guid? VideoId { get; set; }
        public Video? Video { get; set; }
        public IList<Comment> ChildComments { get; set; }
        public Guid? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
    }
}