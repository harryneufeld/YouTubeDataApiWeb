namespace YoutubeApi.Domain.Models.VideoData
{
    public class VideoRoot
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public VideoItem[] items { get; set; }
        public PageInfo pageInfo { get; set; }
    }
}
