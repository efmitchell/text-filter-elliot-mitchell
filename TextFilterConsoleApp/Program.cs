// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TextFilterConsoleApp.Filters;
using TextFilterConsoleApp.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddSingleton<ITextProcessor, TextProcessor>();

builder.Services.AddSingleton<ITextFilter, VowelMiddleFilter>();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
var configuration = app.Services.GetRequiredService<IConfiguration>();
var textProcessor = app.Services.GetRequiredService<ITextProcessor>();

try
{
    logger.LogInformation("ext Filter Application Starting...");
    
    var inputFilePath = configuration.GetValue<string>("Processing:InputFilePath", "input.txt");
    
    if (!File.Exists(inputFilePath))
    {
        logger.LogError("Input file not found: {FilePath}", inputFilePath);
        return;
    }

    var filteredText = await textProcessor.ProcessFileAsync(inputFilePath);
    
    Console.WriteLine("==== FILTERED TEXT OUTPUT ====");
    Console.WriteLine(filteredText);
    Console.WriteLine("==============================");
    
    logger.LogInformation("Text filtering completed successfully.");
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while processing the text");
}