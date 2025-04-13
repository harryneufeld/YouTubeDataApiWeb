using Microsoft.AspNetCore.Components;
using YoutubeApi.Application.Dto;
using YoutubeApi.Application.UseCases.VideoUseCases;

namespace YoutubeApi.Web.Pages
{
    public partial class Admin
    {
        [Inject] public QueryAllVideosUseCase GetAllVideosUseCase { get; set; }
        public IList<VideoUserDto> VideoList = new List<VideoUserDto>();
        private string _userMessage;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                VideoList = await GetAllVideosUseCase.ExecuteAsync();
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task LoadUser(Guid userId)
        {
            NavigationManager.NavigateTo($"/videos/{userId}");
        }
    }
}