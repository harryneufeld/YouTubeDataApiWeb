using YoutubeApi.Domain.Entities;

namespace YoutubeApi.Domain.Interfaces
{
    public interface IVideoRepository
    {
        Task<List<Video>> GetVideosByUserIdAsync(Guid userId);
        Task AddVideoAsync(Video video);
        Task DeleteVideoAsync(Video video);
        Task<int> SaveChangesAsync();
        Task RemoveComment(Comment comment);
        IQueryable<IGrouping<Guid, Video>> QueryVideosGroupedByUser();
        Task<List<Comment>> GetCommentsByVideoIdAsync(Guid videoId);
        Task<Guid?> GetUserIdByVideoIdAsync(Guid videoId);
    }
}