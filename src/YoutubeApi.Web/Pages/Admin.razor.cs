using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using YoutubeApi.Application.Dto;
using YoutubeApi.Infrastructure.Persistence.Contexts;

namespace YoutubeApi.Web.Pages
{
    public partial class Admin
    {
        [Inject] private IDbContextFactory<VideoDbContext> DbContextFactory { get; set; } = default!;
        public IList<VideoUserDto> VideoList = new List<VideoUserDto>();
        private string _userMessage;
        private VideoDbContext _context { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (_context is null)
                    _context = DbContextFactory.CreateDbContext();
                VideoList = await _context.Videos
                    .GroupBy(x => x.UserId)
                    .Select(x => new VideoUserDto()
                    {
                        UserId = x.Key,
                        VideoCount = x.Count(),
                        CreatedAt = x.First().CreatedAt,
                    })
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task LoadUser(Guid userId)
        {
            NavigationManager.NavigateTo($"/videos/{userId}");
        }
    }
}