using YoutubeApi.Domain.Entities;
using YoutubeApi.Domain.Interfaces;

namespace YoutubeApi.Application.UseCases
{
    public class RemoveCommentUseCase
    {
        private readonly IVideoRepository _videoRepository;

        public RemoveCommentUseCase(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public async Task ExecuteAsync(Comment comment)
        {
            await _videoRepository.RemoveComment(comment);
            await _videoRepository.SaveChangesAsync();
        }
    }
}