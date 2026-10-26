using YoutubeApi.Application.Mapper;
using YoutubeApi.Domain.Entities;
using YoutubeApi.Domain.Entities.ExportData;

namespace YoutubeApi.Tests;

public class VideoMapperTests
{
    private static Video CreateVideo()
    {
        var video = new Video
        {
            Id = Guid.NewGuid(),
            PublicId = "abc123",
            ChannelId = "channel-1",
            ChannelTitle = "Channel",
            VideoTitle = "Title",
            Description = "Description",
            Duration = 120,
            ViewCount = 1000,
            LikeCount = 50,
            DislikeCount = 2,
            PublishedAt = new DateTime(2024, 1, 2),
            Comments = new List<Comment>()
        };

        var parent = new Comment { Id = Guid.NewGuid(), Text = "Parent", Author = "Alice", LikeCount = 3, ChildComments = new List<Comment>() };
        var reply = new Comment { Id = Guid.NewGuid(), Text = "Reply", Author = "Bob", ParentComment = parent, ParentCommentId = parent.Id, ChildComments = new List<Comment>() };
        parent.ChildComments.Add(reply);
        var single = new Comment { Id = Guid.NewGuid(), Text = "Single", Author = "Carol" };

        video.Comments.Add(parent);
        video.Comments.Add(single);
        return video;
    }

    [Fact]
    public void MapVideoToDocNode_IncludesTopLevelCommentsAndReplies()
    {
        var video = CreateVideo();

        var nodes = VideoMapper.MapVideoToDocNode(video);

        Assert.Equal(new[] { "Parent", "Reply", "Single" }, nodes.Select(n => n.Value));
        Assert.All(nodes, n => Assert.IsType<YoutubeCommentVideoDetails>(n.Attributes));
    }

    [Fact]
    public void MapVideoToExportType_MapsVideoAndCommentFields()
    {
        var video = CreateVideo();
        var parent = video.Comments[0];

        var details = VideoMapper.MapVideoToExportType(video, parent);

        Assert.Equal("abc123", details.VideoId);
        Assert.Equal("channel-1", details.VideoAuthorChannelId);
        Assert.Equal("Channel", details.VideoChannelTitle);
        Assert.Equal("Title", details.VideoTitle);
        Assert.Equal(2, details.VideoCommentCount);
        Assert.Equal(50, details.VideoLikeCount);
        Assert.Equal(1000, details.VideoViewCount);
        Assert.Equal(parent.Id.ToString(), details.CommentId);
        Assert.Equal(3, details.CommentLikeCount);
        Assert.False(details.CommentIsReply);
        Assert.Null(details.ParentCommentId);
        Assert.Null(details.CommentText);
    }

    [Fact]
    public void MapVideoToExportType_MapsReplyAndOptionalText()
    {
        var video = CreateVideo();
        var parent = video.Comments[0];
        var reply = parent.ChildComments[0];

        var details = VideoMapper.MapVideoToExportType(video, reply, includeCommentText: true);

        Assert.True(details.CommentIsReply);
        Assert.Equal(parent.Id.ToString(), details.ParentCommentId);
        Assert.Equal("Alice", details.CommentIsReplyToName);
        Assert.Equal("Reply", details.CommentText);
    }
}
