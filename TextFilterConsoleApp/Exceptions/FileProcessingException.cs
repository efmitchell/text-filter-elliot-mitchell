namespace TextFilterConsoleApp.Exceptions;

public class FileProcessingException : TextProcessingException
{
    public FileProcessingException(string message, string filePath) : base(message, filePath)
    {
    }

    public FileProcessingException(string message, string filePath, Exception innerException) : base(message, filePath, innerException)
    {
    }
}