namespace TextFilterConsoleApp.Exceptions;

public class TextProcessingException : Exception
{
    public string? FilePath { get; }

    public TextProcessingException(string message) : base(message)
    {
    }

    public TextProcessingException(string message, string filePath) : base(message)
    {
        FilePath = filePath;
    }

    public TextProcessingException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public TextProcessingException(string message, string filePath, Exception innerException) : base(message, innerException)
    {
        FilePath = filePath;
    }
}