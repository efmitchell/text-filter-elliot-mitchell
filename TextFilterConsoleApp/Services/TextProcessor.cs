using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TextFilterConsoleApp.Exceptions;
using TextFilterConsoleApp.Filters;

namespace TextFilterConsoleApp.Services;

public class TextProcessor : ITextProcessor
{
    private readonly IFileService _fileService;
    private readonly IEnumerable<ITextFilter> _filters;
    private readonly ILogger<TextProcessor> _logger;
    private readonly int _chunkSize;
    private static readonly Regex WordSeparatorRegex = new(@"\s+", RegexOptions.Compiled);

    public TextProcessor(
        IFileService fileService,
        IEnumerable<ITextFilter> filters,
        IConfiguration configuration,
        ILogger<TextProcessor> logger)
    {
        _fileService = fileService;
        _filters = filters;
        _logger = logger;
        _chunkSize = configuration.GetValue<int>("Processing:ChunkSize", 1000);
    }

    public async Task<string> ProcessFileAsync(string filePath)
    {
        try
        {
            _logger.LogInformation("Starting text processing for file: {FilePath}", filePath);

            var enabledFilters = _filters.Where(f => f.IsEnabled).ToList();

            var chunks = new List<Task<string>>();
            var wordBuffer = new List<string>();

            await foreach (var line in _fileService.ReadTextAsync(filePath))
            {
                var words = SplitIntoWords(line);
                wordBuffer.AddRange(words);

                while (wordBuffer.Count >= _chunkSize)
                {
                    var chunkWords = wordBuffer.Take(_chunkSize).ToList();
                    wordBuffer.RemoveRange(0, _chunkSize);
                    chunks.Add(ProcessWordChunkAsync(chunkWords, enabledFilters));
                }
            }

            if (wordBuffer.Count > 0)
            {
                chunks.Add(ProcessWordChunkAsync(wordBuffer.ToList(), enabledFilters));
            }

            if (chunks.Count == 0)
            {
                _logger.LogInformation("No content to process.");
                return string.Empty;
            }

            if (chunks.Count == 1)
            {
                _logger.LogInformation("Processing single chunk");
                var result = await chunks[0];
                var finalSingleResult = result.Trim();
                _logger.LogInformation("Text processing completed. Output length: {Length} characters",
                    finalSingleResult.Length);
                return finalSingleResult;
            }

            _logger.LogInformation("Processing {ChunkCount} chunks in parallel", chunks.Count);
            var chunkResults = await Task.WhenAll(chunks);

            var filteredResults = chunkResults.Where(r => !string.IsNullOrEmpty(r));
            var finalResult = string.Join(" ", filteredResults).Trim();
            _logger.LogInformation("Text processing completed. Output length: {Length} characters", finalResult.Length);

            return finalResult;
        }
        catch (FileProcessingException)
        {
            throw;
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "File I/O error processing file: {FilePath}", filePath);
            throw new FileProcessingException($"Failed to read or access file: {filePath}", filePath, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Access denied processing file: {FilePath}", filePath);
            throw new FileProcessingException($"Access denied to file: {filePath}", filePath, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing file: {FilePath}", filePath);
            throw new FileProcessingException($"Unexpected error processing file: {filePath}", filePath, ex);
        }
    }

    private static Task<string> ProcessWordChunkAsync(List<string> words, List<ITextFilter> filters)
    {
        return Task.Run(() =>
        {
            try
            {
                var sb = new StringBuilder(words.Count * 10);

                foreach (var word in words)
                {
                    var cleanWord = CleanWord(word);
                    if (string.IsNullOrWhiteSpace(cleanWord))
                    {
                        continue;
                    }

                    var shouldFilter = filters.Any(filter => filter.ShouldFilter(cleanWord));

                    if (!shouldFilter)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(' ');
                        }

                        sb.Append(word);
                    }
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new ChunkProcessingException($"Error processing chunk", ex);
            }
        });
    }

    private static string[] SplitIntoWords(string text)
    {
        return WordSeparatorRegex.Split(text)
            .Where(word => !string.IsNullOrWhiteSpace(word))
            .ToArray();
    }

    private static string CleanWord(string word)
    {
        if (word.All(c => char.IsLetter(c) || char.IsDigit(c)))
        {
            return word;
        }

        return new string(word.Where(c => char.IsLetter(c) || char.IsDigit(c)).ToArray());
    }
}