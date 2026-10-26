using Microsoft.EntityFrameworkCore;
using YoutubeApi.Application.UseCases.VideoUseCases;
using YoutubeApi.Domain.Entities;
using YoutubeApi.Infrastructure.Persistence.Contexts;
using YoutubeApi.Infrastructure.Persistence.Repositories;

namespace YoutubeApi.Tests;

// VideoDbContext always uses the SQLite file in <bin>/Data, so these tests run against
// a freshly migrated copy of it, one test at a time.
[Collection(FileSystemCollection.Name)]
public class VideoRepositoryTests : IDisposable
{
    private readonly VideoDbContext _context;
    private readonly VideoRepository _repository;

    public VideoRepositoryTests()
    {
        _context = new VideoDbContext(new DbContextOptionsBuilder<VideoDbContext>().Options);
        _context.Database.EnsureDeleted();
        _context.Database.Migrate();
        _repository = new VideoRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private static Video CreateVideo(Guid userId, DateTime createdAt, params string[] commentTexts)
    {
        var video = new Video
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PublicId = $"vid-{Guid.NewGuid():N}",
            CreatedAt = createdAt,
            Comments = new List<Comment>()
        };
        for (int i = 0; i < commentTexts.Length; i++)
        {
            video.Comments.Add(new Comment
            {
                Id = Guid.NewGuid(),
                Text = commentTexts[i],
                Author = "Author",
                CreatedAt = createdAt.AddMinutes(commentTexts.Length - i), // reverse order on purpose
                ChildComments = new List<Comment>()
            });
        }
        return video;
    }

    [Fact]
    public void Migrations_MatchCurrentModel()
    {
        Assert.False(_context.Database.HasPendingModelChanges());
    }

    [Fact]
    public async Task AddVideo_ThenGetByUser_ReturnsOnlyThatUsersVideos()
    {
        var user = Guid.NewGuid();
        await _repository.AddVideoAsync(CreateVideo(user, DateTime.Now));
        await _repository.AddVideoAsync(CreateVideo(user, DateTime.Now));
        await _repository.AddVideoAsync(CreateVideo(Guid.NewGuid(), DateTime.Now));

        var videos = await _repository.GetVideosByUserIdAsync(user);

        Assert.Equal(2, videos.Count);
        Assert.All(videos, v => Assert.Equal(user, v.UserId));
    }

    [Fact]
    public async Task GetCommentsByVideoId_ReturnsCommentsOrderedByCreatedAt()
    {
        var video = CreateVideo(Guid.NewGuid(), DateTime.Now, "first", "second", "third");
        await _repository.AddVideoAsync(video);

        var comments = await _repository.GetCommentsByVideoIdAsync(video.Id);

        Assert.Equal(new[] { "third", "second", "first" }, comments.Select(c => c.Text));
        Assert.All(comments, c => Assert.NotNull(c.Video));
    }

    [Fact]
    public async Task GetUserIdByVideoId_ReturnsOwner()
    {
        var user = Guid.NewGuid();
        var video = CreateVideo(user, DateTime.Now);
        await _repository.AddVideoAsync(video);

        Assert.Equal(user, await _repository.GetUserIdByVideoIdAsync(video.Id));
    }

    [Fact]
    public async Task DeleteVideo_RemovesItsComments()
    {
        var video = CreateVideo(Guid.NewGuid(), DateTime.Now, "a", "b");
        await _repository.AddVideoAsync(video);

        await _repository.DeleteVideoAsync(video);

        Assert.Empty(await _context.Videos.ToListAsync());
        Assert.Empty(await _context.Comments.ToListAsync());
    }

    [Fact]
    public async Task RemoveComment_RemovesReplies()
    {
        var video = CreateVideo(Guid.NewGuid(), DateTime.Now, "parent");
        var parent = video.Comments[0];
        parent.ChildComments.Add(new Comment { Id = Guid.NewGuid(), Text = "reply", Author = "Author", VideoId = video.Id, ChildComments = new List<Comment>() });
        await _repository.AddVideoAsync(video);

        await _repository.RemoveComment(parent);

        Assert.Empty(await _context.Comments.ToListAsync());
    }

    [Fact]
    public async Task QueryAllVideosUseCase_GroupsByUserNewestFirst()
    {
        var olderUser = Guid.NewGuid();
        var newerUser = Guid.NewGuid();
        await _repository.AddVideoAsync(CreateVideo(olderUser, new DateTime(2024, 1, 1)));
        await _repository.AddVideoAsync(CreateVideo(olderUser, new DateTime(2024, 3, 1)));
        await _repository.AddVideoAsync(CreateVideo(newerUser, new DateTime(2024, 2, 1)));

        var result = await new QueryAllVideosUseCase(_repository).ExecuteAsync();

        Assert.Equal(new[] { newerUser, olderUser }, result.Select(r => r.UserId));
        Assert.Equal(new[] { 1, 2 }, result.Select(r => r.VideoCount));
        Assert.Equal(new DateTime(2024, 1, 1), result[1].CreatedAt);
    }
}
