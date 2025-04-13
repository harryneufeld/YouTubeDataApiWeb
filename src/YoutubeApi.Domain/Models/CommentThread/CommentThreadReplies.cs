using YoutubeApi.Domain.Models.Comment;

namespace YoutubeApi.Domain.Models.CommentThread
{
    public class CommentThreadReplies
    {
        public IList<CommentRoot> comments { get; set; }
    }
}