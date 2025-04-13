namespace YoutubeApi.Domain.Models.CommentThread
{
    public class YoutubeCommentThreadRoot
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string nextPageToken { get; set; }
        public CommentThreadSnippet snippet { get; set; }
        public PageInfo pageInfo { get; set; }
        public CommentThreadItem[] items { get; set; }
    }
}