using Microsoft.EntityFrameworkCore;
using YoutubeApi.Domain.Entities;
using YoutubeApi.Domain.Interfaces;
using YoutubeApi.Infrastructure.Persistence.Contexts;

namespace YoutubeApi.Infrastructure.Persistence.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly VideoDbContext _context;

        public VideoRepository(VideoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Video>> GetVideosByUserIdAsync(Guid userId)
        {
            return await _context.Videos
                .Where(v => v.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Comment>> GetCommentsByVideoIdAsync(Guid videoId)
        {
            return await _context.Comments
                .Include(c => c.Video)
                .Where(c => c.VideoId == videoId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Guid?> GetUserIdByVideoIdAsync(Guid videoId)
        {
            return await _context.Videos
                .Where(v => v.Id == videoId)
                .Select(v => v.UserId)
                .FirstOrDefaultAsync();
        }

        public async Task AddVideoAsync(Video video)
        {
            await _context.Videos.AddAsync(video);
            await SaveChangesAsync();
        }

        public async Task DeleteVideoAsync(Video video)
        {
            _context.Videos.Remove(video);
            await SaveChangesAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task RemoveComment(Comment comment)
        {
            _context.Comments.Remove(comment);
            await SaveChangesAsync();
        }

        public IQueryable<IGrouping<Guid, Video>> QueryVideosGroupedByUser()
        {
            return _context.Videos.GroupBy(v => v.UserId);
        }
    }
}