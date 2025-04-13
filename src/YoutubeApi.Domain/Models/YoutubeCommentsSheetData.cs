namespace YoutubeApi.Domain.Models
{
    public class YoutubeCommentsSheetData
    {
        public string VideoId { get; set; }
        public string VideoTitle { get; set; }
        public string VideoUrl { get; set; }
        public string CommentAuthor { get; set; }
        public string CommentText { get; set; }
    }
}
