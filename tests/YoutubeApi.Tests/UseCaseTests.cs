using YoutubeApi.Application.UseCases.VideoUseCases;
using YoutubeApi.Domain.Entities;
using YoutubeApi.Domain.Interfaces;
using YoutubeApi.Infrastructure.ApiClients.v3.Videos;

namespace YoutubeApi.Tests;

public class UseCaseTests
{
    private class FakeVideoRepository : IVideoRepository
    {
        public List<Video> Added { get; } = new();
        public int SaveCount { get; private set; }

        public Task AddVideoAsync(Video video) { Added.Add(video); return Task.CompletedTask; }
        public Task<int> SaveChangesAsync() { SaveCount++; return Task.FromResult(0); }
        public Task<List<Video>> GetVideosByUserIdAsync(Guid userId) => Task.FromResult(Added.Where(v => v.UserId == userId).ToList());
        public Task DeleteVideoAsync(Video video) { Added.Remove(video); return Task.CompletedTask; }
        public Task RemoveComment(Comment comment) => Task.CompletedTask;
        public IQueryable<IGrouping<Guid, Video>> QueryVideosGroupedByUser() => Added.GroupBy(v => v.UserId).AsQueryable();
        public Task<List<Comment>> GetCommentsByVideoIdAsync(Guid videoId) => Task.FromResult(new List<Comment>());
        public Task<Guid?> GetUserIdByVideoIdAsync(Guid videoId) => Task.FromResult<Guid?>(Added.FirstOrDefault(v => v.Id == videoId)?.UserId);
    }

    [Fact]
    public async Task ImportVideos_AddsEveryVideo()
    {
        var repo = new FakeVideoRepository();
        var videos = new[] { new Video(), new Video(), new Video() };

        await new ImportVideosUseCase(repo).ExecuteAsync(videos);

        Assert.Equal(videos, repo.Added);
        Assert.True(repo.SaveCount >= 1);
    }

    [Fact]
    public async Task GetVideosByUserId_ReturnsRepositoryResult()
    {
        var repo = new FakeVideoRepository();
        var user = Guid.NewGuid();
        await repo.AddVideoAsync(new Video { UserId = user });
        await repo.AddVideoAsync(new Video { UserId = Guid.NewGuid() });

        var result = await new GetVideosByUserIdUseCase(repo).ExecuteAsync(user);

        Assert.Single(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void VideosApi_WithoutApiKey_Throws(string? apiKey)
    {
        var ex = Assert.ThrowsAny<Exception>(() => new VideosApi(apiKey!));
        Assert.Equal("NoApiKeyProvidedException", ex.GetType().Name);
    }
}
