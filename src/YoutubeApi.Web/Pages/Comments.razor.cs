using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using YoutubeApi.Application.UseCases.CommentUseCases;
using YoutubeApi.Application.UseCases.VideoUseCases;
using YoutubeApi.Domain.Entities;
using YoutubeApi.Infrastructure.Persistence.Contexts;

namespace YoutubeApi.Web.Pages
{
    public partial class Comments
    {
        [Parameter] public Guid VideoId { get; set; }
        [Inject] public GetCommentsUseCase GetCommentsUseCase { get; set; }
        [Inject] public GetUserIdByVideoIdUseCase GetUserIdByVideoIdUseCase { get; set; }
        public Video Video { get; set; }
        public IList<Comment> CommentList = new List<Comment>();
        private string _userMessage;
        private bool _isBusy;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isBusy = true;
                await InvokeAsync(StateHasChanged);

                if (VideoId != Guid.Empty)
                {
                    var comments = await GetCommentsUseCase.ExecuteAsync(VideoId);

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
            var userId = await GetUserIdByVideoIdUseCase.ExecuteAsync(VideoId);
            NavigationManager.NavigateTo($"/videos/{userId}");
        }
    }
}