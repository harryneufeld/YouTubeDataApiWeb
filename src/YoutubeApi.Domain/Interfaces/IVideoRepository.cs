using YoutubeApi.Domain.Entities;

namespace YoutubeApi.Domain.Interfaces
{
    public interface IVideoRepository
    {
        Task<List<Video>> GetVideosByUserIdAsync(Guid userId);
        Task AddVideoAsync(Video video);
        Task DeleteVideoAsync(Video video);
        Task SaveChangesAsync();
    }
}