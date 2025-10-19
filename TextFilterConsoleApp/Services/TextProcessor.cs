using Microsoft.Extensions.Logging;
using TextFilterConsoleApp.Filters;

namespace TextFilterConsoleApp.Services;

public class TextProcessor : ITextProcessor
{
    private readonly IFileService _fileService;
    private readonly IEnumerable<ITextFilter> _filters;
    private readonly ILogger<TextProcessor> _logger;

    public TextProcessor(IFileService fileService, IEnumerable<ITextFilter> filters, ILogger<TextProcessor> logger)
    {
        _fileService = fileService;
        _filters = filters;
        _logger = logger;
    }

    public Task<string> ProcessFileAsync(string filePath)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}