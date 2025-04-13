namespace YoutubeApi.Domain.Models.VideoData
{
    public class ContentDetails
    {
        public string duration { get; set; }
        public string dimension { get; set; }
        public string definition { get; set; }
        public string caption { get; set; }
        public bool licensedContent { get; set; }
        public ContentRating contentRating { get; set; }
        public string projection { get; set; }
    }

    public class ContentRating
    {
    }
}
