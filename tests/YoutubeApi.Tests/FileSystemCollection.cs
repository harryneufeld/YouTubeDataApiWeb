namespace YoutubeApi.Tests;

/// <summary>
/// Groups tests that share files on disk (export folder, SQLite database) so they don't run in parallel.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public class FileSystemCollection
{
    public const string Name = "FileSystem";
}
