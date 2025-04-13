using YoutubeApi.Infrastructure.ApiClients.v3.Models;

namespace YoutubeApi.Infrastructure.ApiClients.v3.Models.VideoData
{
    public class VideoRoot
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public VideoItem[] items { get; set; }
        public PageInfo pageInfo { get; set; }
    }
}
