using System;

namespace YoutubeApi.Domain.Models.Comment
{
    public class CommentRoot
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string id { get; set; }
        public CommentSnippet snippet { get; set; }
    }

    public class CommentSnippet
    {
        public string authorDisplayName { get; set; }
        public string authorProfileImageUrl { get; set; }
        public string authorChannelUrl { get; set; }
        public AuthorChannelId authorChannelId { get; set; }
        public string channelId { get; set; }
        public string textDisplay { get; set; }
        public string textOriginal { get; set; }
        public string parentId { get; set; }
        public bool canRate { get; set; }
        public string viewerRating { get; set; }
        public int likeCount { get; set; }
        public string moderationStatus { get; set; }
        public DateTime publishedAt { get; set; }
        public DateTime updatedAt { get; set; }
    }

    public class AuthorChannelId
    {
        public string value { get; set; }
    }
}