namespace YoutubeApi.Domain.Models.CommentThread
{

    public class CommentThreadItem
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string id { get; set; }
        public CommentThreadSnippet snippet { get; set; }
        public CommentThreadReplies replies { get; set; }
    }
}
