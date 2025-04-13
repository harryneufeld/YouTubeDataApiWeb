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

        public async Task AddVideoAsync(Video video)
        {
            await _context.Videos.AddAsync(video);
        }

        public async Task DeleteVideoAsync(Video video)
        {
            _context.Videos.Remove(video);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}