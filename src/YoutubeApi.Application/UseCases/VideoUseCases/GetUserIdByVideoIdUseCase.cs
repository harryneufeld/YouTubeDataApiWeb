using YoutubeApi.Domain.Interfaces;

namespace YoutubeApi.Application.UseCases.VideoUseCases
{
    public class GetUserIdByVideoIdUseCase
    {
        private readonly IVideoRepository _videoRepository;

        public GetUserIdByVideoIdUseCase(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public async Task<Guid?> ExecuteAsync(Guid videoId)
        {
            return await _videoRepository.GetUserIdByVideoIdAsync(videoId);
        }
    }
}