using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Serilog;
using System.ComponentModel;
using System.IO.Compression;
using YoutubeApi.Application.Mapper;
using YoutubeApi.Infrastructure.ApiClients.v3.CommentThreads;
using YoutubeApi.Infrastructure.ApiClients.v3.Videos;
using YoutubeApi.Infrastructure.IO;
using YoutubeApi.Infrastructure.ApiClients.v3.Models.CommentThread;
using YoutubeApi.Infrastructure.ApiClients.v3.Models.VideoData;
using YoutubeApi.Domain.Entities;
using YoutubeApi.Domain.Entities.ExportData;
using YoutubeApi.Application.UseCases;
using YoutubeApi.Application.UseCases.VideoUseCases;
using YoutubeApi.Application.UseCases.CommentUseCases;

namespace YoutubeApi.Web.Pages
{
    public partial class Videos
    {
        [Parameter] public Guid UserId { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public IConfiguration Config { get; set; } = null!;
        [Inject] public GetVideosByUserIdUseCase GetVideosByUserIdUseCase { get; set; } = null!;
        [Inject] public GetCommentsUseCase GetCommentsUseCase { get; set; } = null!;
        [Inject] public AddVideoUseCase AddVideoUseCase { get; set; } = null!;
        [Inject] public DeleteVideoUseCase DeleteVideoUseCase { get; set; } = null!;
        [Inject] public SaveChangesUseCase SaveChangesUseCase { get; set; } = null!;
        [Inject] public ImportVideosUseCase ImportVideosUseCase { get; set; } = null!;
        [Inject] public RemoveCommentUseCase RemoveCommentUseCase { get; set; } = null!;
        public Video Video { get; set; } = new Video();
        public IList<Video> VideoList = new List<Video>();

        private MarkupString _userMessage;
        private string _downloadPath;
        private string _file;
        private bool _isDownloadReady = false;
        private int _downloadProgress = 0;
        private bool _isBusy = false;
        private bool _isApiLoading = false;
        private SheetColumnNames _selectedColumn;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isBusy = true;
                await InvokeAsync(StateHasChanged);

                if (UserId != Guid.Empty)
                {
                    List<Video> videos = await GetVideosByUserIdUseCase.ExecuteAsync(UserId);
                    if (videos != null)
                        VideoList = videos;
                    else
                        _userMessage = new MarkupString("Es wurden keine Videos gefunden.");
                }
                else
                {
                    UserId = Guid.NewGuid();
                }

                _isBusy = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task AddVideo()
        {
            if (UserId == Guid.Empty)
            {
                UserId = Guid.NewGuid();
            }

            try
            {
                Video.UserId = UserId;
                Video.PublicId = Video.PublicId!.Trim();
                await AddVideoUseCase.ExecuteAsync(Video);
                NavigationManager.NavigateTo($"videos/{UserId}");
            }
            catch (Exception e)
            {
                _userMessage = new MarkupString(e.Message + (e.InnerException != null ? e.InnerException.Message : ""));
            }

            VideoList.Add(Video);
            Video = new Video();
            await InvokeAsync(StateHasChanged);
        }

        private async Task CancelYoutubeData()
        {
            _isApiLoading = false;
            _isBusy = false;
            _downloadProgress = 0;
            _userMessage = new MarkupString();
            await InvokeAsync(StateHasChanged);
        }

        private async Task LoadYoutubeData()
        {
            if (_isBusy)
                return;
            _isBusy = true;
            _isApiLoading = true;
            await InvokeAsync(StateHasChanged);
            if (VideoList != null)
            {
                // get api key from environment variable
                var apiKeyEnv = Environment.GetEnvironmentVariable("YOUTUBE_API_KEY");
                var apiKey = !string.IsNullOrEmpty(apiKeyEnv) ? apiKeyEnv : Config.GetValue<string>("YoutubeApiKey");
                var commentThreadsClient = new CommentThreadsApi(apiKey);
                var videoClient = new VideosApi(apiKey);
                int videoCount = 0;

                foreach (var video in VideoList)
                {
                    if (!_isApiLoading)
                        break;
                    videoCount++;
                    _userMessage = new MarkupString($"Loading video {videoCount} of {VideoList.Count}.");
                    _downloadProgress = videoCount * 100 / VideoList.Count;
                    VideoRoot videoSnippet = null;

                    // Check PublicId
                    if (video.PublicId is null || video.PublicId.Length < 11)
                    {
                        video.ValidationErrorMessage = "Video-ID is Empty!";
                        video.HasValidationError = true;
                        await SaveChangesUseCase.ExecuteAsync();
                        continue;
                    }
                    // Getting Video-Title
                    if (string.IsNullOrWhiteSpace(video.VideoTitle))
                    {
                        try
                        {
                            videoSnippet = await videoClient.GetVideosAsync(video.PublicId?.Trim());
                        }
                        catch (Exception e)
                        {
                            _userMessage = new MarkupString("Error loading video-details: " + e.Message);
                            video.ValidationErrorMessage = e.Message;
                            video.HasValidationError = true;
                            await InvokeAsync(StateHasChanged);
                            return;
                        }
                        if (videoSnippet?.items is not null)
                        {
                            video.VideoTitle = videoSnippet.items.FirstOrDefault()?.snippet.title;
                            await SaveChangesUseCase.ExecuteAsync();
                            await InvokeAsync(StateHasChanged);
                        }
                    }
                    else
                    {
                        // Update Progress for skipping videos
                        if (videoCount % 10 == 0)
                        {
                            _userMessage = new MarkupString($"Skipping video {videoCount} of {VideoList.Count}.");
                            await InvokeAsync(StateHasChanged);
                        }
                    }
                    if (string.IsNullOrWhiteSpace(video.Url))
                    {
                        video.Url = $"https://www.youtube.com/watch?v={video.PublicId}";
                        await SaveChangesUseCase.ExecuteAsync();
                    }

                    // Video Details
                    if (videoSnippet != null && videoSnippet.items.Count() > 0)
                    {
                        // video.Duration = Int32.Parse(videoSnippet.items[0].contentDetails.duration);
                        video.Description = videoSnippet.items[0].snippet.description;
                        video.ChannelId = videoSnippet.items[0].snippet.channelId;
                        video.ChannelTitle = videoSnippet.items[0].snippet.channelTitle;
                        video.PublishedAt = videoSnippet.items[0].snippet.publishedAt;
                        video.LikeCount = Int32.Parse(videoSnippet.items[0].statistics.likeCount ?? "0");
                        video.DislikeCount = Int32.Parse(videoSnippet.items[0].statistics.dislikeCount ?? "0");
                        video.ViewCount = Int32.Parse(videoSnippet.items[0].statistics.viewCount ?? "0");
                        await SaveChangesUseCase.ExecuteAsync();
                    } else
                    {
                        video.HasValidationError = true;
                        video.ValidationErrorMessage = "PublicId invalid!";
                        await SaveChangesUseCase.ExecuteAsync();
                        continue;
                    }

                    // Deleteing Comments that are incomplete
                    if (video.Comments is not null)
                    {
                        if (video.Comments.Any() && video.HasValidationError)
                        {
                            foreach (var com in video.Comments)
                            {
                                await RemoveCommentUseCase.ExecuteAsync(com);
                            }
                            video.ValidationErrorMessage = null;
                            video.HasValidationError = false;
                            await SaveChangesUseCase.ExecuteAsync();
                        }
                        else if (video.Comments.Any() && !video.HasValidationError)
                        {
                            continue;
                        }
                    }

                    video.Comments = new List<Comment>();
                    try
                    {
                        string pageToken = null;
                        bool run = true;
                        while (run)
                        {
                            YoutubeCommentThreadRoot commentThreads = null;
                            try
                            {
                                commentThreads = await commentThreadsClient.GetCommentThreadsAsync(videoId: video.PublicId, part: "snippet,replies", pageToken: pageToken);
                            }
                            catch (Exception e)
                            {
                                _userMessage = new MarkupString("Error loading comments: " + e.Message);
                                await InvokeAsync(StateHasChanged);
                            }

                            if (commentThreads is null)
                            {
                                run = false;
                                break;
                            }

                            foreach (var item in commentThreads.items)
                            {
                                var comment = new Comment()
                                {
                                    Text = item.snippet.topLevelComment.snippet.textOriginal,
                                    Author = item.snippet.topLevelComment.snippet.authorDisplayName,
                                    CreatedAt = item.snippet.topLevelComment.snippet.publishedAt,
                                    LikeCount = item.snippet.topLevelComment.snippet.likeCount,
                                    ChildComments = new List<Comment>()
                                };
                                if (item.replies is not null && item.replies.comments is not null)
                                {
                                    foreach (var reply in item.replies.comments)
                                    {
                                        comment.ChildComments.Add(new Comment()
                                        {
                                            Text = reply.snippet.textOriginal,
                                            Author = reply.snippet.authorDisplayName,
                                            CreatedAt = reply.snippet.publishedAt,
                                            LikeCount = reply.snippet.likeCount,
                                            ChildComments = new List<Comment>()
                                        });
                                    }
                                }
                                video.Comments.Add(comment);
                            }

                            if (!string.IsNullOrWhiteSpace(commentThreads?.nextPageToken))
                                pageToken = commentThreads.nextPageToken;
                            else
                                run = false;
                        }
                    }
                    catch (Exception e)
                    {
                        video.HasValidationError = true;
                        video.ValidationErrorMessage = e.Message + (e.InnerException != null ? e.InnerException.Message : "");
                    }
                    if (await SaveChangesUseCase.ExecuteAsync() == 0)
                    {
                        _userMessage = new MarkupString($"{_userMessage + Environment.NewLine}No comments Added to {video.VideoTitle ?? video.PublicId}");
                    }
                    else
                    {
                        _userMessage = new MarkupString($"{_userMessage + Environment.NewLine}Comments for {video.VideoTitle ?? video.PublicId} were added.");
                    }
                }
            }
            else
            {
                _userMessage = new MarkupString("No videos were found.");
                await InvokeAsync(StateHasChanged);
            }
            _isApiLoading = false;
            _isBusy = false;
        }

        private async Task UploadFile(InputFileChangeEventArgs e)
        {
            if (_isBusy)
                return;
            _isBusy = true;
            var file = e.File;
            string trustedFileName = null!;
            try
            {
                trustedFileName = Path.GetRandomFileName();
                var path = Path.Combine("wwwroot",
                    "files",
                    trustedFileName);

                await using FileStream fs = new(path, FileMode.Create);
                await file.OpenReadStream((1024 * 10) * 1024).CopyToAsync(fs);
                _file = path;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fehler beim Upload der Datei");
                _userMessage = new MarkupString("Fehler beim Upload der Datei.");
            }
            _isBusy = false;
            await InvokeAsync(StateHasChanged);
        }

        private async Task ImportVideosFromFile()
        {
            var list = GetVideoIdsFromFile(_file, _selectedColumn.ToString());
            var videos = list.Select(id => new Video
            {
                PublicId = id,
                UserId = UserId
            }).ToList();

            await ImportVideosUseCase.ExecuteAsync(videos);

            _userMessage = new MarkupString($"File uploaded. {list.Length} videos found.");
            await InvokeAsync(StateHasChanged);

            VideoList = await GetVideosByUserIdUseCase.ExecuteAsync(UserId);
        }

        public string[] GetVideoIdsFromFile(string path, string column)
        {
            string[] videoIds = ListImportHelper.GetColumnValuesFromExcelFileIgnoreFormularErrors(path, column);
            return videoIds;
        }

        private async Task CopyUserId()
        {
            var content = UserId.ToString();
            await JSRuntime.InvokeAsync<string>("navigator.clipboard.writeText", content);
        }

        private async Task ExportExcel()
        {
            if (_isBusy)
                return;
            _isBusy = true;
            _userMessage = new MarkupString();
            _isDownloadReady = false;
            var exportList = new List<YoutubeCommentVideoDetails>();
            string filePath = String.Concat("video_comments_", VideoList.FirstOrDefault().UserId);

            var videosToExport = VideoList
                    .Where(x => x.IsChecked == true
                        && !string.IsNullOrWhiteSpace(x.VideoTitle)
                        && !string.IsNullOrWhiteSpace(x.Url))
                    .ToList();
            if (videosToExport.Count == 0)
            {
                _userMessage = new MarkupString("No videos selected for export.");
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                _downloadPath = await ExportAllVideosInChunks(
                    videosToExport,
                    filePath);

                _isDownloadReady = true;
                if (_downloadPath is not null)
                    await DownloadCurrentFile();
                await UncheckAll();
            }

            _isBusy = false;
        }

        private async Task UncheckAll()
        {
            for (int i = 0; i < VideoList.Count; i++)
            {
                VideoList[i].IsChecked = false;
            }
            // await InvokeAsync(StateHasChanged);
        }

        private async Task CheckAll()
        {
            for (int i = 0; i < VideoList.Count; i++)
            {
                VideoList[i].IsChecked = true;
            }
            // await InvokeAsync(StateHasChanged);
        }

        private async Task<string> ExportAllVideosInChunks(IList<Video> videos, string baseFilePath)
        {
            const int chunkSize = 2_000;
            int totalVideos = videos.Count;
            int numberOfChunks = (int)Math.Ceiling((double)totalVideos / chunkSize);
            if (!Directory.Exists("wwwroot/files"))
                Directory.CreateDirectory("wwwroot/files");
            string directoryPath = Path.Combine("wwwroot/files", baseFilePath);

            try
            {
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                for (int i = 0; i < numberOfChunks; i++)
                {
                    var chunk = videos.Skip(i * chunkSize).Take(chunkSize).ToList();
                    string filePath = Path.Combine(directoryPath, $"{baseFilePath}_part_{i + 1}");
                    await ExportVideoToExcel(chunk, filePath);
                }

                string zipFilePath = Path.Combine("wwwroot/files", $"{baseFilePath}.zip");
                if (File.Exists(zipFilePath))
                    File.Delete(zipFilePath);
                ZipFile.CreateFromDirectory(directoryPath, zipFilePath);
                if (File.Exists(zipFilePath))
                    Directory.Delete(directoryPath, true);
                return zipFilePath;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error exporting videos in chunks: {ex.Message}");
                return null!;
            }
        }

        private async Task<string> ExportVideoToExcel(IList<Video> videos, string filePath)
        {
            var exportList = new List<YoutubeCommentVideoDetails>();
            try
            {
                await MapVideosToExportList(videos, exportList);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            if (exportList != null)
            {
                int count = exportList.Count;
                if (count == 0)
                {
                    _userMessage = new MarkupString("No comments found for export.");
                    await InvokeAsync(StateHasChanged);
                    return null!;
                }

                ListExportHelper.ProgressChanged += ListExportProgressChanged;
                filePath = await ListExportHelper.ExportToExcel(exportList, filePath);
                ListExportHelper.ProgressChanged -= ListExportProgressChanged;
                _userMessage = new MarkupString($"Exported {count} rows to {Path.GetFileName(filePath)}.");
                await InvokeAsync(StateHasChanged);
            }
            return filePath;
        }

        private async Task MapVideosToExportList(IList<Video> videos, List<YoutubeCommentVideoDetails> exportList)
        {
            foreach (var video in videos)
            {
                video.Comments = await GetCommentsUseCase.ExecuteAsync(video.Id);
                if (video.Comments != null && video.Comments.Count > 0)
                {
                    foreach (var comment in video.Comments)
                    {
                        exportList.Add(
                            VideoMapper.MapVideoToExportType(
                                video,
                                comment,
                                includeCommentText: true));
                        if (comment.ChildComments?.Count > 0)
                        {
                            foreach (var reply in comment.ChildComments)
                            {
                                exportList.Add(
                                    VideoMapper.MapVideoToExportType(
                                        video,
                                        reply,
                                        includeCommentText: true));
                            }
                        }
                    }
                }
            }
        }

        private void ListExportProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (_downloadProgress != e.ProgressPercentage)
            {
                _downloadProgress = e.ProgressPercentage;
                InvokeAsync(StateHasChanged);
            }
        }

        private async Task DownloadCurrentFile()
        {
            Log.Information($"Downloading file: {_downloadPath}");
            using var fileStream = new FileStream(_downloadPath, FileMode.Open);
            using var streamRef = new DotNetStreamReference(stream: fileStream);
            await JSRuntime.InvokeVoidAsync("downloadFileFromStream", Path.GetFileName(_downloadPath), streamRef);
        }

        private async Task ExportSketchEngine(MouseEventArgs e)
        {
            if (_isBusy)
                return;
            _isBusy = true;
            _userMessage = new MarkupString();
            var list = new List<DocNode>();
            var videoList = VideoList
                .Where(v => v.IsChecked == true);
            if (videoList.Count() == 0)
            {
                _userMessage = new MarkupString("No videos selected for export.");
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                string filePath = String.Concat("video_comments_", VideoList.FirstOrDefault().UserId);
                foreach (var video in videoList)
                {
                    video.Comments = await GetCommentsUseCase.ExecuteAsync(video.Id);
                    if (video.Comments != null && video.Comments.Count > 0)
                    {
                        list.AddRange(VideoMapper.MapVideoToDocNode(video));
                    }
                }
                if (list.Count > 0)
                {
                    _downloadPath = ListExportHelper.ExportToSketchXml(list, filePath);
                }
                _userMessage = new MarkupString($"Data exported to SketchEngine file.");
                await InvokeAsync(StateHasChanged);
                await DownloadCurrentFile();
            }
            _isBusy = false;
        }

        private async Task DeleteVideo(Video video)
        {
            await DeleteVideoUseCase.ExecuteAsync(video);
            VideoList.Remove(video);
            await InvokeAsync(StateHasChanged);
        }

        private async Task DeleteSelectedVideos()
        {
            var videosToDelete = VideoList
                .Where(x => x.IsChecked == true)
                .ToList();
            foreach (var video in videosToDelete)
            {
                await DeleteVideo(video);
                VideoList.Remove(video);
            }
            await InvokeAsync(StateHasChanged);
        }
    }
}