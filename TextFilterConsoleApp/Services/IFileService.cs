namespace TextFilterConsoleApp.Services;

public interface IFileService
{
    IAsyncEnumerable<string> ReadTextAsync(string filePath, CancellationToken cancellationToken = default);
}