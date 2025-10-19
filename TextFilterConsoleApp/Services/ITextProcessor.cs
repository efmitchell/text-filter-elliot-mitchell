namespace TextFilterConsoleApp.Services;

public interface ITextProcessor
{
    Task<string> ProcessFileAsync(string filePath);
}