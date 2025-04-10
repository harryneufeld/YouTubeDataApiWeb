using System.ComponentModel.DataAnnotations.Schema;

namespace YoutubeApi.Infrastructure.Persistence.Models
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ChangedAt { get; set; }
    }
}
