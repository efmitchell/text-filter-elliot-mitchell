using System.Runtime.CompilerServices;

namespace TextFilterConsoleApp.Services;

public class FileService : IFileService
{
    public async IAsyncEnumerable<string> ReadTextAsync(
        string filePath, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        using var reader = new StreamReader(filePath);
        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            yield return line;
        }
    }
}