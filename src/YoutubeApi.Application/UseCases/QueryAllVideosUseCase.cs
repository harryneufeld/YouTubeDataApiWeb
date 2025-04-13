using Microsoft.EntityFrameworkCore;
using YoutubeApi.Application.Dto;
using YoutubeApi.Domain.Interfaces;

namespace YoutubeApi.Application.UseCases
{
    public class QueryAllVideosUseCase
    {
        private readonly IVideoRepository _videoRepository;

        public QueryAllVideosUseCase(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public async Task<IList<VideoUserDto>> ExecuteAsync()
        {
            var groupedVideos = _videoRepository.QueryVideosGroupedByUser();

            // Map domain entities to DTOs
            var videoUserDtos = await groupedVideos
                .Select(group => new VideoUserDto
                {
                    UserId = group.Key,
                    VideoCount = group.Count(),
                    CreatedAt = group.Min(v => v.CreatedAt)
                })
                .OrderByDescending(dto => dto.CreatedAt)
                .ToListAsync();

            return videoUserDtos;
        }
    }
}