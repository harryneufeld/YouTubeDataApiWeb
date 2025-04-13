using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using YoutubeApi.Domain.Entities;
using YoutubeApi.Infrastructure.Persistence.Contexts;

namespace YoutubeApi.Web.Pages
{
    public partial class Comments
    {
        [Parameter] public Guid VideoId { get; set; }
        [Inject] IDbContextFactory<VideoDbContext> DbContextFactory { get; set; }
        public Video Video { get; set; }
        public IList<Comment> CommentList = new List<Comment>();
        private string _userMessage;
        private VideoDbContext _context;
        private bool _isBusy;

        protected override async Task OnInitializedAsync()
        {
            
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isBusy = true;
                await InvokeAsync(StateHasChanged);

                if (_context is null)
                    _context = await DbContextFactory.CreateDbContextAsync();

                if (VideoId != Guid.Empty)
                {
                    var comments = await _context.Comments
                        .Include(c => c.Video)
                        //.Include(c => c.ChildComments)
                        .Where(c => c.VideoId == VideoId)
                        .OrderBy(c => c.CreatedAt)
                        .ToListAsync();
                    if (comments != null)
                    {
                        CommentList = comments;
                        Video = CommentList.FirstOrDefault()?.Video;
                    }
                    else
                    {
                        _userMessage = "No comments found";
                    }
                }

                _isBusy = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task Back()
        {
            // Navigate Back
            var userId = await _context.Videos.Where(v => v.Id == VideoId)
                .Select(v => v.UserId)
                .FirstOrDefaultAsync();
            NavigationManager.NavigateTo($"/videos/{userId}");
        }
    }
}