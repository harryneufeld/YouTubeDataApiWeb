namespace YoutubeApi.Infrastructure.ApiClients.v3.Models.VideoData
{
    public class Thumbnails
    {
        public Default _default { get; set; }
        public Medium medium { get; set; }
        public High high { get; set; }
        public Standard standard { get; set; }
        public Maxres maxres { get; set; }
    }
    public class Default
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
    public class Medium : Default
    {
    }
    public class High : Default
    { 
    }
    public class Standard : Default
    {
    }
    public class Maxres : Default
    {
    }
}
