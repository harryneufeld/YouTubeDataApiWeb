using Microsoft.AspNetCore.Components;

namespace YoutubeApi.Web.Pages
{
    public partial class Home
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        private string UserId { get; set; } = string.Empty;
        protected async override Task OnInitializedAsync()
        {
            // Initialization logic if needed
        }

        public void LoadYoutubeData()
        {
            NavigationManager.NavigateTo($"videos/{UserId}");
        }
    }
}