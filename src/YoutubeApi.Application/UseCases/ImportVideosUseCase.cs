using YoutubeApi.Domain.Entities;
using YoutubeApi.Domain.Interfaces;

namespace YoutubeApi.Application.UseCases
{
    public class ImportVideosUseCase
    {
        private readonly IVideoRepository _videoRepository;

        public ImportVideosUseCase(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public async Task ExecuteAsync(IEnumerable<Video> videos)
        {
            foreach (var video in videos)
            {
                await _videoRepository.AddVideoAsync(video);
            }
            await _videoRepository.SaveChangesAsync();
        }
    }
}