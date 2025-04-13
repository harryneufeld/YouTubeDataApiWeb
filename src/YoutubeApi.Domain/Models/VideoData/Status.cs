namespace YoutubeApi.Domain.Models.VideoData
{
    public class Status
    {
        public string uploadStatus { get; set; }
        public string privacyStatus { get; set; }
        public string license { get; set; }
        public bool embeddable { get; set; }
        public bool publicStatsViewable { get; set; }
        public bool madeForKids { get; set; }
    }
}
