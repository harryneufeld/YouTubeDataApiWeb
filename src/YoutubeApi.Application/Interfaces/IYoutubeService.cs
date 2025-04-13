namespace YoutubeApi.Application.Interfaces
{
    internal interface IYoutubeService
    {
        public Task GetVideoDetails(string videoId);
    }
}
