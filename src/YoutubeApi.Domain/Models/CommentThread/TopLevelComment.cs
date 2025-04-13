namespace YoutubeApi.Domain.Models.CommentThread
{
    public class TopLevelComment
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string id { get; set; }
        public CommentSnippet snippet { get; set; }
    }
}
