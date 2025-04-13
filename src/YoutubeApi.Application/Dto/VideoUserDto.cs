namespace YoutubeApi.Application.Dto
{
    public class VideoUserDto
    {
        public Guid UserId { get; set; }
        public int VideoCount { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
