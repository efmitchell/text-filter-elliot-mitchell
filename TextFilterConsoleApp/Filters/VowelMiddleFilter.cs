using Microsoft.Extensions.Configuration;

namespace TextFilterConsoleApp.Filters;

public class VowelMiddleFilter : ITextFilter
{
    private static readonly HashSet<char> Vowels = new() { 'a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U' }; // Ignoring Y
    
    public string Name => "VowelMiddleFilter";
    public bool IsEnabled { get; }

    public VowelMiddleFilter(IConfiguration configuration)
    {
        IsEnabled = configuration.GetValue($"TextFilters:{Name}:Enabled", true);
    }

    public bool ShouldFilter(string word)
    {
        if (!IsEnabled || string.IsNullOrEmpty(word))
        {
            return false;
        }

        var cleanWord = word.Trim().ToLowerInvariant();
        if (cleanWord.Length < 3)
        {
            return false;
        }

        var middleChars = GetMiddleCharacters(cleanWord);
        return middleChars.Any(c => Vowels.Contains(c));
    }

    private static IEnumerable<char> GetMiddleCharacters(string word)
    {
        var length = word.Length;
        var middleIndex = length / 2;

        if (length % 2 == 1)
        {
            yield return word[middleIndex];
        }
        else
        {
            yield return word[middleIndex - 1];
            yield return word[middleIndex];
        }
    }
}