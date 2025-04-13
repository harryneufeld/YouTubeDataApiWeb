using YoutubeApi.Domain.Entities;
using YoutubeApi.Domain.Interfaces;

namespace YoutubeApi.Application.UseCases
{
    public class DeleteVideoUseCase
    {
        private readonly IVideoRepository _videoRepository;

        public DeleteVideoUseCase(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public async Task ExecuteAsync(Video video)
        {
            await _videoRepository.DeleteVideoAsync(video);
            await _videoRepository.SaveChangesAsync();
        }
    }
}