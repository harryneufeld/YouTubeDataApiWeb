using System.Net.Http.Headers;
using System.Text.Json;
using YoutubeApi.Domain.Models.VideoData;
using YoutubeApi.Infrastructure.ApiClient;
using YoutubeApi.Infrastructure.Exceptions;

namespace YoutubeApi.Infrastructure.ApiClients.v3.Videos
{
    public class VideosApi : ApiClientBase
    {
        private string _apiKey;
        public VideosApi(string apiKey)
            : base()
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new NoApiKeyProvidedException();
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri($"https://www.googleapis.com/youtube/v3/videos");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                               new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Requests a list of TopLevel-Comments from a Video
        /// Ref: https://developers.google.com/youtube/v3/docs/commentThreads/list
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="part"></param>
        /// <param name="maxResults"></param>
        /// <returns></returns>
        public async Task<VideoRoot> GetVideosAsync(
            string videoId, 
            string part = "snippet,status,statistics,contentDetails,player,recordingDetails", 
            int maxResults = 100, 
            string pageToken = null)
        {
            string query = $"?id={videoId}&key={_apiKey}&part={part}&maxResults={maxResults}";
            if (pageToken is not null)
                query += $"&pageToken={pageToken}";
            var response = await _httpClient.GetAsync(query);
            response.EnsureSuccessStatusCode();
            using var content = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<VideoRoot>(content);
            return result;
        }
    }
}
