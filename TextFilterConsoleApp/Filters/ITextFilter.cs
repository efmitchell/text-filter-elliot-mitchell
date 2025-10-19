namespace TextFilterConsoleApp.Filters;

public interface ITextFilter
{
    string Name { get; }
    bool IsEnabled { get; }
    bool ShouldFilter(string word);
}