namespace YoutubeApi.Infrastructure.ApiClients.v3.Models.CommentThread
{
    public class CommentThreadSnippet
    {
        public string channelId { get; set; }
        public string videoId { get; set; }
        public TopLevelComment topLevelComment { get; set; }
        public bool canReply { get; set; }
        public int totalReplyCount { get; set; }
        public bool isPublic { get; set; }
    }
}
