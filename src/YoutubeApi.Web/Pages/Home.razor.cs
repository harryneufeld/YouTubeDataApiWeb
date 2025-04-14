using Microsoft.AspNetCore.Components;

namespace YoutubeApi.Web.Pages
{
    public partial class Home
    {
        [Inject] private NavigationManager NavigationManager { get; set; }
        private string UserId { get; set; } = string.Empty;
        private string _version;

        protected override void OnInitialized()
        {
            _version = GetType().Assembly.GetName().Version?.ToString() ?? "Unknown";
            //_appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public void LoadYoutubeData()
        {
            NavigationManager.NavigateTo($"videos/{UserId}");
        }
    }
}