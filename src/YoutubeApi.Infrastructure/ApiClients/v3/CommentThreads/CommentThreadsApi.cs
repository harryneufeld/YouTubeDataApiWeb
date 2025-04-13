#nullable disable
using System.Net.Http.Headers;
using System.Text.Json;
using YoutubeApi.Domain.Models.CommentThread;
using YoutubeApi.Infrastructure.ApiClient;
using YoutubeApi.Infrastructure.Exceptions;

namespace YoutubeApi.Infrastructure.ApiClients.v3.CommentThreads
{
    public class CommentThreadsApi : ApiClientBase
    {
        private string _apiKey;
        public CommentThreadsApi(string apiKey)
            : base()
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new NoApiKeyProvidedException();
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri($"https://www.googleapis.com/youtube/v3/commentThreads");
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
        public async Task<YoutubeCommentThreadRoot> GetCommentThreadsAsync(
            string videoId, 
            string part, 
            int maxResults = 100, 
            string pageToken = null)
        {
            string query = $"?videoId={videoId}&key={_apiKey}&part={part}&maxResults={maxResults}";
            if (pageToken is not null)
                query += $"&pageToken={pageToken}";
            var response = await _httpClient.GetAsync(query);
            response.EnsureSuccessStatusCode();
            using var content = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<YoutubeCommentThreadRoot>(content);
            return result;
        }
    }
}
