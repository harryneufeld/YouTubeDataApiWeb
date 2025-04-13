using Microsoft.EntityFrameworkCore;
using YoutubeApi.Domain.Entities;

namespace YoutubeApi.Infrastructure.Persistence.Contexts
{
    public class VideoDbContext : DbContext
    {
        public DbSet<Video> Videos { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public string DbPath { get; private set; }

        public VideoDbContext(DbContextOptions<VideoDbContext> options) : base(options)
        {
            var executingDirectory = AppContext.BaseDirectory;
            if (!Directory.Exists(Path.Combine(executingDirectory, "Data")))
                Directory.CreateDirectory(Path.Combine(executingDirectory, "Data"));
            DbPath = Path.Combine(executingDirectory, "Data", "YoutubeApi.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Video>()
                .HasMany(v => v.Comments)
                .WithOne(c => c.Video)
                .HasForeignKey(c => c.VideoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasMany(c => c.ChildComments)
                .WithOne(c => c.ParentComment)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
