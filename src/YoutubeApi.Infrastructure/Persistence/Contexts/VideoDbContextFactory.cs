   using Microsoft.EntityFrameworkCore;
   using Microsoft.EntityFrameworkCore.Design;

   namespace YoutubeApi.Infrastructure.Persistence.Contexts
   {
       public class VideoDbContextFactory : IDesignTimeDbContextFactory<VideoDbContext>
       {
           public VideoDbContext CreateDbContext(string[] args)
           {
               SQLitePCL.Batteries.Init(); // Initialisiere SQLite
               
               var optionsBuilder = new DbContextOptionsBuilder<VideoDbContext>();
               optionsBuilder.UseSqlite("Data Source=Data/VideoData.db");

               return new VideoDbContext(optionsBuilder.Options);
           }
       }
   }
   