using YoutubeApi.Domain.Interfaces;

namespace YoutubeApi.Application.UseCases
{
    public class SaveChangesUseCase
    {
        private readonly IVideoRepository _videoRepository;

        public SaveChangesUseCase(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public async Task<int> ExecuteAsync()
        {
            return await _videoRepository.SaveChangesAsync();
        }
    }
}