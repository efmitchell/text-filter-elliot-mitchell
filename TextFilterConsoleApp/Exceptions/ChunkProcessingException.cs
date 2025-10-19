namespace TextFilterConsoleApp.Exceptions;

public class ChunkProcessingException : TextProcessingException
{
    public int ChunkOrder { get; }

    public ChunkProcessingException(string message) : base(message)
    {
    }

    public ChunkProcessingException(string message, Exception innerException) : base(message, innerException)
    {
    }
}