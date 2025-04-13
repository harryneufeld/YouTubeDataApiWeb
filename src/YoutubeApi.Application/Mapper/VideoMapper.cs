using YoutubeApi.Infrastructure.Persistence.Models;
using YoutubeApi.Infrastructure.Persistence.Models.ExportData;

namespace YoutubeApi.Application.Mapper
{
    public static class VideoMapper
    {
        /// <summary>
        /// Map Video including Comments to YoutubeCommentVideoDetails
        public static IList<DocNode> MapVideoToDocNode(Video video)
        {
            var result = new List<DocNode>();
            foreach (var comment in video.Comments)
            {
                YoutubeCommentVideoDetails node = MapVideoToExportType(video, comment);
                result.Add(new DocNode() { Attributes = node, Value = comment.Text });

                if (comment.ChildComments?.Count > 0)
                {
                    foreach (var reply in comment.ChildComments)
                    {
                        YoutubeCommentVideoDetails replyNode = MapVideoToExportType(video, reply);
                        result.Add(new DocNode() { Attributes = replyNode, Value = reply.Text });
                    }
                }
            }
            return result;
        }

        public static YoutubeCommentVideoDetails MapVideoToExportType(Video video, Comment comment, bool includeCommentText = false)
        {
            var node = new YoutubeCommentVideoDetails()
            {
                VideoId = video.PublicId,
                VideoAuthorChannelId = video.ChannelId,
                VideoChannelTitle = video.ChannelTitle,
                VideoTitle = video.VideoTitle,
                VideoCommentCount = video.Comments.Count,
                VideoLikeCount = video.LikeCount,
                VideoDislikeCount = video.DislikeCount,
                VideoDescription = video.Description,
                VideoDuration = video.Duration,
                VideoPublishedAt = video.PublishedAt,
                VideoViewCount = video.ViewCount,
                CommentId = comment.Id.ToString(),
                CommentLikeCount = comment.LikeCount,
                CommentDislikeCount = comment.DislikeCount,
                CommentIsReply = comment.ParentComment != null,
                ParentCommentId = comment.ParentCommentId?.ToString(),
                CommentIsReplyToName = comment.ParentComment?.Author,
                CommentText = includeCommentText ? comment.Text : null,
                CommentCreatedAt = comment.CreatedAt,
            };
            return node;
        }
    }
}
