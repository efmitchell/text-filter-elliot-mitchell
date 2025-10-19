using Microsoft.Extensions.Configuration;

namespace TextFilterConsoleApp.Filters;

public class MinimumLengthFilter : ITextFilter
{
    public string Name => "MinimumLengthFilter";
    public bool IsEnabled { get; }
    public int MinimumLength { get; }

    public MinimumLengthFilter(IConfiguration configuration)
    {
        IsEnabled = configuration.GetValue($"TextFilters:{Name}:Enabled", true);
        MinimumLength = configuration.GetValue($"TextFilters:{Name}:MinimumLength", 3);
    }

    public bool ShouldFilter(string word)
    {
        if (!IsEnabled)
        {
            return false;
        }

        if (string.IsNullOrEmpty(word))
        {
            return true; 
        }

        var cleanWord = word.Trim();
        return cleanWord.Length < MinimumLength;
    }
}