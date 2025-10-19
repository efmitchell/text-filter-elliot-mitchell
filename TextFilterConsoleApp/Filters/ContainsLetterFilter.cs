using Microsoft.Extensions.Configuration;

namespace TextFilterConsoleApp.Filters;

public class ContainsLetterFilter : ITextFilter
{
    public string Name => "ContainsLetterFilter";
    public bool IsEnabled { get; }
    public char TargetLetter { get; }

    public ContainsLetterFilter(IConfiguration configuration)
    {
        IsEnabled = configuration.GetValue<bool>($"TextFilters:{Name}:Enabled", true);
        var letterConfig = configuration.GetValue<string>($"TextFilters:{Name}:Letter", "t");
        TargetLetter = !string.IsNullOrEmpty(letterConfig) ? letterConfig.ToLowerInvariant()[0] : 't';
    }

    public bool ShouldFilter(string word)
    {
        if (!IsEnabled || string.IsNullOrEmpty(word))
        {
            return false;
        }

        var cleanWord = word.Trim().ToLowerInvariant();
        return cleanWord.Contains(TargetLetter);
    }
}