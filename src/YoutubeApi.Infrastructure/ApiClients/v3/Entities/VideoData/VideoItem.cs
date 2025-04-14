namespace YoutubeApi.Infrastructure.ApiClients.v3.Models.VideoData
{
    public class VideoItem
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string id { get; set; }
        public VideoSnippet snippet { get; set; }
        public ContentDetails contentDetails { get; set; }
        public Status status { get; set; }
        public Statistics statistics { get; set; }
    }
}
