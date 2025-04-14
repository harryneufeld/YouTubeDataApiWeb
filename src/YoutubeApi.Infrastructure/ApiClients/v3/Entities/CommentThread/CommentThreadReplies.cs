using YoutubeApi.Infrastructure.ApiClients.v3.Models.Comment;

namespace YoutubeApi.Infrastructure.ApiClients.v3.Models.CommentThread
{
    public class CommentThreadReplies
    {
        public IList<CommentRoot> comments { get; set; }
    }
}