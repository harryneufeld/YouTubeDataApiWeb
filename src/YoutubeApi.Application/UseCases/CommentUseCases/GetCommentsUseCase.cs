using YoutubeApi.Domain.Entities;
using YoutubeApi.Domain.Interfaces;

namespace YoutubeApi.Application.UseCases.CommentUseCases
{
    public class GetCommentsUseCase
    {
        private readonly IVideoRepository _videoRepository;

        public GetCommentsUseCase(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public async Task<List<Comment>> ExecuteAsync(Guid videoId)
        {
            return await _videoRepository.GetCommentsByVideoIdAsync(videoId);
        }
    }
}
