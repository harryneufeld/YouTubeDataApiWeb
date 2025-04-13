using YoutubeApi.Domain.Entities;
using YoutubeApi.Domain.Interfaces;

namespace YoutubeApi.Application.UseCases
{
    public class GetVideosByUserIdUseCase
    {
        private readonly IVideoRepository _videoRepository;

        public GetVideosByUserIdUseCase(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public async Task<List<Video>> ExecuteAsync(Guid userId)
        {
            return await _videoRepository.GetVideosByUserIdAsync(userId);
        }
    }
}