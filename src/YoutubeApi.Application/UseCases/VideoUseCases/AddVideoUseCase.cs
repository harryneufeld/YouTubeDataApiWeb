using YoutubeApi.Domain.Entities;
using YoutubeApi.Domain.Interfaces;

namespace YoutubeApi.Application.UseCases.VideoUseCases
{
    public class AddVideoUseCase
    {
        private readonly IVideoRepository _videoRepository;

        public AddVideoUseCase(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public async Task ExecuteAsync(Video video)
        {
            await _videoRepository.AddVideoAsync(video);
            await _videoRepository.SaveChangesAsync();
        }
    }
}